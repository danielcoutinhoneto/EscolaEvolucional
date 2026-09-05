using EscolaEvolucional.Api.DTOs;

namespace EscolaEvolucional.Api.Services.Aluno
{
    public interface IAlunoService
    {
        PagedResult<AlunoResponseDto> ObterTodos(string nome, int page, int pageSize);
        AlunoResponseDto ObterPorId(int id);
        int Criar(AlunoCreateDto alunoCreateDto);
        void Atualizar(int id, AlunoUpdateDto alunoUpdateDto);
        void Excluir(int id);
    }
}