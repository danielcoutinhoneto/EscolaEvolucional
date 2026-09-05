using EscolaEvolucional.Api.DTOs.Matricula;
using EscolaEvolucional.Api.Models;
using EscolaEvolucional.Api.Repository.Matricula;
using System;

namespace EscolaEvolucional.Api.Services.Matricula
{
    public class MatriculaService : IMatriculaService
    {
        private readonly IMatriculaRepository _matriculaRepository;

        public MatriculaService(IMatriculaRepository matriculaRepository)
        {
            if (matriculaRepository == null)
            {
                throw new ArgumentNullException(nameof(matriculaRepository));
            }

            _matriculaRepository = matriculaRepository;
        }

        public MatriculaResultado Matricular(MatriculaCreateDto matriculaCreateDto)
        {
            if (matriculaCreateDto == null)
            {
                throw new ArgumentNullException(nameof(matriculaCreateDto));
            }

            if (matriculaCreateDto.AlunoId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(matriculaCreateDto.AlunoId),
                    "O id do aluno deve ser maior que zero.");
            }

            if (matriculaCreateDto.TurmaId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(matriculaCreateDto.TurmaId),
                    "O id da turma deve ser maior que zero.");
            }

            return _matriculaRepository.Matricular(
                matriculaCreateDto.AlunoId,
                matriculaCreateDto.TurmaId);
        }
    }
}
