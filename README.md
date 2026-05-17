# Logs Triage Sample Web API

This service intentionally behaves like a small backend application so incident and triage tooling can see useful stack traces. It includes minimal API endpoints, handlers, services, repositories, managers, payment strategies, external providers, options, DTOs, and custom exception types.

## Run

```powershell
cd LogsTriageSampleWebApi
dotnet run
```

Swagger is enabled in all environments. The `.http` file also contains ready-to-run requests.

## Useful Endpoints

- `GET /api/test` verifies the service through the health service.
- `POST /api/data` stores a small data submission through the submission service/repository.
- `GET /api/error` throws an unhandled diagnostics exception from the service layer.
- `GET /api/issues` lists the original focused issue scenarios.
- `GET /api/orders/scenarios` lists layered order-processing scenarios.
- `POST /api/orders` runs the full customer, pricing, payment, inventory, fulfillment, and save flow.
- `POST /api/orders/{orderId}/transitions` applies an order state transition.

## Order Exception Triggers

Send these JSON bodies to `POST /api/orders` unless another path is listed.

| Scenario | Trigger |
| --- | --- |
| Happy path | `{"customerId":"cust-ok","sku":"WIDGET-A","quantity":1,"paymentMethod":"credit-card","shipToRegion":"west"}` |
| Validation failure | `{"customerId":"","sku":"WIDGET-A","quantity":0,"paymentMethod":"credit-card"}` |
| Missing data | `{"customerId":"cust-missing","sku":"WIDGET-A","quantity":1,"paymentMethod":"credit-card"}` |
| Null reference | `{"customerId":"cust-null-billing","sku":"WIDGET-A","quantity":1,"paymentMethod":"credit-card"}` |
| Invalid business state | `{"customerId":"cust-suspended","sku":"WIDGET-A","quantity":1,"paymentMethod":"credit-card"}` |
| Repository/data access chain | `{"customerId":"cust-ok","sku":"DB-DEADLOCK","quantity":1,"paymentMethod":"credit-card"}` |
| External provider failure | `{"customerId":"cust-ok","sku":"WIDGET-A","quantity":1,"paymentMethod":"declined-network"}` |
| Timeout-like failure | `{"customerId":"cust-ok","sku":"WIDGET-A","quantity":1,"paymentMethod":"timeout-card"}` |
| Retry exhaustion | `{"customerId":"cust-ok","sku":"WIDGET-A","quantity":1,"paymentMethod":"flaky-card"}` |
| Strategy resolution failure | `{"customerId":"cust-ok","sku":"WIDGET-A","quantity":1,"paymentMethod":"crypto"}` |
| Configuration failure | `{"customerId":"cust-ok","sku":"WIDGET-A","quantity":1,"paymentMethod":"credit-card","couponCode":"RISKY"}` |
| Inconsistent layered state | `{"customerId":"cust-inconsistent","sku":"WIDGET-A","quantity":3,"paymentMethod":"credit-card"}` |
| Specific input combination | `{"customerId":"cust-vip","sku":"SUBSCRIPTION-PRO","quantity":1,"paymentMethod":"invoice","couponCode":"WELCOME10"}` |
| Nested provider chain | `{"customerId":"cust-ok","sku":"WIDGET-A","quantity":1,"paymentMethod":"credit-card","shipToRegion":"provider-down"}` |
| Invalid transition | `POST /api/orders/ord-shipped/transitions` with `{"targetStatus":"Cancelled","reason":"customer requested cancellation"}` |
