namespace FIAP.MicroService.Jogos.Dominio.Dtos;

public class PagedResult<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public long TotalItems { get; set; }
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
}
