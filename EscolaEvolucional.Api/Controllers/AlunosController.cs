using EscolaEvolucional.Api.DTOs;
using EscolaEvolucional.Api.Infrastructure.Data;
using EscolaEvolucional.Api.Repository;
using EscolaEvolucional.Api.Services.Aluno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace EscolaEvolucional.Api.Controllers
{
    [RoutePrefix("api/alunos")]
    public class AlunosController : ApiController
    {
        private readonly IAlunoService _alunoService;

        public AlunosController()
            : this(new AlunoService(
                new AlunoRepository(
                    new SqlConnectionFactory())))
        {
        }

        public AlunosController(IAlunoService alunoService)
        {
            if (alunoService == null)
            {
                throw new ArgumentNullException(nameof(alunoService));
            }

            _alunoService = alunoService;
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult ObterTodos(
            string nome = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var resultado = _alunoService.ObterTodos(nome, page, pageSize);
                return Ok(resultado);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                return Erro(HttpStatusCode.BadRequest, exception.Message);
            }
        }

        [HttpGet]
        [Route("{id:int}", Name = "ObterAlunoPorId")]
        public IHttpActionResult ObterPorId(int id)
        {
            if (id <= 0)
            {
                return Erro(
                    HttpStatusCode.BadRequest,
                    "O identificador deve ser maior que zero.");
            }

            var aluno = _alunoService.ObterPorId(id);

            if (aluno == null)
            {
                return Erro(HttpStatusCode.NotFound, "Aluno não encontrado.");
            }

            return Ok(aluno);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Criar(AlunoCreateDto alunoCreateDto)
        {
            if (alunoCreateDto == null)
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
                var id = _alunoService.Criar(alunoCreateDto);
                var alunoCriado = _alunoService.ObterPorId(id);

                return CreatedAtRoute(
                    "ObterAlunoPorId",
                    new { id },
                    alunoCriado);
            }
            catch (ArgumentException exception)
            {
                return Erro(HttpStatusCode.BadRequest, exception.Message);
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Atualizar(int id, AlunoUpdateDto alunoUpdateDto)
        {
            if (id <= 0)
            {
                return Erro(
                    HttpStatusCode.BadRequest,
                    "O identificador deve ser maior que zero.");
            }

            if (alunoUpdateDto == null)
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
                _alunoService.Atualizar(id, alunoUpdateDto);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return Erro(HttpStatusCode.NotFound, "Aluno não encontrado.");
            }
            catch (ArgumentException exception)
            {
                return Erro(HttpStatusCode.BadRequest, exception.Message);
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Excluir(int id)
        {
            if (id <= 0)
            {
                return Erro(
                    HttpStatusCode.BadRequest,
                    "O identificador deve ser maior que zero.");
            }

            try
            {
                _alunoService.Excluir(id);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return Erro(
                    HttpStatusCode.NotFound,
                    "Aluno não encontrado ou já inativo.");
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