using EscolaEvolucional.Api.DTOs.Relatorio;
using EscolaEvolucional.Api.Repository.Relatorio;
using System;
using System.Collections.Generic;
using System.Linq;
using RelatorioModel = EscolaEvolucional.Api.Models.RelatorioAlunosPorTurma;

namespace EscolaEvolucional.Api.Services.Relatorio
{
    public class RelatorioService : IRelatorioService
    {
        private readonly IRelatorioRepository _relatorioRepository;

        public RelatorioService(IRelatorioRepository relatorioRepository)
        {
            if (relatorioRepository == null)
            {
                throw new ArgumentNullException(nameof(relatorioRepository));
            }

            _relatorioRepository = relatorioRepository;
        }

        public IEnumerable<AlunosPorTurmaResponseDto> ObterAlunosPorTurma()
        {
            return _relatorioRepository
                .ObterAlunosPorTurma()
                .Select(Mapear)
                .ToList();
        }

        private static AlunosPorTurmaResponseDto Mapear(RelatorioModel item)
        {
            return new AlunosPorTurmaResponseDto
            {
                TurmaId = item.TurmaId,
                TurmaNome = item.TurmaNome,
                QuantidadeAlunos = item.QuantidadeAlunos,
                VagasRestantes = item.VagasRestantes
            };
        }
    }
}
