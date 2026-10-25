# DevFlow

API REST para gestão de projetos e tarefas, com registro de horas trabalhadas. Feita em C# / .NET 10 seguindo Clean Architecture.

## Tecnologias

- .NET 10 / ASP.NET Core
- Entity Framework Core + PostgreSQL 17
- Docker Compose

## Arquitetura

```
src/
├── DevFlow.Domain          # Entidades e regras de negócio (não depende de ninguém)
├── DevFlow.Application     # Casos de uso (services) e contratos (interfaces)
├── DevFlow.Infrastructure  # Banco de dados: EF Core, repositórios, migrations
└── DevFlow.API             # Controllers e tratamento global de erros
```

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

## Como rodar

**1. Clone o repositório**

```bash
git clone https://github.com/marcelloabritta/DevFlow.git
cd DevFlow
```

**2. Configure as variáveis do banco**

Copie o `.env.example` para `.env`:

```bash
cp .env.example .env
```

Preencha as três variáveis (exemplo: usuário `devflow`, banco `devflow` e uma senha à sua escolha):

```env
POSTGRES_USER=devflow
POSTGRES_PASSWORD=sua_senha
POSTGRES_DB=devflow
```

**3. Suba o banco (PostgreSQL no Docker)**

```bash
docker compose up -d
```

O banco fica disponível na porta **5433**.

**4. Configure a connection string (User Secrets)**

Use os mesmos valores que você colocou no `.env`:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5433;Database=devflow;Username=devflow;Password=SUA_SENHA" --project src/DevFlow.API
```

**5. Crie as tabelas (migrations)**

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update --project src/DevFlow.Infrastructure --startup-project src/DevFlow.API
```

**6. Rode a API**

```bash
dotnet run --project src/DevFlow.API
```

A API sobe em `http://localhost:5120`.

## Endpoints

| Método | Rota                                               | Descrição                                           | Sucesso |
| ------ | -------------------------------------------------- | --------------------------------------------------- | ------- |
| POST   | `/api/projects`                                    | Cria um projeto                                     | 201     |
| GET    | `/api/projects?page=1&pageSize=10`                 | Lista projetos (paginado, máx. 50)                  | 200     |
| GET    | `/api/projects/{id}`                               | Busca um projeto pelo id                            | 200     |
| PUT    | `/api/projects/{id}`                               | Edita nome e descrição                              | 204     |
| DELETE | `/api/projects/{id}`                               | Exclui um projeto (soft delete)                     | 204     |
| POST   | `/api/taskitems`                                   | Cria uma tarefa                                     | 201     |
| GET    | `/api/taskitems?projectId={id}&page=1&pageSize=10` | Lista tarefas do projeto, com responsável (máx. 50) | 200     |
| GET    | `/api/taskitems/{id}`                              | Busca uma tarefa pelo id                            | 200     |
| PUT    | `/api/taskitems/{id}`                              | Edita título, descrição, prioridade, horas e prazo  | 204     |
| DELETE | `/api/taskitems/{id}`                              | Exclui uma tarefa (soft delete)                     | 204     |
| POST   | `/api/taskitems/{id}/logs`                         | Registra horas trabalhadas na tarefa                | 204     |
| PATCH  | `/api/taskitems/{id}/status`                       | Muda o status da tarefa                             | 204     |

Erros seguem o padrão: **400** para dados inválidos e **404** para recurso inexistente.
