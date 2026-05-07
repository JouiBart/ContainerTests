# ContainerTests

A sample project built on **.NET 10** and **.NET Aspire**, designed to validate Kubernetes infrastructure and Azure service connectivity.

Contains:
- **Blazor Server** web application (`Aspire.Web`)
- **REST API** service (`Aspire.ApiService`)
- **Aspire AppHost** orchestrator
- **Aspire ServiceDefaults** shared configuration

The purpose of this project is to verify infrastructure components and external service integrations in a progressive and controlled way.

---

## Goals

The project is designed to help validate:
- Internal web ↔ API communication
- Kubernetes networking
- Azure service connectivity
- Secret management
- Database access
- Infrastructure readiness

---

## Architecture

```text
[ Aspire AppHost ]
        |
        +---> [ Aspire.Web  (Blazor Server) ]
        |               |
        |               v
        +---> [ Aspire.ApiService  :8081 ]
                        |
                        +--> Azure Blob Storage
                        |
                        +--> Azure Key Vault
                        |
                        +--> SQL Database
```

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Aspire workload: `dotnet workload install aspire`
- Docker Desktop

## Getting Started

```bash
dotnet run --project Aspire.AppHost
```

The Aspire Dashboard will open automatically in your browser.

## Endpoints – ApiService (port 8081)

| Endpoint | Description |
|---|---|
| `GET /` | Verify the API is running |
| `GET /weatherforecast` | Sample weather forecast data |
| `GET /azure-check` | Check Azure services connectivity |
| `GET /health` | Health check (all checks) |
| `GET /alive` | Liveness check (tag `live`) |
| `GET /healtz` | Liveness probe (Kubernetes) |
| `GET /ready` | Readiness probe (Kubernetes) |

## Azure Configuration

| Environment Variable | appsettings.json key | Description |
|---|---|---|
| `Azure__Storage__Uri` | `Azure:Storage:Uri` | Azure Blob Storage account URI |
| `Azure__KeyVault__Uri` | `Azure:KeyVault:Uri` | Azure Key Vault URI |
| `Database__ConnectionString` | `Database:ConnectionString` | SQL database connection string |

Authentication is handled via `DefaultAzureCredential` (Managed Identity, Azure CLI, etc.).

### `/azure-check` Response

```json
{
  "storage":  { "configured": true,  "connected": true,  "message": "Connection succeeded." },
  "keyVault": { "configured": true,  "connected": true,  "message": "Connection succeeded." },
  "database": { "configured": false, "connected": false, "message": "Not configured." }
}
```

---

## Version Roadmap

### v1.0.0 ✅

**Features**
- [x] Web application deployment
- [x] API deployment
- [x] Web ↔ API connection testing

**Purpose** — Validate:
- [x] Kubernetes deployment
- [x] Service discovery
- [x] Internal networking
- [x] Ingress / basic communication

### v2.0.0 ✅

**Features**
- [x] All features from v1.0.0
- [x] Azure Storage connectivity testing

**Purpose** — Validate:
- [x] Azure Storage access
- [x] Managed identities / credentials
- [x] Persistent cloud connectivity

### v3.0.0 ✅

**Features**
- [x] All features from v2.0.0
- [x] Azure Key Vault integration testing

**Purpose** — Validate:
- [x] Secret retrieval
- [x] Key Vault permissions
- [x] Secure configuration management

### v4.0.0 ✅

**Features**
- [x] All features from v3.0.0
- [x] Database connection testing

**Purpose** — Validate:
- [x] Database connectivity
- [x] Authentication
- [x] Network access
- [x] Application data layer readiness

---

## Use Cases

This project can be used for:
- Kubernetes environment validation
- Infrastructure smoke testing
- Azure integration testing
- CI/CD verification
- DevOps onboarding
- Cluster readiness checks

---

## Technologies

- .NET 10
- .NET Aspire
- Blazor Server
- Minimal API (REST)
- Azure Blob Storage
- Azure Key Vault
- SQL Database
- OpenTelemetry
- Docker
- Kubernetes (K8S)

---

## Project Structure

```text
.
├── Aspire.AppHost/         # Aspire orchestrator
├── Aspire.ServiceDefaults/ # Shared configuration (OTel, health checks)
├── Aspire.Web/             # Blazor Server frontend
└── Aspire.ApiService/      # REST API backend
```

---

## Future Improvements

Potential future extensions:
- Redis connectivity testing
- Message broker validation
- Service Bus integration
- Monitoring & observability checks
- Health dashboards
- Performance testing
- Automated chaos testing

---

## License

MIT License