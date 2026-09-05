using EscolaEvolucional.Api.DTOs.Turma;
using EscolaEvolucional.Api.Repository.Turma;
using System;
using System.Collections.Generic;
using System.Linq;
using TurmaModel = EscolaEvolucional.Api.Models.Turma;

namespace EscolaEvolucional.Api.Services.Turma
{
    public class TurmaService : ITurmaService
    {
        private readonly ITurmaRepository _turmaRepository;

        public TurmaService(ITurmaRepository turmaRepository)
        {
            if (turmaRepository == null)
            {
                throw new ArgumentNullException(nameof(turmaRepository));
            }

            _turmaRepository = turmaRepository;
        }

        public IEnumerable<TurmaResponseDto> ObterTodos()
        {
            return _turmaRepository
                .ObterTodos()
                .Select(Mapear)
                .ToList();
        }

        private static TurmaResponseDto Mapear(TurmaModel turma)
        {
            return new TurmaResponseDto
            {
                Id = turma.Id,
                Nome = turma.Nome,
                Periodo = turma.Periodo,
                VagasTotal = turma.VagasTotal,
                VagasDisponiveis = turma.VagasDisponiveis
            };
        }
    }
}
