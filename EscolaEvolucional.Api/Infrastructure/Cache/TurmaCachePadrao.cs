using System;

namespace EscolaEvolucional.Api.Infrastructure.Cache
{
    public static class TurmaCachePadrao
    {
        public static readonly ITurmaCache Instancia = new MemoryTurmaCache(TimeSpan.FromMinutes(1));
    }
}