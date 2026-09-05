using EscolaEvolucional.Api.DTOs.Turma;
using System.Collections.Generic;
namespace EscolaEvolucional.Api.Infrastructure.Cache { public interface ITurmaCache { bool TentarObter(string chave, out IReadOnlyList<TurmaResponseDto> turmas); void Armazenar(string chave, IEnumerable<TurmaResponseDto> turmas); void Invalidar(string chave); } }