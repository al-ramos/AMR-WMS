# PROGRESS — Sprint 8 — Dashboard WMS

**Data:** 2026-06-05  
**Branch:** `claude/new-session-D5smy`

## Status: ✅ Concluído

## Implementado

### Application (CQRS)
- `IDashboardRepository` interface + DTOs em `Application/Dashboard/`
- `GetDashboardWmsKpisQuery` (KPIs com filtro por dias)
- `GetOcupacaoPorZonaQuery`
- `GetTopProdutosQuery` (filtro por dias)
- `GetAlertasQuery`

### Infrastructure
- `DashboardRepository` com EF Core direto (agregações por zona, top produtos, alertas)
- DI registrado em `InfrastructureServiceExtensions`

### API
- `DashboardController`:
  - GET /api/dashboard/wms?dias=1
  - GET /api/dashboard/wms/ocupacao
  - GET /api/dashboard/wms/top-produtos?dias=30
  - GET /api/dashboard/wms/alertas

### Frontend
- `DashboardPage.tsx` completo:
  - Filtro período: Hoje / 7 dias / 30 dias
  - 4 KPI cards: Total Localizações, Ocupação Média, Ordens Separação, Ordens Recebimento
  - Gráfico de barras (Recharts) — ocupação % por zona, cores dinâmicas
  - Tabela de alertas (>90% ocupação)
  - Top 10 produtos movimentados
- `recharts` instalado
