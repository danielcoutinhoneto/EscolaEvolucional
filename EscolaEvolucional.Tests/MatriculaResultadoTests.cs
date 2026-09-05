using EscolaEvolucional.Api.Models;

namespace EscolaEvolucional.Tests;

[TestClass]
public sealed class MatriculaResultadoTests
{
    [TestMethod]
    public void CriarSucesso_ComMatriculaNula_LancaExcecao()
    {
        Assert.ThrowsException<ArgumentNullException>(() => MatriculaResultado.CriarSucesso(null!));
    }

    [TestMethod]
    public void CriarFalha_ComTipoCriada_LancaExcecao()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            MatriculaResultado.CriarFalha(MatriculaResultadoTipo.Criada));
    }

    [TestMethod]
    public void CriarFalha_ComTipoInvalido_LancaExcecao()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            MatriculaResultado.CriarFalha((MatriculaResultadoTipo)999));
    }

    [TestMethod]
    public void CriarFalha_ComRegraDeNegocio_MantemMatriculaNula()
    {
        var resultado = MatriculaResultado.CriarFalha(MatriculaResultadoTipo.Duplicada);

        Assert.AreEqual(MatriculaResultadoTipo.Duplicada, resultado.Tipo);
        Assert.IsNull(resultado.Matricula);
    }
}