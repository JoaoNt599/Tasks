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

        [AuthorizeAdmin]
        [HttpPost]
        public async Task<ActionResult<UsuarioResponseDto>> CriarUsuario([FromBody] UsuarioDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuario = await _usuarioService.CriarUsuario(dto);
                var usuarioResponseDto = new UsuarioResponseDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Perfil = usuario.Perfil
                };

                return CreatedAtAction(nameof(ObterUsuarioPorId), new { id = usuario.Id }, usuarioResponseDto);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Usuário não autenticado.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar novo usuário: {ex.Message}");
            }   
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioResponseDto>> ObterUsuarioPorId(int id)
        {
            var usuario = await _usuarioService.ObterPorId(id);
            if (usuario == null)
                return NotFound();

            var usuarioResponseDto = new UsuarioResponseDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil
            };

            return Ok(usuarioResponseDto);
        }

        [HttpGet]
        public async Task<ActionResult<List<UsuarioResponseDto>>> ListarUsuarios()
        {
            var usuarios = await _usuarioService.ListarUsuarios();

            var usuariosDto = usuarios.Select(u => new UsuarioResponseDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                Perfil = u.Perfil
            }).ToList();

            return Ok(usuariosDto);
        }

        [AuthorizeAdmin]
        [HttpPut("{id}")]
        public async Task<ActionResult<UsuarioResponseDto>> AtualizarUsuario(int id, [FromBody] UsuarioDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuarioAtualizado = await _usuarioService.AtualizarUsuario(id, dto);
                if (usuarioAtualizado == null)
                    return NotFound("Usuário não encontrado.");


                var usuarioResponseDto = new UsuarioResponseDto
                {
                    Id = usuarioAtualizado.Id,
                    Nome = usuarioAtualizado.Nome,
                    Email = usuarioAtualizado.Email,
                    Perfil = usuarioAtualizado.Perfil
                };

                return Ok(usuarioResponseDto);         
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Usuário não autenticado");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar usuário: {ex.Message}");
            }
        }
    }
}