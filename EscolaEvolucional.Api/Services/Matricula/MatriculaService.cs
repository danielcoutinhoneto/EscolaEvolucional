using EscolaEvolucional.Api.DTOs.Matricula;
using EscolaEvolucional.Api.Infrastructure.Cache;
using EscolaEvolucional.Api.Models;
using EscolaEvolucional.Api.Repository.Matricula;
using System;

namespace EscolaEvolucional.Api.Services.Matricula
{
    public class MatriculaService : IMatriculaService
    {
        private readonly IMatriculaRepository _matriculaRepository;
        private readonly ITurmaCache _turmaCache;

        public MatriculaService(IMatriculaRepository matriculaRepository)
            : this(matriculaRepository, TurmaCachePadrao.Instancia)
        {
        }

        public MatriculaService(IMatriculaRepository matriculaRepository, ITurmaCache turmaCache)
        {
            if (matriculaRepository == null)
            {
                throw new ArgumentNullException(nameof(matriculaRepository));
            }

            if (turmaCache == null)
            {
                throw new ArgumentNullException(nameof(turmaCache));
            }

            _matriculaRepository = matriculaRepository;
            _turmaCache = turmaCache;
        }

        public MatriculaResultado Matricular(MatriculaCreateDto matriculaCreateDto)
        {
            if (matriculaCreateDto == null)
            {
                throw new ArgumentNullException(nameof(matriculaCreateDto));
            }

            if (matriculaCreateDto.AlunoId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(matriculaCreateDto.AlunoId), "O id do aluno deve ser maior que zero.");
            }

            if (matriculaCreateDto.TurmaId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(matriculaCreateDto.TurmaId), "O id da turma deve ser maior que zero.");
            }

            var resultado = _matriculaRepository.Matricular(matriculaCreateDto.AlunoId, matriculaCreateDto.TurmaId);
            if (resultado.Tipo == MatriculaResultadoTipo.Criada)
            {
                _turmaCache.Invalidar(TurmaCacheChaves.Listagem);
            }

            return resultado;
        }
    }
}