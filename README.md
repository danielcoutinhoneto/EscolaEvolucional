# Escola Evolucional — API de Matrículas

API REST para controle de alunos, turmas e matrículas escolares, desenvolvida como parte do teste prático para a vaga de **Analista de Desenvolvimento .NET Pleno/Sênior da Evolucional**.

O objetivo do projeto é demonstrar a construção de uma API em .NET Framework com separação de responsabilidades, consultas SQL explícitas e tratamento transacional das matrículas.

> **Status atual:** a configuração inicial de acesso a dados com Dapper está em desenvolvimento. Os endpoints e os testes ainda serão implementados. As rotas documentadas abaixo representam o contrato previsto para a entrega.

## Stack do projeto

- .NET Framework 4.8;
- ASP.NET Web API 2;
- C#;
- SQL Server;
- Dapper com SQL escrito manualmente;
- Newtonsoft.Json.

## Funcionalidades

- CRUD de alunos com exclusão lógica;
- listagem paginada de alunos e filtro opcional por nome;
- consulta de turmas com o número de vagas restantes;
- matrícula de aluno em turma com controle transacional;
- relatório de alunos por turma gerado diretamente no SQL Server.

## Pré-requisitos

- Windows;
- Visual Studio com a carga de trabalho **Desenvolvimento para ASP.NET e Web**;
- .NET Framework 4.8 Developer Pack;
- SQL Server, SQL Server Express ou LocalDB;
- SQL Server Management Studio ou outra ferramenta para executar scripts SQL;
- Git.

## Como executar

### 1. Clone o repositório

```powershell
git clone https://github.com/danielcoutinhoneto/EscolaEvolucional.git
cd EscolaEvolucional
```

### 2. Crie o banco de dados

Execute no SQL Server o arquivo [`database/script-banco.sql`](database/script-banco.sql). Ele cria o banco `TesteEscola`, as tabelas `Aluno`, `Turma` e `Matricula` e também inclui dados para teste.

O arquivo recebido com o desafio foi versionado sem alterações estruturais nesta etapa. Foram avaliadas duas proteções adicionais para o banco:

- uma restrição `CHECK` para impedir vagas negativas ou maiores que o total;
- uma restrição ou índice único para impedir mais de uma matrícula do mesmo aluno na mesma turma.

Essas proteções ainda não foram aplicadas ao script fornecido. A decisão será reavaliada durante a implementação da matrícula transacional e, caso sejam adicionadas, a alteração será registrada nesta documentação.

### 3. Configure a conexão

A connection string real não é armazenada no `Web.config` nem versionada no repositório. O `Web.config` referencia o arquivo local `EscolaEvolucional.Api/ConnectionStrings.config`, que é ignorado pelo Git.

Crie o arquivo local copiando o modelo fornecido:

```powershell
Copy-Item .\EscolaEvolucional.Api\ConnectionStrings.config.example `
          .\EscolaEvolucional.Api\ConnectionStrings.config
```

Depois, abra `ConnectionStrings.config` e substitua o texto `Aqui é sua connection string` pela connection string do seu ambiente:

```xml
<connectionStrings>
  <add name="TesteEscola"
       connectionString="Aqui é sua connection string"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

O nome `TesteEscola` deve ser preservado, pois é utilizado por `SqlConnectionFactory`. Não remova a regra do `.gitignore` e nunca force a inclusão de `ConnectionStrings.config` em um commit. Apenas o arquivo `.example`, que contém o marcador sem credenciais, deve ser versionado.

### 4. Restaure os pacotes e inicie a aplicação

1. Abra `EscolaEvolucional.slnx` no Visual Studio.
2. Restaure os pacotes NuGet da solução.
3. Defina `EscolaEvolucional.Api` como projeto de inicialização.
4. Execute com `F5` ou `Ctrl+F5`.

O projeto está configurado para usar IIS Express no endereço:

```text
https://localhost:44360/
```

A porta pode ser diferente caso o Visual Studio gere outra configuração local.

## Endpoints

### Alunos

| Método | Rota | Descrição |
| --- | --- | --- |
| `GET` | `/api/alunos?page=1&pageSize=10&nome=Ana` | Lista os alunos com paginação e filtro opcional por nome |
| `GET` | `/api/alunos/{id}` | Consulta um aluno pelo identificador |
| `POST` | `/api/alunos` | Cadastra um aluno |
| `PUT` | `/api/alunos/{id}` | Atualiza um aluno |
| `DELETE` | `/api/alunos/{id}` | Desativa o aluno sem removê-lo do banco |

Exemplo de cadastro:

```http
POST /api/alunos HTTP/1.1
Host: localhost:44360
Content-Type: application/json

{
  "nome": "Maria da Silva",
  "email": "maria.silva@email.com",
  "dataNascimento": "2006-05-20"
}
```

Exemplo de resposta paginada:

```json
{
  "items": [
    {
      "id": 1,
      "nome": "Ana Souza",
      "email": "ana.souza@email.com",
      "dataNascimento": "2006-03-14",
      "ativo": true
    }
  ],
  "page": 1,
  "pageSize": 10,
  "total": 8
}
```

### Turmas

| Método | Rota | Descrição |
| --- | --- | --- |
| `GET` | `/api/turmas` | Lista as turmas com a quantidade de vagas restantes |

### Matrículas

| Método | Rota | Descrição |
| --- | --- | --- |
| `POST` | `/api/matriculas` | Matricula um aluno em uma turma |

Exemplo:

```http
POST /api/matriculas HTTP/1.1
Host: localhost:44360
Content-Type: application/json

{
  "alunoId": 1,
  "turmaId": 2
}
```

A matrícula deve respeitar as seguintes regras:

- aluno e turma devem existir;
- o aluno deve estar ativo;
- a turma deve possuir vaga disponível;
- o aluno não pode estar matriculado duas vezes na mesma turma;
- a matrícula e o decremento da vaga devem ocorrer na mesma transação.

### Relatórios

| Método | Rota | Descrição |
| --- | --- | --- |
| `GET` | `/api/relatorios/alunos-por-turma` | Retorna nome da turma, quantidade de alunos e vagas restantes |

O agrupamento do relatório será realizado diretamente no SQL Server utilizando `JOIN` e `GROUP BY`.

## Status HTTP

| Status | Situação |
| --- | --- |
| `200 OK` | Consulta ou atualização realizada com sucesso |
| `201 Created` | Recurso criado com sucesso |
| `400 Bad Request` | Parâmetros ou corpo da requisição inválidos |
| `404 Not Found` | Aluno ou turma não encontrado |
| `409 Conflict` | Aluno inativo, turma sem vaga ou matrícula duplicada |
| `500 Internal Server Error` | Erro inesperado da aplicação ou infraestrutura |

## Organização do código

```text
EscolaEvolucional.Api/
|-- App_Start/       # Configuração da Web API
|-- Controllers/     # Entrada HTTP e respostas
|-- Contracts/       # Objetos de requisição e resposta
|-- Models/          # Modelos do domínio
|-- Services/        # Regras de negócio e casos de uso
|-- Repositories/    # Acesso ao SQL Server com Dapper
|-- Infrastructure/  # Conexões e componentes externos
`-- Web.config       # Configurações da aplicação
```

Os controllers não devem conter regras de negócio. Eles recebem a requisição e convertem o resultado do serviço em uma resposta HTTP. Os serviços coordenam as regras e os repositórios concentram o SQL e o acesso ao banco.

## Testes

Os testes automatizados ainda serão adicionados. A prioridade será cobrir os seguintes cenários da matrícula:

- matrícula realizada com sucesso;
- aluno inexistente ou inativo;
- turma inexistente ou sem vagas;
- matrícula duplicada;
- falha na operação e garantia de rollback.

## Licença

Consulte o arquivo [LICENSE.txt](LICENSE.txt).
