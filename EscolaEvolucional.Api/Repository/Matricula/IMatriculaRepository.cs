using EscolaEvolucional.Api.Models;

namespace EscolaEvolucional.Api.Repository.Matricula
{
    public interface IMatriculaRepository
    {
        MatriculaResultado Matricular(int alunoId, int turmaId);
    }
}
