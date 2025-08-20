using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Tasks_Backend.Services;
using Tasks_Backend.DTOs;

namespace Tasks_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public AuthController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] UsuarioDto dto)
        {
            var usuario = await _usuarioService.Autenticar(dto.Email, dto.Senha);
            if (usuario == null)
                return Unauthorized("Credenciais inválidas");

            var token = _usuarioService.GerarToken(usuario);
            return Ok(token);
        }
    }
}