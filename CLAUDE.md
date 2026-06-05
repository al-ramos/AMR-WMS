# AMR-WMS — Contexto para Claude Code

## Identidade do módulo
Módulo WMS (Warehouse Management System) do AMR SYSTEM — gestão de localizações, recebimento, separação e movimentação de estoque no armazém.
- **API local**: http://localhost:5188/swagger
- **Web local**: http://localhost:5177
- **Banco**: SQLite via EF Core 10 (EFS `/data` em produção)

## Ecossistema AMR SYSTEM
- **AMR-Financeiro** — SQL Server, porta API :5015, web :5173
- **AMR-Core** — SQLite, porta API :5001, web :5175
- **AMR-Fábrica** — SQLite, porta API :5186, web :5174
- **AMR-CRM** — SQLite, porta API :5187, web :5176
- **AMR-WMS** (este repo) — SQLite, porta API :5188, web :5177

## Stack
- Backend: .NET 10 + Clean Architecture + CQRS (MediatR 12+)
- ORM: EF Core + SQLite + Migrations
- Frontend: React 19 + TypeScript + Vite 6.0.5 + Bootstrap 5 + Lucide React
- Testes: xUnit + Moq + FluentAssertions + Coverlet
- Infra: AWS ECS Fargate + ECR + ALB + EFS | CI/CD: GitHub Actions

## Arquitetura
```
src/
├── AMR.WMS.Domain/          # Entidades, enums, interfaces
├── AMR.WMS.Application/     # CQRS handlers, DTOs, queries, commands, validators
├── AMR.WMS.Infrastructure/  # EF Core, SQLite, repositories, UoW, migrations
├── AMR.WMS.Shared/          # Result<T>
└── AMR.WMS.API/             # Controllers, Middleware, Program.cs
frontend/                     # React 19 + Bootstrap 5
tests/
└── AMR.WMS.Tests/           # xUnit + Moq + FluentAssertions (domínio + application)
```

Padrões: Clean Architecture, CQRS+MediatR, Repository Pattern, Unit of Work, DI, ValidationBehavior.

## Entidades do Domínio
- `Localizacao` — endereço físico no armazém (Zona/Corredor/Prateleira/Posição), capacidade e ocupação
- `MovimentacaoEstoque` — registro de entrada, saída ou transferência de produto em uma localização

## Enums
- `TipoLocalizacao`: Picking, Reserva, Expedicao, Bloqueado
- `TipoMovimentacao`: Entrada, Saida, Transferencia

## Comandos Principais
```bash
# Backend
cd src/AMR.WMS.API && dotnet run
# → http://localhost:5188/swagger

# Frontend
cd frontend && npm install && npm run dev
# → http://localhost:5177

# Testes
dotnet test

# Migrations
dotnet ef migrations add <Nome> --project src/AMR.WMS.Infrastructure --startup-project src/AMR.WMS.API
dotnet ef database update --project src/AMR.WMS.Infrastructure --startup-project src/AMR.WMS.API
```

## Estado do Projeto — Sprint 8 (05/06/2026)

### ✅ Entregues
| Card | Commit | Data |
|---|---|---|
| 🏗️ Scaffold AMR-WMS (.NET 10 + React 19) | inicial | 05/06/2026 |

### 🔲 Backlog Sprint 8
- CRUD de Localizações (lista, criação, edição, inativação)
- Fluxo de Recebimento (entrada de mercadoria + movimentação)
- Fluxo de Separação / Picking
- Dashboard WMS — ocupação por zona, movimentações recentes
- Integração RabbitMQ/MassTransit com AMR-Core (sync produtos)

## Seed de Dados Demo
- 30 localizações distribuídas em 3 zonas:
  - **Zona A** (10 loc.) — Picking, capacidade 50
  - **Zona B** (10 loc.) — Reserva, capacidade 100
  - **Zona C** (10 loc.) — Expedição, capacidade 200

## Troubleshooting
| Problema | Solução |
|---|---|
| Porta errada no backend | `launchSettings.json` → `applicationUrl: http://localhost:5188` |
| CORS bloqueando frontend | `appsettings.Development.json` → `AllowedOrigins: http://localhost:5177` |
| MediatR não resolve handlers | Verificar `RegisterServicesFromAssembly` no `Program.cs` |
| Vite proxy não funciona | `vite.config.ts` → `target: http://localhost:5188` |
