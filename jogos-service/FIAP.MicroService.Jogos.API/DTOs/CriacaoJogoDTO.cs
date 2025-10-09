using FIAP.MicroService.Jogos.Dominio.Enums;

namespace FIAP.MicroService.Jogos.API.DTOs
{
    public class CriacaoJogoDTO
    {
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public EnumeradorClassificacaoDoJogo Classificacao { get; set; }
        public decimal Preco { get; set; }
        public DateTime DataLancamento { get; set; }
    }
}
