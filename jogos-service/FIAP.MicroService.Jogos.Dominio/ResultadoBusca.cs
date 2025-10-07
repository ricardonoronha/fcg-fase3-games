namespace FIAP.MicroService.Jogos.Dominio;

public class ResultadoBusca<T> where T : class
{
    public long TotalResultados { get; set; }
    public int PaginaAtual { get; set; }
    public int TamanhoPagina { get; set; }
    public IEnumerable<T> Itens { get; set; } = new List<T>();
}