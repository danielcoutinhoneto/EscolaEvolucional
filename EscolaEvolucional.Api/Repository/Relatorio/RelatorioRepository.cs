using Dapper;
using EscolaEvolucional.Api.Infrastructure.Data;
using EscolaEvolucional.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EscolaEvolucional.Api.Repository.Relatorio
{
    public class RelatorioRepository : IRelatorioRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RelatorioRepository(IDbConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException(nameof(connectionFactory));
            }

            _connectionFactory = connectionFactory;
        }

        public IEnumerable<RelatorioAlunosPorTurma> ObterAlunosPorTurma()
        {
            const string sql = @"
                SELECT t.Id AS TurmaId,
                       t.Nome AS TurmaNome,
                       COUNT(m.Id) AS QuantidadeAlunos,
                       t.VagasDisponiveis AS VagasRestantes
                FROM dbo.Turma AS t
                LEFT JOIN dbo.Matricula AS m
                    ON m.TurmaId = t.Id
                GROUP BY t.Id,
                         t.Nome,
                         t.VagasDisponiveis
                ORDER BY t.Nome, t.Id;";

            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                return connection.Query<RelatorioAlunosPorTurma>(sql).ToList();
            }
        }
    }
}
