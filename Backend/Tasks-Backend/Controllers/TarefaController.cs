using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasks_Backend.DTOs;
using Tasks_Backend.Entities;
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
        public async Task<ActionResult<Tarefa>> CriarTarefa([FromBody] TarefaDto dto)
        {
            try
            {
                var tarefa = await _tarefaService.CriarTarefa(dto, User);
                return CreatedAtAction(nameof(ObterTarefaPorId), new { id = tarefa.Id }, tarefa);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Usuário não autenticado.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar nova tarafa: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<TarefaDto>>> ListarTarefas()
        {
            var tarafas = await _tarefaService.ListarTarefas();
            return Ok(tarafas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tarefa>> ObterTarefaPorId(int id)
        {
            var tarafa = await _tarefaService.BuscarPorId(id);

            if (tarafa == null)
                return NotFound("Tarefa não encontrada.");

            return Ok(tarafa);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TarefaDto>> AtualizarTarefa(int id, [FromBody] TarefaDto dto)
        {
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
    }
}