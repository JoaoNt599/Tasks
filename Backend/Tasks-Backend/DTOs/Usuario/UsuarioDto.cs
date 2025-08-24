using System.ComponentModel.DataAnnotations;
using Tasks_Backend.enums;

namespace Tasks_Backend.DTOs
{
    public class UsuarioDto
    {
        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Senha { get; set; } = string.Empty;

        [Required]
        public Perfil Perfil { get; set; }
    }
}