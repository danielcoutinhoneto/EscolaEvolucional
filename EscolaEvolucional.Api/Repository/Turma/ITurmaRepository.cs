using System.Collections.Generic;
using TurmaModel = EscolaEvolucional.Api.Models.Turma;

namespace EscolaEvolucional.Api.Repository.Turma
{
    public interface ITurmaRepository
    {
        IEnumerable<TurmaModel> ObterTodos();
    }
}
