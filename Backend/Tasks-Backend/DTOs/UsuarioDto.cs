using Tasks_Backend.enums;

namespace Tasks_Backend.DTOs
{
    public class UsuarioDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public Perfil Perfil { get; set; }
    }
}