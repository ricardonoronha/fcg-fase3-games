using FluentValidation;
using FIAP.MicroService.Jogos.Dominio.Enums;

namespace FIAP.MicroService.Jogos.Dominio.Models
{
    public class Jogo
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public EnumeradorClassificacaoDoJogo Classificacao { get; set; }
        public decimal Preco { get; set; }
        public DateTime DataLancamento { get; set; } = DateTime.UtcNow;

        public Jogo() { }

        public Jogo(string nome, string categoria, EnumeradorClassificacaoDoJogo classificacao, decimal preco)
        {
            Nome = nome;
            Categoria = categoria;
            Classificacao = classificacao;
            Preco = preco;
        }
    }

    public class JogoValidation : AbstractValidator<Jogo>
    {
        public JogoValidation()
        {
            RuleFor(t => t.Nome)
                .NotEmpty().WithMessage("O nome do jogo é obrigatório.")
                .MinimumLength(3).WithMessage("O nome deve ter pelo menos 3 caracteres.");

            RuleFor(j => j.Categoria)
                .NotEmpty().WithMessage("A categoria é obrigatória.")
                .MinimumLength(3).WithMessage("A categoria deve ter pelo menos 3 caracteres.");

            RuleFor(j => j.Classificacao)
                .IsInEnum().WithMessage("A classificação indicativa informada é inválida.");

        }
    }
}
