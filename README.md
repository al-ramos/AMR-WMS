# AMR-WMS

Módulo de gestão de armazém do AMR SYSTEM — endereçamento, localizações, separação
(picking) e dashboard operacional.

**Stack:** .NET 10 + React 19 + SQLite | **Deploy:** AWS ECS Fargate + ECR + ALB (porta 8084)

> **Estado real (03/09/2026):** o módulo compila, os 11 testes passam e o frontend
> constrói. **Não está em produção** — não há cluster ECS provisionado na conta
> (`amr-system` não existe), e o workflow de deploy termina em verde registrando
> que o deploy foi ignorado por falta de infraestrutura. Ver
> [Catálogo Verificado](https://github.com/al-ramos/AMR/blob/main/docs/CATALOGO-VERIFICADO.md).

---

## Dev local

```bash
# Backend
cd src/AMR.WMS.API && dotnet run
# → http://localhost:5188/swagger

# Frontend
cd frontend && npm install && npm run dev
# → http://localhost:5177

# Ambos via Docker Compose
docker compose up --build
# API → http://localhost:5188   Web → http://localhost:5177

# Testes
dotnet test
```

---

## Estrutura

| Projeto | Papel |
|---|---|
| `AMR.WMS.Domain` | Entidades e interfaces de repositório |
| `AMR.WMS.Application` | Casos de uso (separação, dashboard) |
| `AMR.WMS.Infrastructure` | EF Core/SQLite, repositórios, cliente HTTP do Core |
| `AMR.WMS.API` | Controllers, CORS, health checks |
| `AMR.WMS.Shared` | Contratos compartilhados |

Controllers: `Dashboard`, `Localizacoes`, `Separacao`.

---

## Configuração

Nada de ambiente fica fixado no código. Em produção estes valores **têm** de vir
por variável de ambiente — a API falha no boot, com mensagem nomeando a chave,
se `AmrCore:BaseUrl` estiver ausente fora de Development.

| Variável | Para quê |
|---|---|
| `ConnectionStrings__AmrWms` | Caminho do arquivo SQLite (default `/app/data/amr_wms.db`) |
| `Cors__AllowedOrigins` | Origem do frontend. Vazio = nenhuma origem liberada |
| `AmrCore__BaseUrl` | URL do AMR-Core. Obrigatória fora de Development |
| `ASPNETCORE_ENVIRONMENT` | `Development` habilita os defaults de localhost |

Os valores de desenvolvimento estão em `src/AMR.WMS.API/appsettings.Development.json`.

---

## Health checks

| Rota | Responde |
|---|---|
| `GET /health` | Liveness — o processo subiu. Não toca no banco |
| `GET /health/ready` | Readiness — verifica conexão com o banco; 503 se indisponível |

O container web expõe `GET /health` no nginx, sem depender da API.

---

## Entrega

- `ci.yml` — build + testes do backend e build do frontend, em `main`, `develop`, `feat/**` e `fix/**`
- `deploy-aws.yml` — build/push das imagens no ECR e `force-new-deployment` no ECS.
  Confere os segredos e a existência do cluster antes; sem infraestrutura, encerra
  em verde declarando que nada foi publicado.

Secrets necessários: `AWS_ACCOUNT_ID`, `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`.

---

## Pendências conhecidas

- **Sem autenticação.** A API não tem `AddAuthentication` nem `[Authorize]`. Não
  deve ser exposta em ALB público antes disso.
- **Sem infraestrutura provisionada.** Não existe IaC para o cluster `amr-system`
  que o workflow tem como alvo, e não há repositórios ECR `amr-wms/api|web`.
- **Integração com o Core é HTTP síncrono sem resiliência** — sem retry nem
  circuit breaker (o Core já usa Polly; o WMS ainda não).
