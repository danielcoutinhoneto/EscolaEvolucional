using EscolaEvolucional.Api.DTOs;
using EscolaEvolucional.Api.Models;

namespace EscolaEvolucional.Api.Repository
{
    public interface IAlunoRepository
    {
        PagedResult<Aluno> ObterTodos(string nome, int page, int pageSize);
        Aluno ObterPorId(int id);
        int Inserir(Aluno aluno);
        bool Atualizar(Aluno aluno);
        bool Excluir(int id);
    }
}