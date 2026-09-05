using EscolaEvolucional.Api.Infrastructure.Data;
using EscolaEvolucional.Api.Repository.Relatorio;
using EscolaEvolucional.Api.Services.Relatorio;
using System;
using System.Web.Http;

namespace EscolaEvolucional.Api.Controllers
{
    [RoutePrefix("api/relatorios")]
    public class RelatoriosController : ApiController
    {
        private readonly IRelatorioService _relatorioService;

        public RelatoriosController()
            : this(new RelatorioService(
                new RelatorioRepository(
                    new SqlConnectionFactory())))
        {
        }

        public RelatoriosController(IRelatorioService relatorioService)
        {
            if (relatorioService == null)
            {
                throw new ArgumentNullException(nameof(relatorioService));
            }

            _relatorioService = relatorioService;
        }

        [HttpGet]
        [Route("alunos-por-turma")]
        public IHttpActionResult ObterAlunosPorTurma()
        {
            return Ok(_relatorioService.ObterAlunosPorTurma());
        }
    }
}
