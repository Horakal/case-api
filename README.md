# Case API - Sistema de Gerenciamento de Tarefas

API REST para gerenciamento de tarefas da empresa Tarefas S.A. Permite criar, atualizar, buscar e deletar tarefas com autenticação JWT.

## Tecnologias

- .NET 9 / C#
- PostgreSQL 16
- Entity Framework Core
- JWT Authentication
- Docker / Docker Compose
- xUnit + FluentAssertions (84%+ cobertura)

## Arquitetura

O projeto segue Clean Architecture com 4 camadas:

```
RestApiCase.Api            → Controllers, Middleware
RestApiCase.Application    → Services, DTOs, Validators
RestApiCase.Domain         → Entidades, Interfaces, Enums
RestApiCase.Infrastructure → Repositórios, DbContext, Migrations
```

## Como rodar

### Com Docker (recomendado)

```bash
docker-compose up --build
```

A API ficará disponível em `http://localhost:8080` e o PostgreSQL em `localhost:5432`.

### Sem Docker

**Pré-requisitos:** .NET 9 SDK, PostgreSQL 16

1. Crie o banco `TarefasSA` no PostgreSQL
2. Ajuste a connection string em `RestApiCase.Api/appsettings.json` se necessário
3. Execute:

```bash
dotnet run --project RestApiCase.Api
```

A API ficará disponível em `http://localhost:5289` ou `https://localhost:7027`.

As migrations são aplicadas automaticamente na inicialização, junto com o seed de usuários.

### Usuários pré-cadastrados

| Usuário   | Senha     | Perfil     |
|-----------|-----------|------------|
| user1     | user1     | USER       |
| user2     | user2     | USER       |
| SuperUser | superUser | SUPER_USER |

## Rotas da API

### Autenticação

| Método | Rota                | Descrição                   | Auth |
|--------|---------------------|-----------------------------|------|
| POST   | `/api/Auth/login`   | Login, retorna JWT (5min)   | Não  |
| POST   | `/api/Auth/logout`  | Logout, revoga refresh token| Sim  |
| POST   | `/api/Auth/refresh` | Renova token via refresh    | Não  |

### Tarefas

| Método | Rota                              | Descrição                        | Auth |
|--------|-----------------------------------|----------------------------------|------|
| GET    | `/api/v1/TasksItems`              | Listar tarefas (pending primeiro)| Sim  |
| GET    | `/api/v1/TasksItems?status=Pending` | Filtrar por status             | Sim  |
| GET    | `/api/v1/TasksItems/{id}`         | Buscar tarefa por ID             | Sim  |
| POST   | `/api/v1/TasksItems`              | Criar tarefa                     | Sim  |
| PUT    | `/api/v1/TasksItems/{id}`         | Atualizar tarefa                 | Sim  |
| DELETE | `/api/v1/TasksItems/{id}`         | Deletar tarefa                   | Sim  |

### Operacionais

| Método | Rota           | Descrição                          | Auth |
|--------|----------------|------------------------------------|------|
| GET    | `/healthcheck` | Saúde da aplicação e do banco      | Não  |
| GET    | `/metrics`     | Indicadores de performance da API  | Não  |

## Exemplos de uso (CURL)

### Login

```bash
curl -X POST http://localhost:8080/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "user1", "password": "user1"}'
```

Resposta:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "d3f1a2b3-c4d5-..."
}
```

### Criar tarefa

```bash
curl -X POST http://localhost:8080/api/v1/TasksItems \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "title": "Minha tarefa",
    "description": "Descrição detalhada da tarefa",
    "summary": "Resumo breve",
    "dueDate": "2026-03-01T00:00:00"
  }'
```

### Listar todas as tarefas

```bash
curl http://localhost:8080/api/v1/TasksItems \
  -H "Authorization: Bearer {token}"
```

### Filtrar tarefas por status

```bash
curl "http://localhost:8080/api/v1/TasksItems?status=Pending" \
  -H "Authorization: Bearer {token}"
```

### Buscar tarefa por ID

```bash
curl http://localhost:8080/api/v1/TasksItems/{id} \
  -H "Authorization: Bearer {token}"
```

### Atualizar tarefa

```bash
curl -X PUT http://localhost:8080/api/v1/TasksItems/{id} \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "title": "Título atualizado",
    "description": "Nova descrição",
    "status": 2
  }'
```

Status: `0` = Pending, `2` = Completed

### Deletar tarefa

```bash
curl -X DELETE http://localhost:8080/api/v1/TasksItems/{id} \
  -H "Authorization: Bearer {token}"
```

### Refresh token

```bash
curl -X POST http://localhost:8080/api/Auth/refresh \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "d3f1a2b3-c4d5-...",
    "userId": "guid-do-usuario"
  }'
```

### Health check

```bash
curl http://localhost:8080/healthcheck
```

### Metrics

```bash
curl http://localhost:8080/metrics
```

Resposta:
```json
{
  "totalRequests": 150,
  "averageResponseTimeMs": 42.5,
  "totalErrors": 3,
  "errorRate": 2.0,
  "requestsByMethod": { "GET": 100, "POST": 30, "PUT": 15, "DELETE": 5 },
  "requestsByStatusCode": { "200": 140, "401": 5, "404": 2, "500": 3 },
  "uptimeSince": "2026-02-13T14:00:00"
}
```

## Collection Bruno

Uma collection Bruno está disponível na pasta `bruno/` com todas as requisições pré-configuradas. Para usar:

1. Abra o Bruno
2. Importe a collection da pasta `bruno/`
3. Selecione o environment `Local`
4. Execute `Login` primeiro (o token é salvo automaticamente nas variáveis)

## Testes

```bash
# Rodar testes
dotnet test

# Rodar com cobertura
dotnet test --collect:"XPlat Code Coverage" --settings coverage.runsettings
```


