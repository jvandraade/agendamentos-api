# Sistema de Agendamentos - Backend API

API REST para gerenciamento de agendamentos, construída com ASP.NET Core e Entity Framework Core.

## Sobre o Projeto

API RESTful que permite criar, listar e excluir agendamentos. Utiliza SQLite como banco de dados e possui documentação interativa via Swagger.

## Tecnologias Utilizadas

- ASP.NET Core 8.0
- Entity Framework Core
- PostgreSQL
- Swagger/OpenAPI

## Funcionalidades

- Criar novos agendamentos
- Listar todos os agendamentos
- Excluir agendamentos por ID
- Validação de dados
- Documentação interativa com Swagger
- CORS habilitado para integração com frontend

## Pré-requisitos

- .NET SDK 8.0 ou superior
- Visual Studio 2022 ou VS Code

## Instalação

Clone o repositório:

```
git clone https://github.com/jvandraade/agendamentos-api.git
cd agendamentos-api
```

Restaure as dependências:

```
dotnet restore
```

Execute as migrations para criar o banco de dados:

```
dotnet ef database update
```

Execute a aplicação:

```
dotnet run
```

A API estará disponível em: https://localhost:44300

## Endpoints da API

### GET /api/agendamentos

Lista todos os agendamentos cadastrados.

Resposta:
```json
[
  {
    "id": 1,
    "nome": "João Silva",
    "servico": "Consulta Médica",
    "data": "2024-11-20",
    "hora": "14:30"
  }
]
```

### POST /api/agendamentos

Cria um novo agendamento.

Body:
```json
{
  "nome": "João Silva",
  "servico": "Consulta Médica",
  "data": "2024-11-20",
  "hora": "14:30"
}
```

Resposta:
```json
{
  "id": 1,
  "nome": "João Silva",
  "servico": "Consulta Médica",
  "data": "2024-11-20",
  "hora": "14:30"
}
```

### DELETE /api/agendamentos/{id}

Exclui um agendamento pelo ID.

Resposta: 204 No Content

## Testando a API

### Com Swagger

Acesse https://localhost:44300/swagger no navegador após iniciar a aplicação.

O Swagger permite testar todos os endpoints diretamente na interface.

### Com Postman ou Insomnia

Importe a coleção de requisições ou configure manualmente:

Base URL: https://localhost:44300/api

Content-Type: application/json

### Com cURL

Listar agendamentos:
```
curl -X GET https://localhost:44300/api/agendamentos
```

Criar agendamento:
```
curl -X POST https://localhost:44300/api/agendamentos \
  -H "Content-Type: application/json" \
  -d '{"nome":"João Silva","servico":"Consulta","data":"2024-11-20","hora":"14:30"}'
```

Excluir agendamento:
```
curl -X DELETE https://localhost:44300/api/agendamentos/1
```

## Estrutura do Banco de Dados

Tabela: Agendamentos

- Id (int, primary key, auto increment)
- Nome (string, obrigatório)
- Servico (string, obrigatório)
- Data (string, obrigatório, formato: YYYY-MM-DD)
- Hora (string, obrigatório, formato: HH:mm)

Banco de dados PostgreSQL localizado em: agendamentos.db

## Configuração do banco de dados
- No arquivo appsetings.json, a linha nº 3 ( "DefaultConnection": "Server=localhost;Port=5432;Database=agendamentos;Username=postgres;Password=coloquesuasenha"), fique atento ao mudar o campo "Password", para colocar a mesma senha que você usa em seu banco de dados na máquina local!

## Configuração de CORS

A API está configurada para aceitar requisições de:
- http://localhost:3000 (frontend React em desenvolvimento)

Para adicionar outras origens, edite o arquivo Program.cs na seção de configuração CORS.

## Scripts Disponíveis

```
dotnet run              # Executa a aplicação
dotnet build            # Compila o projeto
dotnet test             # Executa testes
dotnet ef migrations add NomeMigration    # Cria nova migration
dotnet ef database update                  # Aplica migrations
```

## Configurações

As configurações da aplicação estão no arquivo appsettings.json:

- ConnectionStrings: String de conexão com o banco de dados
- Logging: Configurações de logs
- AllowedHosts: Hosts permitidos

## Build e Deploy

Para gerar o build de produção:

```
dotnet publish -c Release -o ./publish
```

Os arquivos compilados estarão na pasta publish/.

Para deploy em produção, configure a string de conexão do banco de dados e as origens permitidas no CORS conforme necessário.

## Autor

Desenvolvido por [Vitor Andrade]
