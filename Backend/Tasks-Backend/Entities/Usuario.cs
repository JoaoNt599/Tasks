using Tasks_Backend.enums;

using System.ComponentModel.DataAnnotations;


namespace Tasks_Backend.Entities
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string SenhaHash { get; set; }

        [Required]
        public Perfil Perfil { get; set; }
    }
}