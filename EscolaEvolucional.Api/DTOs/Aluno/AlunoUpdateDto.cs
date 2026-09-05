using System;
using System.ComponentModel.DataAnnotations;

namespace EscolaEvolucional.Api.DTOs
{
    public class AlunoUpdateDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(120, ErrorMessage = "O nome deve ter no máximo 120 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [StringLength(120, ErrorMessage = "O e-mail deve ter no máximo 120 caracteres.")]
        [EmailAddress(ErrorMessage = "O e-mail informado é inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateTime? DataNascimento { get; set; }
    }
}