using Dapper;
using EscolaEvolucional.Api.DTOs;
using EscolaEvolucional.Api.Infrastructure.Data;
using EscolaEvolucional.Api.Models;
using System;
using System.Linq;

namespace EscolaEvolucional.Api.Repository
{
    public class AlunoRepository : IAlunoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AlunoRepository(IDbConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException(nameof(connectionFactory));
            }

            _connectionFactory = connectionFactory;
        }

        public PagedResult<Aluno> ObterTodos(string nome, int page, int pageSize)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.Aluno
                WHERE @Nome IS NULL OR Nome LIKE @Nome;

                SELECT Id,
                       Nome,
                       Email,
                       DataNascimento,
                       Ativo,
                       DataCadastro
                FROM dbo.Aluno
                WHERE @Nome IS NULL OR Nome LIKE @Nome
                ORDER BY Id
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;";

            var nomeFiltro = string.IsNullOrWhiteSpace(nome)
                ? null
                : "%" + nome.Trim() + "%";

            var offset = (long)(page - 1) * pageSize;

            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();

                using (var multi = connection.QueryMultiple(
                    sql,
                    new
                    {
                        Nome = nomeFiltro,
                        Offset = offset,
                        PageSize = pageSize
                    }))
                {
                    var total = multi.ReadSingle<int>();
                    var items = multi.Read<Aluno>().ToList();

                    return new PagedResult<Aluno>
                    {
                        Items = items,
                        Page = page,
                        PageSize = pageSize,
                        Total = total
                    };
                }
            }
        }

        public Aluno ObterPorId(int id)
        {
            const string sql = @"
                SELECT Id,
                       Nome,
                       Email,
                       DataNascimento,
                       Ativo,
                       DataCadastro
                FROM dbo.Aluno
                WHERE Id = @Id;";

            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                return connection.QueryFirstOrDefault<Aluno>(sql, new { Id = id });
            }
        }

        public int Inserir(Aluno aluno)
        {
            const string sql = @"
                INSERT INTO dbo.Aluno (Nome, Email, DataNascimento)
                VALUES (@Nome, @Email, @DataNascimento);

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                return connection.QuerySingle<int>(sql, aluno);
            }
        }

        public bool Atualizar(Aluno aluno)
        {
            const string sql = @"
                UPDATE dbo.Aluno
                SET Nome = @Nome,
                    Email = @Email,
                    DataNascimento = @DataNascimento
                WHERE Id = @Id
                  AND Ativo = 1;";

            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                return connection.Execute(sql, aluno) > 0;
            }
        }

        public bool Excluir(int id)
        {
            const string sql = @"
                UPDATE dbo.Aluno
                SET Ativo = 0
                WHERE Id = @Id
                  AND Ativo = 1;";

            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                return connection.Execute(sql, new { Id = id }) > 0;
            }
        }
    }
}