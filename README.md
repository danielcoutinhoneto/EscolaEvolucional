# Escola Evolucional — API de Matrículas

API REST para controle de alunos, turmas e matrículas escolares, desenvolvida como parte do teste prático para a vaga de **Analista de Desenvolvimento .NET Pleno/Sênior da Evolucional**.

O projeto utiliza .NET Framework 4.8, ASP.NET Web API 2, SQL Server e Dapper com SQL escrito manualmente. A organização em camadas mantém o tratamento HTTP no controller, as regras no service e o acesso ao banco no repository.

> **Status atual:** os requisitos obrigatórios estão implementados: CRUD de alunos, turmas, relatório SQL, matrícula transacional e testes unitários das regras de matrícula. O cache de turmas foi implementado como bônus; a tela de consulta de alunos está implementada e em verificação final.

## Stack

- .NET Framework 4.8;
- ASP.NET Web API 2;
- C#;
- SQL Server;
- Dapper;
- Newtonsoft.Json.

## Funcionalidades

### Implementado

- cadastro, consulta, atualização e exclusão lógica de alunos;
- listagem paginada com total de registros;
- filtro opcional pelo nome do aluno;
- validação dos dados de entrada;
- listagem de turmas com vagas totais e disponíveis;
- relatório de alunos por turma agregado diretamente no SQL Server;
- inclusão de turmas sem matrícula no relatório;
- respostas JSON em camelCase;
- matrícula transacional com validações, rollback e proteção contra concorrência;
- testes unitários das regras e do mapeamento HTTP de matrícula;
- respostas HTTP 200, 201, 400, 404 e 409 conforme o cenário;
- tela responsiva de consulta de alunos, com filtro e paginação.

## Pré-requisitos

- Windows;
- Visual Studio com a carga de trabalho **Desenvolvimento para ASP.NET e Web**;
- .NET Framework 4.8 Developer Pack;
- SQL Server, SQL Server Express ou LocalDB;
- SQL Server Management Studio ou outra ferramenta para executar scripts SQL;
- Git.

## Como executar

### 1. Clonar o repositório

~~~powershell
git clone https://github.com/danielcoutinhoneto/EscolaEvolucional.git
cd EscolaEvolucional
~~~

### 2. Criar o banco de dados

Execute no SQL Server o arquivo [database/script-banco.sql](database/script-banco.sql).

O script cria o banco TesteEscola, as tabelas Aluno, Turma e Matricula e os dados iniciais utilizados nos testes.

O arquivo recebido no desafio foi versionado com duas proteções adicionais de integridade:

- `CK_Turma_VagasDisponiveis`: restrição `CHECK` que impede `VagasDisponiveis` de ficar negativa ou maior que `VagasTotal`;
- `UQ_Matricula_AlunoId_TurmaId`: restrição `UNIQUE` composta que impede a matrícula repetida do mesmo aluno na mesma turma.

Essas restrições protegem o banco mesmo quando uma gravação não passa pela API ou quando duas requisições concorrentes tentam alterar os mesmos dados. As validações da aplicação continuam necessárias para devolver mensagens e status HTTP adequados.

> Atenção: o script remove e recria as tabelas quando é executado. Dados locais existentes nessas tabelas serão perdidos.

### 3. Configurar a conexão local

A connection string real não fica no Web.config e não deve ser enviada ao Git. O Web.config referencia o arquivo local EscolaEvolucional.Api/ConnectionStrings.config, que está ignorado no .gitignore.

Copie o modelo:

~~~powershell
Copy-Item ./EscolaEvolucional.Api/ConnectionStrings.config.example ./EscolaEvolucional.Api/ConnectionStrings.config
~~~

Depois, abra ConnectionStrings.config e substitua somente o marcador pela conexão do seu ambiente:

~~~xml
<connectionStrings>
  <add name="TesteEscola"
       connectionString="Aqui é sua connection string"
       providerName="System.Data.SqlClient" />
</connectionStrings>
~~~

O nome TesteEscola deve ser preservado porque SqlConnectionFactory procura a configuração por esse nome. Apenas ConnectionStrings.config.example deve ser versionado.

### 4. Restaurar e executar

1. Abra EscolaEvolucional.slnx no Visual Studio.
2. Restaure os pacotes NuGet da solução.
3. Defina EscolaEvolucional.Api como projeto de inicialização.
4. Execute com F5 ou Ctrl+F5.

A configuração do projeto usa IIS Express em:

~~~text
https://localhost:44360/
~~~

O Visual Studio pode definir outra porta em uma configuração local.

## API de alunos

| Método | Rota | Resultado de sucesso |
| --- | --- | --- |
| GET | /api/alunos?page=1&pageSize=10&nome=Ana | Lista paginada — 200 |
| GET | /api/alunos/{id} | Aluno encontrado — 200 |
| POST | /api/alunos | Aluno criado — 201 |
| PUT | /api/alunos/{id} | Aluno atualizado — 200 |
| DELETE | /api/alunos/{id} | Aluno desativado — 200 |

### Paginação e filtro

- page é opcional, começa em 1 e possui valor padrão 1;
- pageSize é opcional, possui valor padrão 10 e aceita valores entre 1 e 100;
- nome é opcional e busca uma parte do nome;
- total informa quantos registros atendem ao filtro, independentemente da página;
- a ordenação por Id torna a paginação determinística.

Exemplo:

~~~http
GET /api/alunos?page=1&pageSize=2&nome=Ana HTTP/1.1
Host: localhost:44360
~~~

~~~json
{
  "items": [
    {
      "id": 1,
      "nome": "Ana Souza",
      "email": "ana.souza@email.com",
      "dataNascimento": "2006-03-14T00:00:00",
      "ativo": true,
      "dataCadastro": "2026-09-03T10:00:00"
    }
  ],
  "page": 1,
  "pageSize": 2,
  "total": 1
}
~~~

### Cadastrar aluno

~~~http
POST /api/alunos HTTP/1.1
Host: localhost:44360
Content-Type: application/json

{
  "nome": "Maria da Silva",
  "email": "maria.silva@email.com",
  "dataNascimento": "2006-05-20"
}
~~~

Nome, e-mail e data de nascimento são obrigatórios. Nome e e-mail aceitam no máximo 120 caracteres, o e-mail precisa ter formato válido e a data de nascimento não pode estar no futuro.

No sucesso, a API responde 201 Created, retorna o aluno criado e informa a rota de consulta no cabeçalho Location.

### Atualizar aluno

~~~http
PUT /api/alunos/1 HTTP/1.1
Host: localhost:44360
Content-Type: application/json

{
  "nome": "Ana Souza Atualizada",
  "email": "ana.atualizada@email.com",
  "dataNascimento": "2006-03-14"
}
~~~

A atualização altera somente alunos ativos. Um identificador inexistente ou pertencente a aluno inativo retorna 404.

### Excluir aluno

~~~http
DELETE /api/alunos/1 HTTP/1.1
Host: localhost:44360
~~~

A exclusão é lógica: a API executa UPDATE e altera Ativo para false. O registro não é apagado do banco.

A listagem e a consulta por identificador mantêm os alunos inativos visíveis e informam ativo: false. Isso permite conferir a exclusão lógica. Uma segunda tentativa de excluir o mesmo aluno retorna 404.

### Formato de erro

Erros de validação são respondidos com 400, sem transformar um erro esperado em 500:

~~~json
{
  "message": "A requisição contém dados inválidos.",
  "errors": {
    "alunoCreateDto.Email": [
      "O e-mail informado é inválido."
    ]
  }
}
~~~

Quando não há erros associados a campos, errors pode ser null.

## API de turmas

| Método | Rota | Descrição |
| --- | --- | --- |
| GET | /api/turmas | Lista turmas com vagas totais e disponíveis — 200 |

O endpoint não utiliza paginação porque ela não foi solicitada para turmas. Quando não houver registros, a resposta é 200 OK com uma coleção vazia.

Exemplo:

~~~json
[
  {
    "id": 1,
    "nome": "3A - Ensino Medio",
    "periodo": "Manha",
    "vagasTotal": 30,
    "vagasDisponiveis": 28
  }
]
~~~

## API de relatórios

| Método | Rota | Descrição |
| --- | --- | --- |
| GET | /api/relatorios/alunos-por-turma | Retorna turma, quantidade de alunos e vagas restantes — 200 |

Exemplo:

~~~json
[
  {
    "turmaId": 1,
    "turmaNome": "3A - Ensino Medio",
    "quantidadeAlunos": 2,
    "vagasRestantes": 28
  },
  {
    "turmaId": 2,
    "turmaNome": "3B - Ensino Medio",
    "quantidadeAlunos": 0,
    "vagasRestantes": 30
  }
]
~~~

O relatório é agregado pelo SQL Server com LEFT JOIN, COUNT(m.Id) e GROUP BY. Dessa forma, turmas sem matrícula também aparecem com quantidade igual a zero, sem agrupamento em memória no C#.

## Matrículas

| Método | Rota | Descrição |
| --- | --- | --- |
| POST | /api/matriculas | Matricula um aluno em uma turma |

### Criar matrícula

~~~http
POST /api/matriculas
Content-Type: application/json

{
  "alunoId": 1,
  "turmaId": 2
}
~~~

Em caso de sucesso, a API retorna `201 Created`:

~~~json
{
  "id": 9,
  "alunoId": 1,
  "turmaId": 2,
  "dataMatricula": "2026-09-05T09:19:51"
}
~~~

A operação valida aluno existente e ativo, turma existente, vaga disponível e matrícula duplicada. O Repository abre uma única conexão, inicia uma transação serializável, insere a matrícula e decrementa a vaga. As duas gravações recebem o mesmo objeto de transação: qualquer falha executa rollback.

Para concorrência, a turma é consultada com `UPDLOCK, HOLDLOCK` e a atualização só ocorre quando `VagasDisponiveis > 0`. A constraint `UNIQUE` do banco continua sendo a proteção final contra duplicidade.

## Status HTTP

| Status | Situação |
| --- | --- |
| 200 OK | Consulta, atualização ou exclusão lógica realizada |
| 201 Created | Recurso criado |
| 400 Bad Request | Rota, parâmetros ou corpo inválidos |
| 404 Not Found | Registro não encontrado ou aluno já inativo na exclusão |
| 409 Conflict | Regra de negócio impede a matrícula: aluno inativo, turma sem vaga ou duplicidade |
| 500 Internal Server Error | Falha inesperada de aplicação ou infraestrutura |

## Organização do código

~~~text
EscolaEvolucional.Api/
|-- App_Start/          # Configuração da Web API
|-- Controllers/        # Entrada HTTP e composição das respostas
|-- DTOs/               # Contratos de entrada, saída, erro e paginação
|-- Models/             # Modelos usados no domínio e pelo Dapper
|-- Services/           # Validações e coordenação dos casos de uso
|-- Repository/         # SQL manual e acesso ao banco com Dapper
|-- Infrastructure/
|   +-- Data/           # Criação de conexões com o SQL Server
+-- Web.config          # Configuração geral da aplicação
~~~

Os controllers não contêm SQL. Os services coordenam os casos de uso e convertem modelos em DTOs. Os repositories concentram o SQL manual e o acesso ao banco com Dapper.

Os controllers possuem construtores que recebem interfaces de service, permitindo testes e substituição das implementações. Os construtores sem parâmetros montam as dependências para que o ASP.NET Web API 2 consiga criar AlunosController, TurmasController, RelatoriosController e MatriculasController sem um contêiner de injeção de dependência. Em uma aplicação maior, essa composição seria centralizada em um contêiner configurado no início da aplicação.

## Verificações executadas no CRUD de alunos

Foram executados build em Debug e testes manuais da API com IIS Express, cobrindo:

- paginação com e sem filtro;
- página e tamanho inválidos;
- consulta existente, inexistente e identificador inválido;
- cadastro válido, corpo vazio, campos inválidos e data futura;
- atualização válida, inexistente e com data futura;
- exclusão lógica e segunda tentativa de exclusão;
- confirmação de que o registro permaneceu no banco com Ativo igual a false.

## Verificações executadas em turmas e relatório

Foram executados build em Debug e testes manuais com IIS Express, cobrindo:

- GET /api/turmas com as quatro turmas do banco;
- vagas totais e disponíveis conferidas com o script inicial;
- GET /api/relatorios/alunos-por-turma;
- contagens 2, 0, 4 e 2 para as quatro turmas;
- presença da turma 3B - Ensino Medio com quantidade zero;
- contrato JSON em camelCase;
- consulta com LEFT JOIN, COUNT(m.Id), GROUP BY e ordenação determinística;
- ausência de SQL e agrupamento nos controllers.

A suíte de testes automatizados das regras de matrícula está documentada na seção de testes abaixo.

## Verificações executadas na matrícula

Foram executados build em Debug e testes manuais com IIS Express, cobrindo:

- criação válida, com uma matrícula inserida e uma vaga decrementada;
- payload e IDs inválidos (`400 Bad Request`);
- aluno e turma inexistentes (`404 Not Found`);
- aluno inativo, turma lotada e matrícula duplicada (`409 Conflict`);
- rollback quando uma falha é provocada após o `INSERT`, sem matrícula nem vaga alterada;
- duas requisições concorrentes para a última vaga: uma retorna `201 Created`, a outra `409 Conflict`, sem ultrapassar a capacidade da turma.

## Testes automatizados da matrícula

O projeto `EscolaEvolucional.Tests` usa MSTest e é direcionado ao .NET Framework 4.8. Ele não acessa SQL Server: usa implementações falsas das interfaces para testar o `MatriculaService`, os invariantes de `MatriculaResultado` e a conversão de resultados para HTTP no `MatriculasController`.

No Visual Studio, abra o **Gerenciador de Testes** e selecione **Executar Todos**. Em um Developer PowerShell do Visual Studio, a execução também pode ser feita assim:

~~~powershell
msbuild .\EscolaEvolucional.Tests\EscolaEvolucional.Tests.csproj /t:Rebuild /p:Configuration=Debug
vstest.console .\EscolaEvolucional.Tests\bin\Debug\net48\EscolaEvolucional.Tests.dll /Platform:x64
~~~

A última execução local aprovou 16 de 16 testes. Transação SQL, rollback físico e concorrência continuam cobertos pelas verificações funcionais da Sprint 04; testes de integração automatizados exigem um banco isolado e são uma evolução futura.

## Licença

Consulte o arquivo [LICENSE](LICENSE.txt).

## Cache de turmas

`GET /api/turmas` usa `ITurmaCache` com a chave estável `turmas:listagem`. Nesta entrega, `MemoryTurmaCache` mantém entradas por chave durante um minuto, com bloqueio para concorrência e cópias defensivas. Em cache hit não há consulta SQL; em cache miss a lista é consultada e armazenada.

Após uma matrícula criada (`201`), o `MatriculaService` invalida o cache somente depois que o repository retorna sucesso — isto ocorre após o commit. Conflitos e rollback não invalidam a chave. Redis pode substituir `ITurmaCache` por outra implementação, sem alterar controllers ou services.

## Tela de alunos (bônus)

Com a API em execução, abra [Content/TelaAlunos.html](EscolaEvolucional.Api/Content/TelaAlunos.html) pelo mesmo endereço da aplicação, por exemplo:

~~~text
https://localhost:44360/Content/TelaAlunos.html
~~~

A tela usa jQuery 3.7.1 carregado por CDN e consome `GET /api/alunos` na mesma origem. Ela apresenta nome, e-mail, nascimento e situação; permite filtrar por nome, navegar entre páginas, visualizar o total e trata carregamento, lista vazia e falha de comunicação. Os valores recebidos da API são escapados antes de serem inseridos na tabela.
