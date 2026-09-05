using EscolaEvolucional.Api.Infrastructure.Data;
using EscolaEvolucional.Api.Repository.Turma;
using EscolaEvolucional.Api.Services.Turma;
using System;
using System.Web.Http;

namespace EscolaEvolucional.Api.Controllers
{
    [RoutePrefix("api/turmas")]
    public class TurmasController : ApiController
    {
        private readonly ITurmaService _turmaService;

        public TurmasController()
            : this(new TurmaService(
                new TurmaRepository(
                    new SqlConnectionFactory())))
        {
        }

        public TurmasController(ITurmaService turmaService)
        {
            if (turmaService == null)
            {
                throw new ArgumentNullException(nameof(turmaService));
            }

            _turmaService = turmaService;
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult ObterTodos()
        {
            return Ok(_turmaService.ObterTodos());
        }
    }
}
