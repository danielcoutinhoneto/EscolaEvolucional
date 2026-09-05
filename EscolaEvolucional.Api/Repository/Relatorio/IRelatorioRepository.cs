using EscolaEvolucional.Api.Models;
using System.Collections.Generic;

namespace EscolaEvolucional.Api.Repository.Relatorio
{
    public interface IRelatorioRepository
    {
        IEnumerable<RelatorioAlunosPorTurma> ObterAlunosPorTurma();
    }
}
