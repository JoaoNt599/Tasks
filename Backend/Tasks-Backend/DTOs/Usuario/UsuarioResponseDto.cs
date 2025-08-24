using Tasks_Backend.enums;

namespace Tasks_Backend.DTOs
{
    public class UsuarioResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public Perfil Perfil { get; set; }
    }
}