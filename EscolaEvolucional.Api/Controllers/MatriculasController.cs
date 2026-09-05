using EscolaEvolucional.Api.DTOs;
using EscolaEvolucional.Api.DTOs.Matricula;
using EscolaEvolucional.Api.Infrastructure.Data;
using EscolaEvolucional.Api.Models;
using EscolaEvolucional.Api.Repository.Matricula;
using EscolaEvolucional.Api.Services.Matricula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using MatriculaModel = EscolaEvolucional.Api.Models.Matricula;

namespace EscolaEvolucional.Api.Controllers
{
    [RoutePrefix("api/matriculas")]
    public class MatriculasController : ApiController
    {
        private readonly IMatriculaService _matriculaService;

        public MatriculasController()
            : this(new MatriculaService(
                new MatriculaRepository(
                    new SqlConnectionFactory())))
        {
        }

        public MatriculasController(IMatriculaService matriculaService)
        {
            if (matriculaService == null)
            {
                throw new ArgumentNullException(nameof(matriculaService));
            }

            _matriculaService = matriculaService;
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Criar(MatriculaCreateDto matriculaCreateDto)
        {
            if (matriculaCreateDto == null)
            {
                return Erro(
                    HttpStatusCode.BadRequest,
                    "O corpo da requisição é obrigatório.");
            }

            if (!ModelState.IsValid)
            {
                return ErroModelState();
            }

            try
            {
                var resultado = _matriculaService.Matricular(matriculaCreateDto);

                switch (resultado.Tipo)
                {
                    case MatriculaResultadoTipo.Criada:
                        return Content(
                            HttpStatusCode.Created,
                            Mapear(resultado.Matricula));

                    case MatriculaResultadoTipo.AlunoNaoEncontrado:
                        return Erro(HttpStatusCode.NotFound, "Aluno não encontrado.");

                    case MatriculaResultadoTipo.TurmaNaoEncontrada:
                        return Erro(HttpStatusCode.NotFound, "Turma não encontrada.");

                    case MatriculaResultadoTipo.AlunoInativo:
                        return Erro(HttpStatusCode.Conflict, "O aluno está inativo.");

                    case MatriculaResultadoTipo.TurmaSemVaga:
                        return Erro(HttpStatusCode.Conflict, "A turma não possui vagas disponíveis.");

                    case MatriculaResultadoTipo.Duplicada:
                        return Erro(
                            HttpStatusCode.Conflict,
                            "O aluno já está matriculado nesta turma.");

                    default:
                        return Erro(
                            HttpStatusCode.InternalServerError,
                            "Resultado de matrícula não reconhecido.");
                }
            }
            catch (ArgumentOutOfRangeException exception)
            {
                return Erro(HttpStatusCode.BadRequest, exception.Message);
            }
            catch (ArgumentException exception)
            {
                return Erro(HttpStatusCode.BadRequest, exception.Message);
            }
        }

        private IHttpActionResult ErroModelState()
        {
            var erros = ModelState
                .Where(item => item.Value.Errors.Count > 0)
                .ToDictionary(
                    item => item.Key,
                    item => item.Value.Errors
                        .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? "Valor inválido."
                            : error.ErrorMessage)
                        .ToArray());

            return Erro(
                HttpStatusCode.BadRequest,
                "A requisição contém dados inválidos.",
                erros);
        }

        private static MatriculaResponseDto Mapear(MatriculaModel matricula)
        {
            return new MatriculaResponseDto
            {
                Id = matricula.Id,
                AlunoId = matricula.AlunoId,
                TurmaId = matricula.TurmaId,
                DataMatricula = matricula.DataMatricula
            };
        }

        private IHttpActionResult Erro(
            HttpStatusCode statusCode,
            string message,
            IDictionary<string, string[]> errors = null)
        {
            return Content(
                statusCode,
                new ErroResponseDto
                {
                    Message = message,
                    Errors = errors
                });
        }
    }
}
