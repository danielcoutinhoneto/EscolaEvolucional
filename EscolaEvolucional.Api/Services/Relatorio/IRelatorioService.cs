using EscolaEvolucional.Api.DTOs.Relatorio;
using System.Collections.Generic;

namespace EscolaEvolucional.Api.Services.Relatorio
{
    public interface IRelatorioService
    {
        IEnumerable<AlunosPorTurmaResponseDto> ObterAlunosPorTurma();
    }
}
