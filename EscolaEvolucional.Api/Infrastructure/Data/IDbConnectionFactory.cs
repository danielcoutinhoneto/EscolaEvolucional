using System.Data;

namespace EscolaEvolucional.Api.Infrastructure.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
