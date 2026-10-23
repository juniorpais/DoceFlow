# DoceFlow

Sistema omnicanal para doceria: doces prontos e sob encomenda, em um único PWA (web, mobile e desktop) com back-end .NET 8.

- Especificação de negócio e arquitetura: [`docs/`](docs/README.md)
- Checklist de desenvolvimento do MVP: [`docs/09-checklist-desenvolvimento-mvp.md`](docs/09-checklist-desenvolvimento-mvp.md)
- Status atual: página "em construção" (`index.html`) servida por nginx via `Dockerfile`.

## Deploy

- Produção: https://doceflow-production.up.railway.app (Railway, deploy automático a cada push em `main`).

## Desenvolvimento local

### Pré-requisitos

- .NET 8 SDK
- Node 20
- Docker (com Docker Compose v2)

### Infraestrutura (banco e storage)

```bash
docker compose up -d db azurite
```

### API (rodando na máquina)

```bash
dotnet run --project src/Api
```

### Web (PWA, modo dev)

```bash
cd web && npm install && npm run dev
```

### Tudo em containers

```bash
docker compose up --build
```

Para incluir o RabbitMQ (profile `broker`):

```bash
docker compose --profile broker up -d
```

### Testes

```bash
dotnet test tests/Unit
dotnet test tests/Integration   # requer SQL Server em localhost:1433 (docker compose up -d db)
dotnet test tests/Functional
```

### URLs

| Serviço          | URL                                   |
| ---------------- | ------------------------------------- |
| API              | http://localhost:8080                 |
| Swagger          | http://localhost:8080/swagger/index.html |
| Health (ready)   | http://localhost:8080/health/ready    |
| Web (PWA)        | http://localhost:5173                 |
| RabbitMQ (admin) | http://localhost:15672 (profile `broker`) |

Requests prontas para a API estão em [`docs/api.http`](docs/api.http).
