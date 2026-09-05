using EscolaEvolucional.Api.DTOs.Matricula;
using EscolaEvolucional.Api.Models;
using EscolaEvolucional.Api.Repository.Matricula;
using EscolaEvolucional.Api.Services.Matricula;

namespace EscolaEvolucional.Tests;

[TestClass]
public sealed class MatriculaServiceTests
{
    [TestMethod]
    public void Matricular_ComDadosValidos_DelegaIdsAoRepositorioERetornaSucesso()
    {
        var resultadoEsperado = MatriculaResultado.CriarSucesso(new Matricula
        {
            Id = 10,
            AlunoId = 2,
            TurmaId = 3
        });
        var repositorio = new RepositorioMatriculaFalso(resultadoEsperado);
        var service = new MatriculaService(repositorio);

        var resultado = service.Matricular(new MatriculaCreateDto
        {
            AlunoId = 2,
            TurmaId = 3
        });

        Assert.AreSame(resultadoEsperado, resultado);
        Assert.AreEqual(1, repositorio.QuantidadeChamadas);
        Assert.AreEqual(2, repositorio.UltimoAlunoId);
        Assert.AreEqual(3, repositorio.UltimoTurmaId);
    }

    [TestMethod]
    public void Matricular_ComAlunoIdInvalido_NaoChamaRepositorio()
    {
        var repositorio = new RepositorioMatriculaFalso(CriarFalha(MatriculaResultadoTipo.AlunoNaoEncontrado));
        var service = new MatriculaService(repositorio);

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => service.Matricular(
            new MatriculaCreateDto { AlunoId = 0, TurmaId = 1 }));

        Assert.AreEqual(0, repositorio.QuantidadeChamadas);
    }

    [TestMethod]
    public void Matricular_ComTurmaIdInvalido_NaoChamaRepositorio()
    {
        var repositorio = new RepositorioMatriculaFalso(CriarFalha(MatriculaResultadoTipo.TurmaNaoEncontrada));
        var service = new MatriculaService(repositorio);

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => service.Matricular(
            new MatriculaCreateDto { AlunoId = 1, TurmaId = 0 }));

        Assert.AreEqual(0, repositorio.QuantidadeChamadas);
    }

    [TestMethod]
    public void Matricular_ComAlunoInexistente_RetornaResultadoDoRepositorio()
    {
        AssertRetornaFalhaDoRepositorio(MatriculaResultadoTipo.AlunoNaoEncontrado);
    }

    [TestMethod]
    public void Matricular_ComAlunoInativo_RetornaResultadoDoRepositorio()
    {
        AssertRetornaFalhaDoRepositorio(MatriculaResultadoTipo.AlunoInativo);
    }

    [TestMethod]
    public void Matricular_ComTurmaInexistente_RetornaResultadoDoRepositorio()
    {
        AssertRetornaFalhaDoRepositorio(MatriculaResultadoTipo.TurmaNaoEncontrada);
    }

    [TestMethod]
    public void Matricular_ComTurmaSemVaga_RetornaResultadoDoRepositorio()
    {
        AssertRetornaFalhaDoRepositorio(MatriculaResultadoTipo.TurmaSemVaga);
    }

    [TestMethod]
    public void Matricular_ComMatriculaDuplicada_RetornaResultadoDoRepositorio()
    {
        AssertRetornaFalhaDoRepositorio(MatriculaResultadoTipo.Duplicada);
    }

    private static void AssertRetornaFalhaDoRepositorio(MatriculaResultadoTipo tipo)
    {
        var resultadoEsperado = CriarFalha(tipo);
        var repositorio = new RepositorioMatriculaFalso(resultadoEsperado);
        var service = new MatriculaService(repositorio);

        var resultado = service.Matricular(new MatriculaCreateDto { AlunoId = 1, TurmaId = 1 });

        Assert.AreSame(resultadoEsperado, resultado);
        Assert.AreEqual(tipo, resultado.Tipo);
        Assert.AreEqual(1, repositorio.QuantidadeChamadas);
    }

    private static MatriculaResultado CriarFalha(MatriculaResultadoTipo tipo)
    {
        return MatriculaResultado.CriarFalha(tipo);
    }

    private sealed class RepositorioMatriculaFalso : IMatriculaRepository
    {
        private readonly MatriculaResultado _resultado;

        public RepositorioMatriculaFalso(MatriculaResultado resultado)
        {
            _resultado = resultado;
        }

        public int QuantidadeChamadas { get; private set; }
        public int UltimoAlunoId { get; private set; }
        public int UltimoTurmaId { get; private set; }

        public MatriculaResultado Matricular(int alunoId, int turmaId)
        {
            QuantidadeChamadas++;
            UltimoAlunoId = alunoId;
            UltimoTurmaId = turmaId;
            return _resultado;
        }
    }
}