using System.ComponentModel.DataAnnotations;

namespace EscolaEvolucional.Api.DTOs.Matricula
{
    public class MatriculaCreateDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "O id do aluno deve ser maior que zero.")]
        public int AlunoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O id da turma deve ser maior que zero.")]
        public int TurmaId { get; set; }
    }
}
