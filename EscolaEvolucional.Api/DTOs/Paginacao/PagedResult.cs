using System.Collections.Generic;

namespace EscolaEvolucional.Api.DTOs
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
    }
}