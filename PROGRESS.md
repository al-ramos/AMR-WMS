# PROGRESS — Sprint 8 — Separação (Picking)

**Data:** 2026-06-05  
**Branch:** `claude/new-session-D5smy`

## Status: ✅ Concluído

## Implementado

### Domain
- `StatusSeparacao` enum (Aberta/EmSeparacao/Concluida)
- `OrdemSeparacao` entity com factory `Criar()`, `IniciarSeparacao()`, `Concluir()`
- `ItemSeparacao` entity com factory `Criar()`, `Separar(localizacaoId, quantidade)`
- `IOrdemSeparacaoRepository` interface

### Application (CQRS)
- `CriarOrdemSeparacaoCommand` + Validator + Handler
- `SepararItemCommand` + Validator + Handler (inicia ordem se Aberta, atualiza ocupação da localização)
- `ConcluirSeparacaoCommand` + Validator + Handler
- `ListarOrdensSeparacaoQuery` + Handler + DTOs
- `GetPVsAprovadosQuery` + Handler (via IAmrCoreService)
- `IAmrCoreService` interface

### Infrastructure
- `AmrCoreService` (HttpClient → Core :5001)
- `OrdemSeparacaoRepository` com Include de Itens
- `OrdemSeparacaoConfiguration` + `ItemSeparacaoConfiguration`
- Migration `20260605200000_AddSeparacao` (tabelas OrdensSeparacao + ItensSeparacao)
- Snapshot atualizado
- Seed demo: 3 ordens (Aberta, EmSeparacao, Concluida)
- DI: IOrdemSeparacaoRepository, IAmrCoreService + HttpClient

### API
- `SeparacaoController`:
  - GET /api/separacao
  - GET /api/separacao/pv-aprovados
  - POST /api/separacao/criar
  - PUT /api/separacao/{id}/separar-item
  - PUT /api/separacao/{id}/concluir
- `appsettings.json`: AmrCore.BaseUrl configurado

### Frontend
- `SeparacaoPage.tsx` completa:
  - Lista ordens por status (Em Separação → Abertas → Concluídas)
  - Modal Nova Ordem: busca PVs aprovados no Core, fallback mock
  - Modal Separar Item: localização FIFO (menor ocupação primeiro), quantidade editável
  - Botão Concluir por ordem
