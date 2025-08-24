using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasks_Backend.DTOs;
using Tasks_Backend.DTOs.Tarefa;
using Tasks_Backend.Entities;
using Tasks_Backend.enums;
using Tasks_Backend.Services;


namespace Tasks_Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly TarefaService _tarefaService;

        public TarefaController(TarefaService tarefaService)
        {
            _tarefaService = tarefaService;
        }

        [HttpPost]
        public async Task<ActionResult<TarefaResponseDto>> CriarTarefa([FromBody] TarefaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var tarefa = await _tarefaService.CriarTarefa(dto, User);
                var tarefaResponseDto = new TarefaResponseDto
                {
                    Id = tarefa.Id,
                    Titulo = tarefa.Titulo,
                    Descricao = tarefa.Descricao,
                    ResponsavelId = tarefa.ResponsavelId,
                    NomeResponsavel = tarefa.Responsavel.Nome,
                    Status = tarefa.Status,
                    DataCriacao = tarefa.DataCriacao
                };

                return CreatedAtAction(nameof(ObterTarefaPorId), new { id = tarefa.Id }, tarefaResponseDto);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Usuário não autenticado.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar nova tarefa: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<TarefaDto>>> ListarTarefas([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 5)
        {
            if (pagina <= 0 || tamanhoPagina <= 0)
                return BadRequest("Página e tamanho devem ser maiores que zero.");

            try
            {
                var tarefas = await _tarefaService.ListaPaginada(pagina, tamanhoPagina);
                return Ok(tarefas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao listar tarefas: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tarefa>> ObterTarefaPorId(int id)
        {
            if (id <= 0)
                return BadRequest("O ID da tarefa deve ser um número positivo.");

            try
            {
                var tarefa = await _tarefaService.BuscarPorId(id);

                if (tarefa == null)
                    return NotFound("Tarefa não encontrada.");

                return Ok(tarefa);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar tarefa: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TarefaDto>> AtualizarTarefa(int id, [FromBody] TarefaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0)
                return BadRequest("O ID da tarefa deve ser um número positivo.");

            try
            {
                var tarefaAtualizada = await _tarefaService.AtualizarTarefa(id, dto, User);
                if (tarefaAtualizada == null)
                    return NotFound("Tarefa não encontrada.");

                return Ok(tarefaAtualizada);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar tarefa: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarTarefa(int id)
        {

            if (id <= 0)
                return BadRequest("O ID da tarefa deve ser um número positivo.");

            try
            {
                var sucesso = await _tarefaService.DeletarTarefa(id, User);
                if (!sucesso)
                    return NotFound("Tarefa não encontrada.");

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao excluir tarefa: {ex.Message}");
            }
        }

        [HttpGet("filtro")]
        public async Task<ActionResult<List<TarefaDto>>> ListarPorStatus(
            [FromQuery] Status status,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 5)
        {
            if (pagina <= 0 || tamanhoPagina <= 0)
                return BadRequest("Página e tamanho devem ser maiores que zero.");

            try
            {
                var tarefas = await _tarefaService.FiltroPorStatus(status, pagina, tamanhoPagina);
                return Ok(tarefas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao filtrar tarefas por status: {ex.Message}");
            }
        }
    }
}