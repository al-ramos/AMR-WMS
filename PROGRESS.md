# AMR-WMS — Progress Log

## Sprint 8 | 05/06/2026

### ✅ Scaffold completo — commit `68a02b3`

**Backend (.NET 10)**
- [x] AMR.WMS.slnx — solução com 6 projetos
- [x] Domain: Localizacao, MovimentacaoEstoque, enums, interfaces
- [x] Application: CriarLocalizacaoCommand + ListarLocalizacoesQuery (CQRS/MediatR 12)
- [x] Infrastructure: EF Core + SQLite, repositories, UoW, seed 30 localizações
- [x] API: porta 5188, Swagger, ExceptionHandlingMiddleware, ValidationBehavior, Serilog
- [x] Migration: InitialCreate gerada

**Testes**
- [x] 11/11 passando (xUnit + FluentAssertions + Moq)

**Frontend (React 19)**
- [x] Vite 6.0.5 + Bootstrap 5 + Lucide React
- [x] Porta 5177, proxy /api → http://localhost:5188
- [x] Sidebar: Dashboard, Localizações, Recebimento, Separação, Relatórios
- [x] DashboardPage placeholder "WMS em construção"

**Infra**
- [x] Dockerfile API (multi-stage .NET 10)
- [x] frontend/Dockerfile (multi-stage nginx)
- [x] docker-compose.yml (api :5188 + web :5177)
- [x] .github/workflows/ci.yml (build + test + workflow_dispatch)
- [x] CLAUDE.md com contexto completo

**Git**
- [x] Repositório criado: https://github.com/al-ramos/AMR-WMS
- [x] Push para main concluído
