using System;

namespace EscolaEvolucional.Api.DTOs.Matricula
{
    public class MatriculaResponseDto
    {
        public int Id { get; set; }
        public int AlunoId { get; set; }
        public int TurmaId { get; set; }
        public DateTime DataMatricula { get; set; }
    }
}
