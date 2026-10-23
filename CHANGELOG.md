# Changelog

Todas as mudanças notáveis deste projeto são documentadas neste arquivo.

O formato segue o [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/),
e o projeto adota [Versionamento Semântico](https://semver.org/lang/pt-BR/).

## [Unreleased]

### Added

- Etapa 0 — Fundação:
  - Esqueleto Clean Architecture: projetos `Common`, `Domain`, `Application`, `Infrastructure` e `Api`, com testes `Unit`, `Integration` e `Functional`.
  - Base CQRS: dispatcher explícito, handlers, validators (FluentValidation) e retorno com `Result` (FluentResults).
  - Cross-cutting: logging estruturado (Serilog), health checks `/health/live`, `/health/ready` e `/health`, autenticação JWT, Swagger com versionamento `/api/v1` e tratamento global de erros (RFC 7807).
  - Scaffold do PWA em `web/` (React + Vite).
  - Ambiente local com `docker-compose.yml` (API, web, SQL Server 2022, Azurite; RabbitMQ sob o profile `broker`) e Dockerfile multi-stage da API com usuário não-root.
  - CI no GitHub Actions: build com warnings como erro, testes Unit/Integration/Functional, gate de cobertura do `Domain`, lint/typecheck/build do front e build das imagens.
