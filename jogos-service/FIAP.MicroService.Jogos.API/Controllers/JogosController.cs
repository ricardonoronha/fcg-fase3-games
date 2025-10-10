using FIAP.MicroService.Jogos.API.DTOs;
using FIAP.MicroService.Jogos.Dominio.Interfaces.Service;
using FIAP.MicroService.Jogos.Dominio.Models;
using Microsoft.AspNetCore.Mvc;
using OpenSearch.Client;

namespace FIAP.MicroService.Jogos.API.Controllers;

[ApiController]
[Route("api/games")]
public class JogosController : ControllerBase
{
    private readonly IJogoService _jogoService;
    private readonly ILogger<JogosController> _logger;

    public JogosController(IJogoService jogoService, ILogger<JogosController> logger)
    {
        this._jogoService = jogoService;
        this._logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Jogo>>> GetAll()
    {
        var jogos = await _jogoService.GetAllAsync();
        
        _logger.LogInformation("Todos os jogos foram listados");
        
        return Ok(jogos);
    }

    [HttpGet("{gameId:guid}")]
    public async Task<IActionResult> GetById(Guid gameId)
    {
        var jogo = await _jogoService.GetByIdAsync(gameId);

        if (jogo == null)
            return NotFound($"Jogo com ID {gameId} não encontrado.");

        _logger.LogInformation("Jogo pesquisado | JogoId: {JogoId}", gameId);

        return Ok(jogo);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CriacaoJogoDTO dto)
    {
        var jogo = new Jogo()
        {
            Nome = dto.Nome,
            Categoria = dto.Categoria,
            Classificacao = dto.Classificacao,
            Preco = dto.Preco,
            DataLancamento = dto.DataLancamento
        };

        var id = await _jogoService.AddAsync(jogo);

        _logger.LogInformation("Jogo adicionado | JogoId: {JogoId}", id);

        return Ok(jogo.Id);
    }

    [HttpPut("{gameId:guid}")]
    public async Task<IActionResult> Update(Guid gameId, [FromBody] AtualizarJogoDTO dto)
    {
        var jogoExistente = await _jogoService.GetByIdAsync(gameId);

        if (jogoExistente == null)
            return NotFound($"Jogo com ID {gameId} não encontrado.");

        jogoExistente.Nome = dto.Nome != null ? dto.Nome : jogoExistente.Nome;
        jogoExistente.Classificacao = dto.Classificacao;
        jogoExistente.Categoria = dto.Categoria != null ? dto.Categoria : jogoExistente.Categoria;
        jogoExistente.Preco = dto.Preco;
        jogoExistente.DataLancamento = dto.DataLancamento.HasValue ? dto.DataLancamento.Value : jogoExistente.DataLancamento;

        var atualizado = await _jogoService.UpdateAsync(jogoExistente);
        if (atualizado == null)
            return NotFound($"Jogo com ID {gameId} não encontrado.");

        _logger.LogInformation("Jogo atualizado | JogoId: {JogoId}", gameId);

        return Ok(atualizado);
    }

    [HttpDelete("{gameId:guid}")]
    public async Task<IActionResult> Delete(Guid gameId)
    {
        var sucesso = await _jogoService.DeleteAsync(gameId);
        if (!sucesso)
            return NotFound($"Jogo com ID {gameId} não encontrado ou não foi possível excluir.");

        _logger.LogInformation("Jogo excluído | JogoId: {JogoId}", gameId);

        return NoContent();
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> MostGames([FromQuery]string textSearch, [FromQuery] int page=1, [FromQuery] int pageSize=10)
    {
        var resultados = await _jogoService.SearchAsync(textSearch, page, pageSize);

        _logger.LogInformation("Consulta de jogos realizada | SearchedText: {SearchedText}, Page: {Page}, PageSize: {PageSize}", textSearch, page, pageSize);

        return Ok(resultados);
    }

    [HttpGet("top")]
    public async Task<IActionResult> Suggest([FromBody] IEnumerable<string> categoriasHistorico, [FromQuery] int tamanho = 5)
    {
        var sugeridos = await _jogoService.SuggestGamesAsync(categoriasHistorico, tamanho);


        return Ok(sugeridos);
    }
}