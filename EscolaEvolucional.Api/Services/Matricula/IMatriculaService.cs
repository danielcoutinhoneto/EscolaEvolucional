using EscolaEvolucional.Api.DTOs.Matricula;
using EscolaEvolucional.Api.Models;

namespace EscolaEvolucional.Api.Services.Matricula
{
    public interface IMatriculaService
    {
        MatriculaResultado Matricular(MatriculaCreateDto matriculaCreateDto);
    }
}
