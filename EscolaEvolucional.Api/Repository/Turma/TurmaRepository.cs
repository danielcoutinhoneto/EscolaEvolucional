using Dapper;
using EscolaEvolucional.Api.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using TurmaModel = EscolaEvolucional.Api.Models.Turma;

namespace EscolaEvolucional.Api.Repository.Turma
{
    public class TurmaRepository : ITurmaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TurmaRepository(IDbConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException(nameof(connectionFactory));
            }

            _connectionFactory = connectionFactory;
        }

        public IEnumerable<TurmaModel> ObterTodos()
        {
            const string sql = @"
                SELECT Id,
                       Nome,
                       Periodo,
                       VagasTotal,
                       VagasDisponiveis
                FROM dbo.Turma
                ORDER BY Nome, Id;";

            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                return connection.Query<TurmaModel>(sql).ToList();
            }
        }
    }
}
