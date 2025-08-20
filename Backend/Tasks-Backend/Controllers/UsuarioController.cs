using Microsoft.AspNetCore.Mvc;
using Tasks_Backend.DTOs;
using Tasks_Backend.Entities;
using Tasks_Backend.Services;

using Microsoft.AspNetCore.Authorization;

namespace Tasks_Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public UsuarioController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // [Authorize(Roles = "Administrador")]
        [AuthorizeAdmin]
        [HttpPost]
        public async Task<ActionResult<Usuario>> CriarUsuario([FromBody] UsuarioDto dto)
        {
            var usuario = await _usuarioService.CriarUsuario(dto);
            return CreatedAtAction(nameof(ObterUsuarioPorId), new { id = usuario.Id }, usuario);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> ObterUsuarioPorId(int id)
        {
            var usuario = await _usuarioService.ObterPorId(id);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        [HttpGet]
        public async Task<ActionResult<List<Usuario>>> ListarUsuarios()
        {
            var usuarios = await _usuarioService.ListarUsuarios();
            return Ok(usuarios);
        }
    }
}