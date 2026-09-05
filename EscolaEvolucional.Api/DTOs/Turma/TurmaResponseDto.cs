using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EscolaEvolucional.Api.DTOs.Erro
{
    public class TurmaResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Periodo { get; set; }
        public int VagasTotal { get; set; }
        public int VagasDisponiveis { get; set; }
    }
}