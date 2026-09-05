using EscolaEvolucional.Api.Controllers;
using EscolaEvolucional.Api.DTOs;
using EscolaEvolucional.Api.DTOs.Matricula;
using EscolaEvolucional.Api.Models;
using EscolaEvolucional.Api.Services.Matricula;
using System.Net;
using System.Web.Http.Results;

namespace EscolaEvolucional.Tests;

[TestClass]
public sealed class MatriculasControllerTests
{
    [TestMethod]
    public void Criar_ComSucesso_RetornaCreated()
    {
        var controller = CriarController(MatriculaResultado.CriarSucesso(new Matricula
        {
            Id = 1,
            AlunoId = 2,
            TurmaId = 3,
            DataMatricula = DateTime.UtcNow
        }));

        var resultado = controller.Criar(new MatriculaCreateDto { AlunoId = 2, TurmaId = 3 })
            as NegotiatedContentResult<MatriculaResponseDto>;

        Assert.IsNotNull(resultado);
        Assert.AreEqual(HttpStatusCode.Created, resultado.StatusCode);
    }

    [TestMethod]
    public void Criar_ComAlunoOuTurmaInexistente_RetornaNotFound()
    {
        foreach (var tipo in new[]
                 {
                     MatriculaResultadoTipo.AlunoNaoEncontrado,
                     MatriculaResultadoTipo.TurmaNaoEncontrada
                 })
        {
            var resultado = CriarController(MatriculaResultado.CriarFalha(tipo))
                .Criar(new MatriculaCreateDto { AlunoId = 1, TurmaId = 1 })
                as NegotiatedContentResult<ErroResponseDto>;

            Assert.IsNotNull(resultado, tipo.ToString());
            Assert.AreEqual(HttpStatusCode.NotFound, resultado.StatusCode, tipo.ToString());
        }
    }

    [TestMethod]
    public void Criar_ComConflitoDeNegocio_RetornaConflict()
    {
        foreach (var tipo in new[]
                 {
                     MatriculaResultadoTipo.AlunoInativo,
                     MatriculaResultadoTipo.TurmaSemVaga,
                     MatriculaResultadoTipo.Duplicada
                 })
        {
            var resultado = CriarController(MatriculaResultado.CriarFalha(tipo))
                .Criar(new MatriculaCreateDto { AlunoId = 1, TurmaId = 1 })
                as NegotiatedContentResult<ErroResponseDto>;

            Assert.IsNotNull(resultado, tipo.ToString());
            Assert.AreEqual(HttpStatusCode.Conflict, resultado.StatusCode, tipo.ToString());
        }
    }

    [TestMethod]
    public void Criar_ComCorpoNulo_RetornaBadRequestSemChamarService()
    {
        var service = new MatriculaServiceFalso(MatriculaResultado.CriarFalha(MatriculaResultadoTipo.Duplicada));
        var controller = CriarController(service);

        var resultado = controller.Criar(null) as NegotiatedContentResult<ErroResponseDto>;

        Assert.IsNotNull(resultado);
        Assert.AreEqual(HttpStatusCode.BadRequest, resultado.StatusCode);
        Assert.AreEqual(0, service.QuantidadeChamadas);
    }

    private static MatriculasController CriarController(MatriculaResultado resultado)
    {
        return CriarController(new MatriculaServiceFalso(resultado));
    }

    private static MatriculasController CriarController(IMatriculaService service)
    {
        return new MatriculasController(service);
    }

    private sealed class MatriculaServiceFalso : IMatriculaService
    {
        private readonly MatriculaResultado _resultado;

        public MatriculaServiceFalso(MatriculaResultado resultado)
        {
            _resultado = resultado;
        }

        public int QuantidadeChamadas { get; private set; }

        public MatriculaResultado Matricular(MatriculaCreateDto matriculaCreateDto)
        {
            QuantidadeChamadas++;
            return _resultado;
        }
    }
}