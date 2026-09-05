using Dapper;
using EscolaEvolucional.Api.Infrastructure.Data;
using EscolaEvolucional.Api.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using MatriculaModel = EscolaEvolucional.Api.Models.Matricula;

namespace EscolaEvolucional.Api.Repository.Matricula
{
    public class MatriculaRepository : IMatriculaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MatriculaRepository(IDbConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException(nameof(connectionFactory));
            }

            _connectionFactory = connectionFactory;
        }

        public MatriculaResultado Matricular(int alunoId, int turmaId)
        {
            const string alunoSql = @"
                SELECT Ativo
                FROM dbo.Aluno WITH (UPDLOCK, HOLDLOCK)
                WHERE Id = @AlunoId;";

            const string turmaSql = @"
                SELECT VagasDisponiveis
                FROM dbo.Turma WITH (UPDLOCK, HOLDLOCK)
                WHERE Id = @TurmaId;";

            const string matriculaExisteSql = @"
                SELECT COUNT(1)
                FROM dbo.Matricula WITH (UPDLOCK, HOLDLOCK)
                WHERE AlunoId = @AlunoId
                  AND TurmaId = @TurmaId;";

            const string inserirSql = @"
                INSERT INTO dbo.Matricula (AlunoId, TurmaId)
                OUTPUT INSERTED.Id,
                       INSERTED.AlunoId,
                       INSERTED.TurmaId,
                       INSERTED.DataMatricula
                VALUES (@AlunoId, @TurmaId);";

            const string decrementarVagaSql = @"
                UPDATE dbo.Turma
                SET VagasDisponiveis = VagasDisponiveis - 1
                WHERE Id = @TurmaId
                  AND VagasDisponiveis > 0;";

            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        var alunoAtivo = connection.QueryFirstOrDefault<bool?>(
                            alunoSql,
                            new { AlunoId = alunoId },
                            transaction);

                        if (!alunoAtivo.HasValue)
                        {
                            return Falhar(transaction, MatriculaResultadoTipo.AlunoNaoEncontrado);
                        }

                        if (!alunoAtivo.Value)
                        {
                            return Falhar(transaction, MatriculaResultadoTipo.AlunoInativo);
                        }

                        var vagasDisponiveis = connection.QueryFirstOrDefault<int?>(
                            turmaSql,
                            new { TurmaId = turmaId },
                            transaction);

                        if (!vagasDisponiveis.HasValue)
                        {
                            return Falhar(transaction, MatriculaResultadoTipo.TurmaNaoEncontrada);
                        }

                        if (vagasDisponiveis.Value <= 0)
                        {
                            return Falhar(transaction, MatriculaResultadoTipo.TurmaSemVaga);
                        }

                        var matriculaExiste = connection.ExecuteScalar<int>(
                            matriculaExisteSql,
                            new { AlunoId = alunoId, TurmaId = turmaId },
                            transaction) > 0;

                        if (matriculaExiste)
                        {
                            return Falhar(transaction, MatriculaResultadoTipo.Duplicada);
                        }

                        var matricula = connection.QuerySingle<MatriculaModel>(
                            inserirSql,
                            new { AlunoId = alunoId, TurmaId = turmaId },
                            transaction);

                        var vagasAtualizadas = connection.Execute(
                            decrementarVagaSql,
                            new { TurmaId = turmaId },
                            transaction);

                        if (vagasAtualizadas != 1)
                        {
                            return Falhar(transaction, MatriculaResultadoTipo.TurmaSemVaga);
                        }

                        transaction.Commit();
                        return MatriculaResultado.CriarSucesso(matricula);
                    }
                    catch (SqlException exception) when (
                        exception.Number == 2601 || exception.Number == 2627)
                    {
                        return Falhar(transaction, MatriculaResultadoTipo.Duplicada);
                    }
                    catch
                    {
                        Reverter(transaction);
                        throw;
                    }
                }
            }
        }

        private static MatriculaResultado Falhar(
            IDbTransaction transaction,
            MatriculaResultadoTipo tipo)
        {
            Reverter(transaction);
            return MatriculaResultado.CriarFalha(tipo);
        }

        private static void Reverter(IDbTransaction transaction)
        {
            try
            {
                transaction.Rollback();
            }
            catch (InvalidOperationException)
            {
                // A transação já foi concluída e não há alteração pendente para desfazer.
            }
            catch (SqlException)
            {
                // A falha original não deve ser ocultada por um erro durante o rollback.
            }
        }
    }
}
