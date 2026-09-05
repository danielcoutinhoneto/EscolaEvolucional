namespace EscolaEvolucional.Api.DTOs.Relatorio
{
    public class AlunosPorTurmaResponseDto
    {
        public int TurmaId { get; set; }
        public string TurmaNome { get; set; }
        public int QuantidadeAlunos { get; set; }
        public int VagasRestantes { get; set; }
    }
}
