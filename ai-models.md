# AutoMarket — AI/ML Models Specification

## 1. AI Mission

The AI layer provides decision support for vehicle discovery, valuation, semantic search, image-quality assistance, personalization, and operational analytics.

The system must be useful without pretending to know what it cannot know.

## 2. AI Architecture

```text
Next.js
  ↓
ASP.NET Core API
  ↓
AI Orchestrator
  ↓
FastAPI / Python
  ├── Recommendation
  ├── Semantic Search
  ├── Price Valuation
  ├── Image Condition Assistance
  ├── Personalization
  └── Forecasting
       ↓
Feature processing / vector retrieval
       ↓
Model registry
       ↓
Evaluation + monitoring
```

## 3. Model Inventory

| Model | Purpose | Suggested baseline |
|---|---|---|
| Recommendation | Match users to cars | Hybrid scoring + learning-to-rank evolution |
| Semantic Search | Understand natural-language intent | Sentence-transformer embeddings + structured filters |
| Valuation | Estimate used-car market range | XGBoost / Gradient Boosting |
| Image Condition | Detect visible conditions | CNN/transformer vision classifier |
| Personalization | Adapt ranking to preferences | Hybrid content + behavior model |
| Seller Quality | Workflow prioritization | Gradient boosting/logistic model |
| Demand Forecast | Inventory/search demand | Time-series baseline + boosting |

## 4. Recommendation Model

### Inputs

- explicit budget;
- brand/model preferences;
- fuel type;
- transmission;
- body type;
- location;
- year range;
- mileage range;
- explicit feature preferences;
- permitted interaction history such as views, favourites, and comparisons.

### Candidate generation

1. Deterministic filters.
2. Text/keyword search.
3. Vector similarity retrieval.
4. Availability filtering.

### Ranking

Initial baseline:

```text
score =
  0.30 * budget_fit
+ 0.20 * preference_fit
+ 0.15 * feature_fit
+ 0.15 * location_fit
+ 0.10 * recency
+ 0.10 * engagement_quality
```

The exact coefficients are experiment configuration, not UI logic.

### Output

```json
{
  "carId": "uuid",
  "score": 0.91,
  "reasons": [
    "Fits the selected budget",
    "Matches automatic transmission",
    "Located in the selected city"
  ],
  "confidence": 0.88,
  "modelVersion": "recommendation-v1.0"
}
```

## 5. Semantic Search

Example user request:

> automatic SUV under 10 lakh for city use and occasional highway travel

Pipeline:

```text
query
 ↓
intent/entity extraction
 ↓
structured constraints
 ↓
embedding
 ↓
vector retrieval
 ↓
filter enforcement
 ↓
ranker
 ↓
explanation
```

The parser must never be allowed to remove authorization or transaction constraints.

## 6. Valuation Model

### Features

- make;
- model;
- variant;
- year;
- registration year;
- mileage;
- fuel;
- transmission;
- ownership count;
- body type;
- city/state;
- condition signals;
- service-history availability;
- market-time features.

### Training strategy

Use time-aware holdout data where market drift is meaningful.

Avoid leakage from:

- post-sale information;
- target-derived fields;
- duplicate listings of the same vehicle across train/test;
- future market observations leaking into historical training.

### Output

Always prefer a range:

```json
{
  "estimatedLow": 615000,
  "estimatedMid": 665000,
  "estimatedHigh": 720000,
  "currency": "INR",
  "confidence": 0.82,
  "topFactors": [
    "Vehicle age",
    "Kilometres driven",
    "Variant",
    "Local market conditions"
  ],
  "modelVersion": "valuation-v1.0"
}
```

The UI must label this as an estimate, not a guaranteed sale price.

## 7. Image Condition Model

### Goal

Assist in identifying **visible** characteristics in vehicle images.

### Allowed outputs

- image quality sufficient/insufficient;
- visible minor damage likely;
- visible moderate damage likely;
- visible major damage likely;
- unclear.

### Forbidden inference

The model must not state that an image proves:

- hidden mechanical condition;
- structural integrity;
- complete accident history;
- legal ownership;
- odometer authenticity.

### Pipeline

```text
upload
 ↓
MIME/size validation
 ↓
quality check
 ↓
normalization
 ↓
vision model
 ↓
confidence calibration
 ↓
policy check
 ↓
result or human review
```

## 8. Personalization

Use product signals and explicit preferences:

- filters;
- views;
- favourites;
- comparisons;
- appointments;
- budget selections;
- declared fuel/transmission/body-type preferences.

Do not use sensitive personal attributes for ranking or recommendations.

## 9. Seller Quality / Lead Model

Purpose: operational prioritization, not automated rejection.

Potential features:

- listing completeness;
- image quality;
- appointment readiness;
- response behavior;
- missing data rate.

High-risk or low-quality cases should be reviewable by operations staff.

## 10. Forecasting

Use for internal insights:

- search volume;
- inventory demand;
- booking demand;
- popular categories.

Start with interpretable baselines before complex models.

## 11. Feature Engineering

Maintain reusable, versioned transforms for:

- numeric normalization;
- categorical encoding;
- age calculation;
- mileage bands;
- price bands;
- regional features;
- text embeddings.

Training and serving must use compatible preprocessing versions.

## 12. Model Registry

Each model release must store:

- model name;
- model version;
- training dataset version;
- feature schema version;
- preprocessing version;
- code commit;
- evaluation metrics;
- approval status;
- deployment status;
- owner;
- rollback version.

## 13. Evaluation

### Recommendation

- Precision@K
- Recall@K
- NDCG@K
- favourite rate
- booking conversion, where valid

### Semantic search

- retrieval recall;
- precision@K;
- intent extraction accuracy;
- human relevance score.

### Valuation

- MAE;
- RMSE;
- median absolute percentage error;
- interval coverage;
- performance by make/model/region/price band.

### Image condition

- precision;
- recall;
- F1;
- confusion matrix;
- calibration.

### Forecasting

- MAE;
- RMSE;
- sMAPE when meaningful.

## 14. AI Alignment and Guardrails

### Principle 1 — AI must not override hard business rules

Examples:

- AI cannot book a sold car.
- AI cannot bypass authentication.
- AI cannot change a user's role.
- AI cannot approve a transaction by itself.

### Principle 2 — No fabricated explanation

Reasons shown to users must derive from real features/signals available to the model and application.

### Principle 3 — Confidence-aware output

For every consequential prediction return:

- prediction;
- confidence;
- model version;
- timestamp;
- important factors;
- uncertainty/fallback state.

### Principle 4 — Human review

Human review should exist for:

- seller rejection;
- major condition claims;
- disputed valuations;
- suspicious or high-risk listings;
- low-confidence cases with material consequences.

### Principle 5 — Graceful degradation

If AI is unavailable:

```text
AI failure
 ↓
deterministic fallback
 ↓
continue core workflow
```

## 15. Monitoring

Monitor:

- inference latency;
- error rate;
- confidence distribution;
- data drift;
- feature drift;
- model performance degradation;
- recommendation diversity;
- valuation error by segment;
- image-classification false positives/negatives.

## 16. Retraining

Retraining triggers may include:

- scheduled cadence;
- meaningful drift;
- performance degradation;
- major inventory distribution changes;
- new vehicle categories.

Every retrained model must pass evaluation gates before deployment.

## 17. AI API Contract

Example valuation request:

```json
{
  "brand": "ExampleBrand",
  "model": "ExampleModel",
  "variant": "ExampleVariant",
  "registrationYear": 2023,
  "kilometers": 28000,
  "fuelType": "PETROL",
  "transmission": "AUTOMATIC",
  "city": "Pune",
  "ownershipCount": 1,
  "conditionScore": 0.86
}
```

Example response:

```json
{
  "prediction": {
    "low": 615000,
    "mid": 665000,
    "high": 720000
  },
  "confidence": 0.82,
  "model": "valuation-v1.0",
  "status": "OK"
}
```

## 18. Production Readiness Gate

A model is not production-ready until:

- training data lineage is documented;
- evaluation metrics meet agreed thresholds;
- inference schema is versioned;
- model artifact is reproducible;
- failure behavior is defined;
- logging is active;
- monitoring exists;
- rollback is possible;
- user-facing limitations are documented.
