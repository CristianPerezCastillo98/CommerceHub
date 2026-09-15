\# CommerceHub



CommerceHub is a backend REST API built with \*\*ASP.NET Core and .NET\*\*, developed as a practical project to explore backend development, API design, persistence, testing, architecture, and cloud technologies.



The project is being developed incrementally, evolving from a simple REST API into a more complete backend application.



\## Current Features



\- REST API with ASP.NET Core

\- Product management

&#x20; - Get all products

&#x20; - Get product by ID

&#x20; - Create products

\- PostgreSQL persistence

\- Entity Framework Core

\- Code First migrations

\- Fluent API entity configuration

\- Repository pattern

\- Unit of Work

\- Dependency Injection

\- Asynchronous database operations

\- CancellationToken propagation

\- Request validation

\- Database constraints



\## Tech Stack



\- .NET

\- ASP.NET Core Web API

\- C#

\- Entity Framework Core

\- PostgreSQL

\- Npgsql

\- Swagger / OpenAPI

\- Git



\## Project Structure



```text

CommerceHub

├── CommerceHub.Api

│   └── REST API and HTTP endpoints

│

├── CommerceHub.Library

│   └── Application services and business logic

│

├── CommerceHub.Persistence

│   └── EF Core, repositories and database access

│

└── CommerceHub.Api.Tests

&#x20;   └── Automated tests

```



\## Database



CommerceHub uses PostgreSQL with Entity Framework Core.



Database schema changes are managed through EF Core migrations.



The current `Product` entity includes database-level constraints for:



\- Positive prices

\- Non-negative stock

\- Product name maximum length

\- Product description maximum length

\- Decimal precision for prices



\## API



Current endpoints:



| Method | Endpoint | Description |

|---|---|---|

| GET | `/api/v1/products` | Get all products |

| GET | `/api/v1/products/{id}` | Get a product by ID |

| POST | `/api/v1/products` | Create a product |



\## Running the Project



A PostgreSQL database and a valid connection string are required.



The connection string is configured using .NET User Secrets:



```bash

dotnet user-secrets set "ConnectionStrings:CommerceHubDatabase" "YOUR\_CONNECTION\_STRING"

```



Then run:



```bash

dotnet run --project CommerceHub.Api

```



\## Database Migrations



Apply existing migrations with:



```bash

dotnet ef database update --project CommerceHub.Persistence --startup-project CommerceHub.Api

```



Create a new migration with:



```bash

dotnet ef migrations add MigrationName --project CommerceHub.Persistence --startup-project CommerceHub.Api

```



\## Roadmap



CommerceHub will progressively include:



\- Complete product CRUD

\- Global exception handling

\- Improved API error responses

\- Unit and integration testing

\- Authentication and authorization

\- Additional domain entities

\- Advanced EF Core querying

\- Docker

\- Caching

\- Messaging

\- CI/CD

\- Cloud deployment

\- Observability

\- AI-powered features



\## Purpose



This project is focused on applying backend engineering concepts in a real application while progressively improving its architecture, maintainability, testing strategy, and infrastructure.

