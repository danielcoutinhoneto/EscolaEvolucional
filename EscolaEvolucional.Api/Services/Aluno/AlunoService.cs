using EscolaEvolucional.Api.DTOs;
using EscolaEvolucional.Api.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using AlunoModel = EscolaEvolucional.Api.Models.Aluno;

namespace EscolaEvolucional.Api.Services.Aluno
{
    public class AlunoService : IAlunoService
    {
        private readonly IAlunoRepository _alunoRepository;

        public AlunoService(IAlunoRepository alunoRepository)
        {
            _alunoRepository = alunoRepository;
        }

        public PagedResult<AlunoResponseDto> ObterTodos(string nome, int page, int pageSize)
        {
            if (page < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(page),
                    "A página deve ser maior ou igual a 1.");
            }

            if (pageSize < 1 || pageSize > 100)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(pageSize),
                    "O tamanho da página deve estar entre 1 e 100.");
            }

            var resultado = _alunoRepository.ObterTodos(nome, page, pageSize);

            return new PagedResult<AlunoResponseDto>
            {
                Items = resultado.Items.Select(Mapear),
                Page = resultado.Page,
                PageSize = resultado.PageSize,
                Total = resultado.Total
            };
        }

        public AlunoResponseDto ObterPorId(int id)
        {
            var aluno = _alunoRepository.ObterPorId(id);
            return aluno == null ? null : Mapear(aluno);
        }

        public int Criar(AlunoCreateDto alunoCreateDto)
        {
            if (alunoCreateDto == null)
            {
                throw new ArgumentNullException(nameof(alunoCreateDto));
            }

            var aluno = new AlunoModel
            {
                Nome = alunoCreateDto.Nome.Trim(),
                Email = alunoCreateDto.Email.Trim(),
                DataNascimento = ValidarDataNascimento(alunoCreateDto.DataNascimento)
            };

            return _alunoRepository.Inserir(aluno);
        }

        public void Atualizar(int id, AlunoUpdateDto alunoUpdateDto)
        {
            if (alunoUpdateDto == null)
            {
                throw new ArgumentNullException(nameof(alunoUpdateDto));
            }

            var aluno = new AlunoModel
            {
                Id = id,
                Nome = alunoUpdateDto.Nome.Trim(),
                Email = alunoUpdateDto.Email.Trim(),
                DataNascimento = ValidarDataNascimento(alunoUpdateDto.DataNascimento)
            };

            if (!_alunoRepository.Atualizar(aluno))
            {
                throw new KeyNotFoundException("Aluno não encontrado.");
            }
        }

        public void Excluir(int id)
        {
            if (!_alunoRepository.Excluir(id))
            {
                throw new KeyNotFoundException("Aluno não encontrado ou já inativo.");
            }
        }

        private static AlunoResponseDto Mapear(AlunoModel aluno)
        {
            return new AlunoResponseDto
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                Email = aluno.Email,
                DataNascimento = aluno.DataNascimento,
                Ativo = aluno.Ativo,
                DataCadastro = aluno.DataCadastro
            };
        }

        private static DateTime ValidarDataNascimento(DateTime? dataNascimento)
        {
            if (!dataNascimento.HasValue)
            {
                throw new ArgumentException("A data de nascimento é obrigatória.");
            }

            var data = dataNascimento.Value.Date;

            if (data > DateTime.Today)
            {
                throw new ArgumentException("A data de nascimento não pode estar no futuro.");
            }

            return data;
        }
    }
}