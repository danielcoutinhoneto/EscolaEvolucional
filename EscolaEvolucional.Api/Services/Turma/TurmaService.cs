using EscolaEvolucional.Api.DTOs.Turma;
using EscolaEvolucional.Api.Infrastructure.Cache;
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
        private readonly ITurmaCache _turmaCache;

        public TurmaService(ITurmaRepository turmaRepository)
            : this(turmaRepository, TurmaCachePadrao.Instancia)
        {
        }

        public TurmaService(ITurmaRepository turmaRepository, ITurmaCache turmaCache)
        {
            if (turmaRepository == null)
            {
                throw new ArgumentNullException(nameof(turmaRepository));
            }

            if (turmaCache == null)
            {
                throw new ArgumentNullException(nameof(turmaCache));
            }

            _turmaRepository = turmaRepository;
            _turmaCache = turmaCache;
        }

        public IEnumerable<TurmaResponseDto> ObterTodos()
        {
            IReadOnlyList<TurmaResponseDto> turmas;
            if (_turmaCache.TentarObter(TurmaCacheChaves.Listagem, out turmas))
            {
                return turmas;
            }

            turmas = _turmaRepository.ObterTodos().Select(Mapear).ToList();
            _turmaCache.Armazenar(TurmaCacheChaves.Listagem, turmas);
            return turmas;
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