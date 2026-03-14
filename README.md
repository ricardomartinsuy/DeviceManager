# Devices API

A production-ready REST API for managing hardware devices, built with **.NET 9 / C# 13**, following **Clean Architecture** principles.

## Architecture

```
DevicesApi/
├── src/
│   ├── DevicesApi.Domain          # Entities, enums, exceptions, interfaces
│   ├── DevicesApi.Application     # Use-case services, DTOs
│   ├── DevicesApi.Infrastructure  # EF Core, PostgreSQL, repository
│   └── DevicesApi.Api             # Controllers, middleware, program
└── tests/
    ├── DevicesApi.Tests.Unit        # xUnit + FluentAssertions + NSubstitute
    └── DevicesApi.Tests.Integration # WebApplicationFactory + Testcontainers
```

**Domain rules enforced at the entity level:**
- `CreationTime` is immutable — set once on creation, never updated
- `Name` and `Brand` cannot be changed when state is `in-use`
- Devices in `in-use` state cannot be deleted

## Prerequisites

- [Docker](https://www.docker.com/products/docker-desktop/) (for running the full stack)
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9) (for local development and tests)

## Running with Docker

```bash
docker compose up --build -d
```

The API will be available at `http://localhost:8080`.

| Endpoint | URL |
|----------|-----|
| Swagger UI | http://localhost:8080/swagger |
| Health Check | http://localhost:8080/health |

To stop:
```bash
docker compose down
```

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| `POST` | `/api/devices` | Create a device |
| `GET` | `/api/devices` | List all devices (supports `?brand=` and `?state=` filters) |
| `GET` | `/api/devices/{id}` | Get device by ID |
| `PUT` | `/api/devices/{id}` | Full update |
| `PATCH` | `/api/devices/{id}` | Partial update |
| `DELETE` | `/api/devices/{id}` | Delete device |

**Device states:** `0` = Available, `1` = InUse, `2` = Inactive

### Example Requests

```bash
# Create a device
curl -X POST http://localhost:8080/api/devices \
  -H "Content-Type: application/json" \
  -d '{"name": "MacBook Pro", "brand": "Apple", "state": 0}'

# List all devices
curl http://localhost:8080/api/devices

# Filter by brand
curl "http://localhost:8080/api/devices?brand=Apple"

# Filter by state (1 = InUse)
curl "http://localhost:8080/api/devices?state=1"

# Get by ID
curl http://localhost:8080/api/devices/{id}

# Full update (PUT)
curl -X PUT http://localhost:8080/api/devices/{id} \
  -H "Content-Type: application/json" \
  -d '{"name": "MacBook Air", "brand": "Apple", "state": 2}'

# Partial update (PATCH)
curl -X PATCH http://localhost:8080/api/devices/{id} \
  -H "Content-Type: application/json" \
  -d '{"state": 0}'

# Delete
curl -X DELETE http://localhost:8080/api/devices/{id}
```

### HTTP Status Codes

| Code | Meaning |
|------|---------|
| `201` | Device created |
| `200` | Success |
| `204` | Deleted |
| `404` | Device not found |
| `409` | Conflict — device in use (cannot edit name/brand or delete) |

## Running Tests

Requires Docker running (integration tests use Testcontainers).

```bash
# Unit tests only
dotnet test tests/DevicesApi.Tests.Unit

# Integration tests (spins up a real PostgreSQL container)
dotnet test tests/DevicesApi.Tests.Integration

# All tests
dotnet test DevicesApi.sln
```

**Coverage:** 33 tests total — 20 unit, 13 integration.

## Local Development (without Docker)

Start a local PostgreSQL instance and update the connection string in `src/DevicesApi.Api/appsettings.json`, then:

```bash
dotnet run --project src/DevicesApi.Api
```

The API will run at `https://localhost:5001` / `http://localhost:5000`.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core 9 |
| Language | C# 13 |
| ORM | Entity Framework Core 9 |
| Database | PostgreSQL 17 |
| Logging | Serilog |
| Documentation | Swagger / OpenAPI (Swashbuckle) |
| Containerization | Docker + Docker Compose |
| Unit Tests | xUnit + FluentAssertions + NSubstitute |
| Integration Tests | xUnit + Testcontainers + WebApplicationFactory |
