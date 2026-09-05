using EscolaEvolucional.Api.DTOs.Turma;
using System.Collections.Generic;

namespace EscolaEvolucional.Api.Services.Turma
{
    public interface ITurmaService
    {
        IEnumerable<TurmaResponseDto> ObterTodos();
    }
}
