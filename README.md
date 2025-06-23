# 🛒 OrderService Microservice — Clean Architecture (.NET 8)

This repository hosts a cleanly architected **OrderService** microservice using **.NET 8**, showcasing domain-driven design principles with modern microservice patterns and integrations like **Redis**, **Kafka**, **MediatR**, and **Polly**.

---

## 📐 Architecture Overview

This project follows the **Clean Architecture** approach to maintain separation of concerns and long-term scalability:

src/
├── Core/                # 🧠 Domain layer - business entities and interfaces
│   ├── Entities/        #   - Order and related domain models
│   └── Interfaces/      #   - Abstractions like IOrderRepository, IKafkaProducer
│
├── Application/         # ⚙️ Application layer - business use cases & CQRS logic
│   ├── DTOs/            #   - Data transfer objects
│   ├── Interfaces/      #   - Abstractions to decouple infrastructure
│   └── Features/
│       └── Orders/      #   - CQRS commands, queries, and handlers for orders
│           ├── Commands/
│           └── Queries/
│
├── Infrastructure/      # 🏗️ Infrastructure layer - external service implementations
│   ├── Kafka/           #   - Kafka producer logic using Confluent.Kafka
│   ├── Persistence/     #   - EF Core DbContext and repository implementation
│   └── Services/        #   - NotificationService implementation using HttpClient + Polly
│
├── WebAPI/              # 🌐 API layer - HTTP endpoints and DI setup
    ├── Controllers/     #   - OrdersController and mock NotificationsController
    └── Program.cs       #   - Application entry point and service registration




### Key Libraries & Decisions

| Tech/Tool           | Reason for Use |
|---------------------|----------------|
| **MediatR**          | Implements **CQRS** pattern for decoupling commands/queries from controllers |
| **Redis (StackExchange.Redis)** | Caches `GET /orders/{id}` for 5 minutes to improve performance |
| **Kafka (Confluent.Kafka)** | Publishes `orders.created` event for asynchronous communication with external systems |
| **Polly**            | Adds resiliency to HTTP calls with **retry logic (exponential backoff)** |
| **In-Memory DB**     | Used via EF Core for simplicity during development (can be replaced with SQL Server or PostgreSQL) |
| **Docker Compose**   | Orchestrates all microservices (Kafka, Redis, OrderService) for local development |

---

## 🔌 Integration Points

| Integration           | Description |
|------------------------|-------------|
| **NotificationService** | Simulated with a mock controller that logs incoming HTTP POST requests |
| **Kafka Broker**        | Publishes to topic `orders.created` with order details |
| **Redis Cache**         | Caches GET order responses with 5-minute expiration |
| **Polly Resilience**    | Retries failed HTTP calls with backoff logic to ensure NotificationService communication is robust |

---

## ⚙️ How to Run Locally

### 🚀 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (running Linux containers)

---

### ▶️ Running with Docker Compose

1. **Clone the Repository**
   ```bash
   git clone https://github.com/NetCSDev/OrderService.git
   cd OrderService


