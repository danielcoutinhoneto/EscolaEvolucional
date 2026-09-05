namespace EscolaEvolucional.Api.Models
{
    public class RelatorioAlunosPorTurma
    {
        public int TurmaId { get; set; }
        public string TurmaNome { get; set; }
        public int QuantidadeAlunos { get; set; }
        public int VagasRestantes { get; set; }
    }
}
