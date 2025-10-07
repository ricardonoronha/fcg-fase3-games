namespace FIAP.MicroService.Jogos.Dominio.Models
{
    
    public class Jogo
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; } 
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
    public class ResultadoBusca<T>
    {
        public int TotalResultados { get; set; }
        public int PaginaAtual { get; set; }
        public int TamanhoPagina { get; set; }
        public IEnumerable<T> Itens { get; set; }
    }
    public class ResultadoAgregado
    {
        public string Chave { get; set; } 
        public decimal ReceitaTotal { get; set; }
        public int TotalVendas { get; set; }
    }
}