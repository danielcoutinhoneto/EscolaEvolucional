using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace EscolaEvolucional.Api.Infrastructure.Data
{
    public sealed class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory()
        {
            var configuration = ConfigurationManager.ConnectionStrings["TesteEscola"];

            if (configuration == null || string.IsNullOrWhiteSpace(configuration.ConnectionString))
            {
                throw new InvalidOperationException("A Connection string 'TesteEscola' não foi configurada.");
            }
            _connectionString = configuration.ConnectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}