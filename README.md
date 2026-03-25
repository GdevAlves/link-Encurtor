# URL Shortener

API de encurtamento de links com arquitetura em camadas (Clean Architecture + CQRS), autenticação JWT e analytics com agente de IA.

## Principais funcionalidades

- Criar URLs encurtadas (com alias customizado ou gerado automaticamente)
- Redirecionar do link curto para o link original
- Consultar detalhes e listagem paginada das URLs do usuario
- Autenticacao e autorizacao com JWT
- Coleta de acessos e analytics com agente Gemini
- Sessao de conversa do analytics persistida no Redis

## Estrutura da solucao

- `UrlShortener.Api`: camada de apresentacao (controllers, middleware, DI)
- `UrlShortener.Application`: casos de uso (commands/queries/handlers)
- `UrlShortener.Domain`: entidades, value objects, contratos e regras
- `UrlShortener.Infra`: persistencia EF Core, repositorios e servicos externos

## Requisitos

- .NET SDK 10.0+
- Docker e Docker Compose (plugin `docker compose`)

## Configuracao

A API le configuracoes por `appsettings` e variaveis de ambiente.

Pontos importantes:

- `ConnectionStrings:DefaultConnection` (MySQL)
- `Redis:ConnectionString` e `Redis:SessionTtlMinutes`
- `Gemini:ApiKey` (User Secrets ou variavel de ambiente)
- `JwtSettings:*`

Exemplo para definir a chave Gemini em desenvolvimento:

```bash
dotnet user-secrets --project UrlShortener.Api set "Gemini:ApiKey" "SUA_CHAVE"
```

## Executando com Docker Compose (recomendado)

Este modo sobe MySQL + Redis + imagens da solucao definidas no `compose.yaml`.

```bash
docker compose -f /home/gabriel/dev/link-Encurtor/compose.yaml up -d
```

Verificar status dos servicos:

```bash
docker compose -f /home/gabriel/dev/link-Encurtor/compose.yaml ps
```

Ver logs da API:

```bash
docker compose -f /home/gabriel/dev/link-Encurtor/compose.yaml logs -f urlshortener.api
```

## Executando API localmente (com infraestrutura em containers)

Suba apenas MySQL e Redis:

```bash
docker compose -f /home/gabriel/dev/link-Encurtor/compose.yaml up -d mysql redis
```

Depois execute a API:

```bash
dotnet run --project /home/gabriel/dev/link-Encurtor/UrlShortener.Api
```

## Build da solucao

```bash
dotnet build /home/gabriel/dev/link-Encurtor/UrlShortener.sln
```

## Documentacao da API

Com a API em execucao, use:

- Swagger UI (ambiente de desenvolvimento)
- Scalar API Reference (mapeada na aplicacao)

## Observacoes

- Excecoes inesperadas sao tratadas centralmente por `GlobalExceptionHandler` na API.
- O estado da conversa do agente de analytics e armazenado no Redis por usuario + `conversationId`.
