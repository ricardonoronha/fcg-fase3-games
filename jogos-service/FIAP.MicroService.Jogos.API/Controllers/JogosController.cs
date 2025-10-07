using Microsoft.AspNetCore.Mvc;
using FIAP.MicroService.Jogos.Dominio.Models;
using FIAP.MicroService.Jogos.Dominio.Interfaces;

namespace FIAP.MicroService.Jogos.API.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class JogosController(IJogoService service) : ControllerBase
    {
        [HttpGet("{gameId}")]
        public async Task<IActionResult> GetById(Guid gameId)
        {
            var Jogo = new Jogo() { Id = gameId, Nome = "Game Test" };

            await Task.CompletedTask;

            return Ok(Jogo);

        }

        [HttpGet] 
        public async Task<ActionResult<IEnumerable<Jogo>>> GetAll() 
        {
            var jogos = await service.ObtenhaTodosJogos();
            return Ok(jogos);
        }
        
        [HttpPost] // POST /api/games
        public async Task<IActionResult> Post(Jogo jogo)
        {
            await service.CriarJogo(jogo); 
            return CreatedAtAction(nameof(GetById), new { gameId = jogo.Id }, jogo);
        }
        
        [HttpGet("search")]
        public async Task<ActionResult<ResultadoBusca<Jogo>>> Search([FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrEmpty(q))
            {
                return BadRequest("O parâmetro de busca 'q' é obrigatório."); 
            }
            
            var resultado = await service.SearchGamesAsync(q, page, pageSize);
            
            return Ok(resultado);
        }
        
        [HttpGet("top")]
        public async Task<ActionResult<ResultadoAgregado>> GetTop([FromQuery] string by, [FromQuery] string window = "30d")
        {
            if (string.IsNullOrEmpty(by) || !(by.Equals("genre", StringComparison.OrdinalIgnoreCase) || by.Equals("game", StringComparison.OrdinalIgnoreCase) || by.Equals("revenue", StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("O parâmetro 'by' deve ser 'genre', 'game' ou 'revenue'.");
            }
            
            var resultados = await service.GetTopAggregatesAsync(by, window);
            return Ok(resultados);
        }
    }
}
