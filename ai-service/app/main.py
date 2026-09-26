"""Internal advisory service. No transaction authority or unapproved model inference."""
import hmac
import json
import math
import os
from datetime import datetime, timezone
from pathlib import Path
from typing import Annotated

from fastapi import Depends, FastAPI, Header, HTTPException
from pydantic import BaseModel, ConfigDict, Field

app = FastAPI(title="AutoMarket Intelligence", version="0.1.0")


def authorize(x_api_key: Annotated[str | None, Header()] = None):
    expected = os.environ.get("AI_API_KEY", "")
    if not expected or not x_api_key or not hmac.compare_digest(expected, x_api_key):
        raise HTTPException(401, "Internal service authentication required")


class Candidate(BaseModel):
    model_config = ConfigDict(extra="forbid", allow_inf_nan=False)
    carId: str = Field(min_length=1, max_length=40)
    available: bool
    budgetFit: float = Field(ge=0, le=1)
    preferenceFit: float = Field(ge=0, le=1)
    featureFit: float = Field(ge=0, le=1)
    locationFit: float = Field(ge=0, le=1)
    recency: float = Field(ge=0, le=1)
    engagementQuality: float = Field(ge=0, le=1)


class Recommendations(BaseModel):
    model_config = ConfigDict(extra="forbid")
    candidates: list[Candidate] = Field(max_length=200)
    limit: int = Field(default=10, ge=1, le=30)


class Valuation(BaseModel):
    model_config = ConfigDict(extra="forbid", allow_inf_nan=False)
    brand: str = Field(min_length=1, max_length=80)
    model: str = Field(min_length=1, max_length=80)
    registrationYear: int = Field(ge=1950, le=datetime.now(timezone.utc).year)
    kilometers: int = Field(ge=0, le=2_000_000)
    ownershipCount: int = Field(ge=1, le=20)


@app.get("/health")
def health():
    return {"status": "alive"}


@app.get("/ready", dependencies=[Depends(authorize)])
def ready():
    return {"status": "ready", "capabilities": ["deterministic-recommendations"], "valuation": "requires-approved-artifact"}


@app.get("/models", dependencies=[Depends(authorize)])
def models():
    return [{"name": "recommendation", "version": "rules-v1", "type": "deterministic",
             "weights": [0.30, 0.20, 0.15, 0.15, 0.10, 0.10], "confidence": None},
            {"name": "valuation", "status": "NOT_CONFIGURED"},
            {"name": "image-condition", "status": "MANUAL_REVIEW_REQUIRED"},
            {"name": "semantic-search", "status": "NOT_CONFIGURED"}]


@app.post("/recommendations", dependencies=[Depends(authorize)])
def recommendations(request: Recommendations):
    result = []
    for c in request.candidates:
        if not c.available:
            continue
        score = sum(a * b for a, b in zip(
            [c.budgetFit, c.preferenceFit, c.featureFit, c.locationFit, c.recency, c.engagementQuality],
            [0.30, 0.20, 0.15, 0.15, 0.10, 0.10], strict=True))
        reasons = []
        if c.budgetFit == 1:
            reasons.append("Fits your selected budget")
        if c.preferenceFit == 1:
            reasons.append("Matches your selected preferences")
        if c.locationFit == 1:
            reasons.append("Located in your selected city")
        result.append({"carId": c.carId, "score": round(score, 6), "reasons": reasons,
                       "confidence": None, "modelVersion": "rules-v1", "method": "deterministic"})
    return sorted(result, key=lambda item: (-item["score"], item["carId"]))[:request.limit]


@app.post("/valuation", dependencies=[Depends(authorize)])
def valuation(request: Valuation):
    # Only a locally approved and trusted artifact may be loaded. No user supplied paths.
    registry = Path(os.environ.get("MODEL_REGISTRY_PATH", "models"))
    manifest_path = registry / "valuation.json"
    if not manifest_path.is_file():
        return {"status": "INSUFFICIENT_DATA", "prediction": None, "confidence": None,
                "modelVersion": None, "humanReviewRequired": True,
                "message": "No approved valuation model is configured. Continue with your expected price."}
    manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    if manifest.get("status") != "APPROVED" or manifest.get("featureSchemaVersion") != "valuation-numeric-v1":
        raise HTTPException(503, "Valuation model is not approved or compatible")
    artifact = (registry / manifest["artifact"]).resolve()
    if not artifact.is_relative_to(registry.resolve()) or not artifact.is_file():
        raise HTTPException(503, "Approved model artifact unavailable")
    import joblib
    model = joblib.load(artifact)
    prediction = float(model.predict([[datetime.now(timezone.utc).year - request.registrationYear,
                                       request.kilometers, request.ownershipCount]])[0])
    residual = float(manifest["holdoutAbsoluteErrorP90"])
    if not math.isfinite(prediction) or not math.isfinite(residual) or prediction <= 0 or residual < 0:
        raise HTTPException(503, "Model output failed validation")
    return {"status": "ESTIMATE", "prediction": {"low": max(0, prediction - residual),
            "mid": prediction, "high": prediction + residual}, "confidence": None,
            "modelVersion": manifest["version"], "humanReviewRequired": True,
            "topFactors": ["Vehicle age", "Kilometres driven", "Ownership count"],
            "message": "Numeric baseline; make/model and local market effects are not modeled. Human review required."}
