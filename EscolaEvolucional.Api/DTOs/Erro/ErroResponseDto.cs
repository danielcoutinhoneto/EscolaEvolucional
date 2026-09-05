using System.Collections.Generic;

namespace EscolaEvolucional.Api.DTOs
{
    public class ErroResponseDto
    {
        public string Message { get; set; }
        public IDictionary<string, string[]> Errors { get; set; }
    }
}