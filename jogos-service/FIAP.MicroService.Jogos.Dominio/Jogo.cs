namespace FIAP.MicroService.Jogos.Dominio;

public class Jogo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public DateTime DataLancamento { get; set; } = DateTime.UtcNow;

    public Jogo() { }

    public Jogo(string nome, string categoria, decimal preco)
    {
        Nome = nome;
        Categoria = categoria;
        Preco = preco;
    }
}