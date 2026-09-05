using EscolaEvolucional.Api.DTOs.Matricula;
using EscolaEvolucional.Api.DTOs.Turma;
using EscolaEvolucional.Api.Infrastructure.Cache;
using EscolaEvolucional.Api.Models;
using EscolaEvolucional.Api.Repository.Matricula;
using EscolaEvolucional.Api.Repository.Turma;
using EscolaEvolucional.Api.Services.Matricula;
using EscolaEvolucional.Api.Services.Turma;
using System.Collections.Generic;

namespace EscolaEvolucional.Tests;

[TestClass]
public sealed class CacheTurmasTests
{
    [TestMethod]
    public void ObterTodos_ComCacheHit_ConsultaRepositorioUmaVez()
    {
        var repository = new TurmaRepositoryFalso();
        var service = new TurmaService(repository, new MemoryTurmaCache(System.TimeSpan.FromMinutes(1)));

        service.ObterTodos();
        service.ObterTodos();

        Assert.AreEqual(1, repository.QuantidadeChamadas);
    }

    [TestMethod]
    public void Matricular_ComSucesso_InvalidaCache()
    {
        var cache = new CacheEspiao();
        var service = new MatriculaService(new MatriculaRepositoryFalso(MatriculaResultado.CriarSucesso(new Matricula())), cache);

        service.Matricular(new MatriculaCreateDto { AlunoId = 1, TurmaId = 1 });

        Assert.AreEqual(1, cache.QuantidadeInvalidacoes);
    }

    [TestMethod]
    public void Matricular_ComConflito_NaoInvalidaCache()
    {
        var cache = new CacheEspiao();
        var service = new MatriculaService(new MatriculaRepositoryFalso(MatriculaResultado.CriarFalha(MatriculaResultadoTipo.Duplicada)), cache);

        service.Matricular(new MatriculaCreateDto { AlunoId = 1, TurmaId = 1 });

        Assert.AreEqual(0, cache.QuantidadeInvalidacoes);
    }

    private sealed class TurmaRepositoryFalso : ITurmaRepository
    {
        public int QuantidadeChamadas { get; private set; }
        public IEnumerable<Turma> ObterTodos() { QuantidadeChamadas++; return new[] { new Turma { Id = 1, Nome = "A", Periodo = "Manha", VagasTotal = 1, VagasDisponiveis = 1 } }; }
    }

    private sealed class MatriculaRepositoryFalso : IMatriculaRepository
    {
        private readonly MatriculaResultado _resultado;
        public MatriculaRepositoryFalso(MatriculaResultado resultado) { _resultado = resultado; }
        public MatriculaResultado Matricular(int alunoId, int turmaId) { return _resultado; }
    }

    private sealed class CacheEspiao : ITurmaCache
    {
        public int QuantidadeInvalidacoes { get; private set; }
        public bool TentarObter(string chave, out IReadOnlyList<TurmaResponseDto> turmas) { turmas = null!; return false; }
        public void Armazenar(string chave, IEnumerable<TurmaResponseDto> turmas) { }
        public void Invalidar(string chave) { QuantidadeInvalidacoes++; }
    }
}