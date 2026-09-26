import os
os.environ["AI_API_KEY"] = "test-only-internal-key"
from fastapi.testclient import TestClient
from app.main import app

client = TestClient(app)
headers = {"x-api-key": "test-only-internal-key"}

def candidate(**changes):
    return dict(carId="vehicle-a", available=True, budgetFit=1, preferenceFit=0,
                featureFit=0, locationFit=1, recency=0, engagementQuality=0) | changes

def test_internal_auth_required():
    assert client.post("/recommendations", json={"candidates": []}).status_code == 401

def test_unavailable_inventory_never_ranked():
    r = client.post("/recommendations", headers=headers, json={"candidates": [candidate(available=False)]})
    assert r.status_code == 200 and r.json() == []

def test_reasons_are_supported_by_features():
    r = client.post("/recommendations", headers=headers, json={"candidates": [candidate()]})
    item = r.json()[0]
    assert item["score"] == 0.45
    assert item["reasons"] == ["Fits your selected budget", "Located in your selected city"]
    assert item["confidence"] is None

def test_out_of_range_features_rejected():
    assert client.post("/recommendations", headers=headers, json={"candidates": [candidate(budgetFit=2)]}).status_code == 422

def test_no_model_does_not_fabricate_a_price(monkeypatch, tmp_path):
    monkeypatch.setenv("MODEL_REGISTRY_PATH", str(tmp_path))
    r = client.post("/valuation", headers=headers, json={"brand": "Honda", "model": "City", "registrationYear": 2020, "kilometers": 40000, "ownershipCount": 1})
    assert r.status_code == 200
    assert r.json()["prediction"] is None
    assert r.json()["humanReviewRequired"] is True
