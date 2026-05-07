# Testing Infrastructure

A lightweight testing platform for validating Kubernetes (K8S) infrastructure integrations and cloud connectivity.

This repository contains:
- **Web application**
- **API service**

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
[ Web Application ]
          |
          v
[ API Service ]
          |
          +--> Azure Storage
          |
          +--> Azure Key Vault
          |
          +--> Database
```

---

## Version Roadmap

### v1.0.0

**Features**
- Web application deployment
- API deployment
- Web ↔ API connection testing

**Purpose** — Validate:
- Kubernetes deployment
- Service discovery
- Internal networking
- Ingress / basic communication

### v2.0.0

**Features**
- All features from v1.0.0
- Azure Storage connectivity testing

**Purpose** — Validate:
- Azure Storage access
- Managed identities / credentials
- Persistent cloud connectivity

### v3.0.0

**Features**
- All features from v2.0.0
- Azure Key Vault integration testing

**Purpose** — Validate:
- Secret retrieval
- Key Vault permissions
- Secure configuration management

### v4.0.0

**Features**
- All features from v3.0.0
- Database connection testing

**Purpose** — Validate:
- Database connectivity
- Authentication
- Network access
- Application data layer readiness

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

- Kubernetes (K8S)
- Web Application
- REST API
- Azure Storage
- Azure Key Vault
- Database Connectivity
- Docker
- CI/CD Pipelines

---

## Project Structure

```text
.
├── web/          # Frontend application
├── api/          # Backend API service
├── k8s/          # Kubernetes manifests
├── docker/       # Docker configurations
└── docs/         # Documentation
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