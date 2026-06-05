# PROGRESS — Sprint 8 — Localizações CRUD

**Data:** 2026-06-05  
**Branch:** `claude/new-session-D5smy`

## Status: ✅ Concluído

## O que foi implementado

### Backend (API)
- `GET /api/localizacoes` — lista todas as localizações (existente)
- `POST /api/localizacoes` — cria nova localização (existente)
- `PUT /api/localizacoes/{id}` — atualiza capacidade e tipo ✅ novo
- `DELETE /api/localizacoes/{id}` — remove localização sem estoque ✅ novo
- `GET /api/localizacoes/disponiveis` — localizações com espaço livre ✅ novo
- `GET /api/localizacoes/zona/{zona}` — localizações por zona ✅ novo

### Application (CQRS)
- `AtualizarLocalizacaoCommand` + Validator + Handler ✅
- `RemoverLocalizacaoCommand` + Handler ✅
- `ObterDisponiveisQuery` + Handler ✅
- `ListarPorZonaQuery` + Handler ✅

### Domain / Infrastructure
- `ILocalizacaoRepository` — adicionados `ListarDisponiveisAsync` e `ListarPorZonaAsync` ✅
- `LocalizacaoRepository` — implementações dos novos métodos ✅

### Frontend
- `LocalizacoesPage.tsx` — implementação completa ✅
  - Tabela por zona com agrupamento dinâmico
  - Barra de ocupação colorida (verde <70% / amarelo 70-90% / vermelho >90%)
  - Modal criar/editar
  - Filtros por Zona e TipoLocalizacao
  - Botão de remoção desabilitado quando há estoque
