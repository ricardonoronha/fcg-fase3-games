using Microsoft.AspNetCore.Mvc;
using FIAP.MicroService.Jogos.Dominio.Models;
using FIAP.MicroService.Jogos.Dominio.Interfaces.Service;

namespace FIAP.MicroService.Jogos.API.Controllers;

[ApiController]
[Route("api/games")]
public class JogosController : ControllerBase
{
    private readonly IJogoService _jogoService;

    public JogosController(IJogoService jogoService)
    {
        this._jogoService = jogoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Jogo>>> GetAll()
    {
        var jogos = await _jogoService.GetAllAsync();
        return Ok(jogos);
    }

    [HttpGet("{gameId:guid}")]
    public async Task<IActionResult> GetById(Guid gameId)
    {
        var jogo = await _jogoService.GetByIdAsync(gameId);
        if (jogo == null)
            return NotFound($"Jogo com ID {gameId} não encontrado.");

        return Ok(jogo);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Jogo jogo)
    {
        var id = await _jogoService.AddAsync(jogo);
        return CreatedAtAction(nameof(GetById), new { id }, jogo);
    }

    [HttpPut("{gameId:guid}")]
    public async Task<IActionResult> Update(Guid gameId, [FromBody] Jogo jogoAtualizado)
    {
        if (gameId != jogoAtualizado.Id)
            return BadRequest("ID da URL não corresponde ao ID do jogo.");

        var atualizado = await _jogoService.UpdateAsync(jogoAtualizado);
        if (atualizado == null)
            return NotFound($"Jogo com ID {gameId} não encontrado.");

        return Ok(atualizado);
    }

    [HttpDelete("{gameId:guid}")]
    public async Task<IActionResult> Delete(Guid gameId)
    {
        var sucesso = await _jogoService.DeleteAsync(gameId);
        if (!sucesso)
            return NotFound($"Jogo com ID {gameId} não encontrado ou não foi possível excluir.");

        return NoContent();
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> MostGames([FromQuery] int top = 5)
    {
        var resultados = await _jogoService.MostPopularGamesAsync(top);
        return Ok(resultados);
    }

    [HttpGet("top")]
    public async Task<IActionResult> Suggest([FromBody] IEnumerable<string> categoriasHistorico, [FromQuery] int tamanho = 5)
    {
        var sugeridos = await _jogoService.SuggestGamesAsync(categoriasHistorico, tamanho);
        return Ok(sugeridos);
    }
}