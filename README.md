# Restaurant OMS – Take-Away Order Management System 

---

## Table of Contents
1. [Project Goals](#1-project-goals)
2. [Tech Stack](#2-tech-stack)
3. [Architecture Overview](#3-architecture-overview)
4. [Key Features](#4-key-features)
5. [Order Lifecycle](#5-order-lifecycle)
6. [Solution Structure](#6-solution-structure)
7. [Getting Started](#7-getting-started)
8. [Running the Stack with Docker Compose](#8-running-the-stack-with-docker-compose)
9. [Security](#9-security)
10. [Testing](#10-testing)
11. [Assumptions & Constraints](#11-assumptions--constraints)
12. [Future Improvements](#12-future-improvements)

---

## 1. Project Goals
* Manage the **full lifecycle of a takeaway order** – from placement and payment to kitchen prep, pickup or courier delivery.
* Showcase **Clean Architecture**, SOLID principles, and modern .NET practices.

---

## 2. Tech Stack
| Area | Choices                             |
|------|-------------------------------------|
| Runtime | **.NET 9** (ASP.NET Core)           |
| Data Access | **EF Core 9** (SQLServer provider)  |
| Mapping | **Static Classes**                  |
| AuthN / AuthZ | **ASP.NET Identity** + JWT (Bearer) |
| Validation | **FluentValidation 12**             |
| Containerisation | **Docker / Docker Compose**         |
| Tests | **xUnit**, **TestContainers**       |

---

## 3. Architecture Overview
```
API  (ASP.NET) 
│
├─ Application            
│   ├─ Interfaces         e.g., IPaymentService
│   └─ Validators         FluentValidation rules
│
├─ Domain
│   ├─ Repositories       IOrderRepository, IMenuItemRepository
│   └─ Value Objects      Money, Address
│
├─ Infrastructure
│   ├─ Persistence        EF Core
│   ├─ Payments           Stripe / PayPal stubs
│   ├─ Identity           JWT generation (ASP.NET Identity)
│   └─ Migrations         EF Core Migrations
│
└─ Tests                  Unit + Integration tests
```

* **Dependency Rule:** outer layers depend on inner ones—never the reverse.
* **Auditing Interceptor:** `CreatedAtUtc` / `UpdatedAtUtc` stamped automatically on every auditable entity.

See `docs/architecture.drawio` for the full component & sequence diagrams.

### 3.1 Architectural Explanation & Trade-Offs

| Decision                                                                    | Rationale                                                                                         | Trade-Off                                                                        |
|-----------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------|
| **Clean Architecture layers** (API → Application → Domain → Infrastructure) | Maximises maintainability and testability; outer rings can be swapped without breaking core logic. | Extra boilerplate, especially for small teams; more projects to navigate.        |
| **EF Core + SQL Server**                                                    | Strong LINQ support; migrations in-repo.                                                          | Can be missed in hand-written SQL and cause inconsistent reads.                  |
| **JWT Bearer & Role Policies**                                              | Stateless auth (no session store); simple RBAC mapping.                                           | More moving parts (refresh endpoint, rotation logic).                            |
| **FluentValidation** | Declarative, testable rules; decoupled from controllers. | Duplicate rules if client-side validation is later required.                     |
| **Stubbed payment gateways**                                                | Keeps repository runnable without secrets; demonstrates Strategy pattern.                         | Not production-ready; no real PSP error scenarios.                               |
| **Single bounded context** (monolith)                                       | Fast to develop and demo; easier local Docker stack.                                              | Limits horizontal scaling; harder to split later than starting modular.          |
| **No Outbox yet**                                                           | Reduces code for the demo                           | At-least-once guarantees require Outbox when moving to multi-service deployment. |
| **Static mapping instead of AutoMapper**                                    | Compile-time safety; zero reflection at runtime.                                                  | More manual code if DTOs explode; less flexible for deep graphs.                 |
| **Docker Compose stack**                                                    | 1-command onboarding; CI-friendly.                                                                | No orchestration; prod would need Kubernetes / ECS.                              |

---

## 4. Key Features
* **Orders**
    * Place, view, change status
    * Valid state transitions enforced
* **Payments**
    * Pluggable gateway strategy (Stripe, PayPal, Cash)
* **Kitchen / Staff**
    * Dashboard endpoints to update status
    * Assign orders
* **Couriers**
    * Mark *Out for Delivery* / *Delivered*
* **RBAC**
    * *Customer* – own orders only
    * *Staff* – all orders
    * *Delivery* – assigned orders
    * *Admin* – full access
* **Seed Data**
    * 5 menu items
    * 3 stub couriers
    * Demo accounts: `admin@demo.io`, `customer@demo.io`, `staff@demo.io`, `delivery@demo.io`, (password `Dev123!`)

---

## 5. Order Lifecycle
### 5.1 Order + Payment Flow

* **Customer pays first**  
   `POST /api/payment` → *Payment Service* calls the external gateway & returns **`transactionId`**.
* **Customer places the order**  
   `POST /api/orders` with the received `transactionId` in the body.
* **API persists the order**  
   *Order Service* inserts the `Order` row (status =`Paid`) inside a SQL transaction and returns **`201 Created`** with the new `orderId`.

See `docs/order_lifecycle.drawio` for the full component & sequence diagrams.

### 5.2 Order Status Valid Transitions

| Current State          | Next State            | Typical Trigger                                  | Role/Actor |
|------------------------|-----------------------|--------------------------------------------------|------------|
| **Pending**            | **Preparing**         | Staff accepts order → starts cooking             | Staff      |
| **Preparing**          | **Ready for Pickup**  | Food is boxed for walk-in collection             | Staff      |
| **Preparing**          | **Ready for Delivery**| Food is boxed & labelled for courier             | Staff      |
| **Ready for Delivery** | **Out for Delivery**  | Courier scans QR / is assigned in dashboard      | Courier    |
| **Out for Delivery**   | **Delivered**         | Courier confirms hand-off to customer            | Courier    |
| **Out for Delivery**   | **Unable to Deliver** | Courier marks failed drop-off (no-show, address) | Courier    |

Only the transitions above are accepted by the domain model; any other attempt raises a `InvalidOrderStateException`.

> Visual reference → open [`docs/order_status_transitions.drawio`](docs/order_status_transitions.drawio).

---


## 6. Solution Structure

```
src/
├─ OMS.Api ← HTTP endpoints
├─ OMS.Application ← Services, validators, business logic
├─ OMS.Domain ← Entities, enums
├─ OMS.Infrastructure
│ ├─ Persistence ← DbContext, migrations
│ ├─ Payments ← Gateways, stubs
│ └─ Identity ← JWT / ASP.NET Identity
└─ OMS.Tests ← Unit & integration tests
docker-compose.yml
.docker/ ← Dockerfile
README.md
```

---

## 7. Getting Started
```bash
# 1. Clone
git clone https://github.com/TheodorosKarropoulos/oms.git
cd oms

# 2. Build & run tests
dotnet restore
dotnet test

# 3. Launch the stack
cd src
docker compose up -d --build
```
Browse Swagger UI: http://localhost:8080/swagger/index.html

---
## 8. Running the Stack with Docker Compose

| Service      | Port                    | Notes                         |
|--------------|-------------------------|-------------------------------|
| `api`        | 8080                    | ASP.NET API (HTTP redirected) |
| `db` (MSSql) | 1433                    | `sa / Strong!Passw0rd`               |

Environment variables (see docker-compose.yml) control connection strings & JWT secrets.

---

## 9. Security
* JWT Bearer authentication with ASP.NET Identity
* Role-based authorization (Policies under OmsPolicies.*)
* HTTP pipeline includes global exception → ProblemDetails & logging

---
## 10. Testing

| Layer           | Highlights                                                         |
| --------------- |--------------------------------------------------------------------|
| **Domain**      | pure unit tests ensure state transitions, money maths              |
| **Application** | validator tests                                                    |
| **API**         | TestServer integration tests (fake auth handler + mocked services) |
| **E2E**  | Swagger hits live Docker stack                                     |


---

## 11. Assumptions & Constraints

- Prices are stored in EUR or USD; multi-currency support can be added with a Money converter.
- Inventory management is out of scope.
- Payment gateways are stubbed for demo.

---

## 12. Future Improvements
- Caching hot endpoints with Redis
- Centralised logging / metrics via Grafana + Prometheus
- Rate limiting using ASP.NET middleware
- Health checks
- CI pipeline (GitHub Actions) building Docker images & running tests
- User-facing notifications and emails, updates when orders change status, send receipt
- Retry / circuit-breaker policies (Polly)
- Reliable messaging with the Outbox pattern
- Domain event publishing to external systems
