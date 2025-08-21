using Tasks_Backend.enums;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations.Schema;


namespace Tasks_Backend.Entities
{
    public class Tarefa
    {
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Descricao { get; set; }

        [Required]
        public int ResponsavelId { get; set; }

        [ForeignKey("ResponsavelId")]
        public Usuario Responsavel { get; set; }

        [Required]
        public Status Status { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; }


        public Tarefa()
        {
            DataCriacao = DateTime.UtcNow;
        }
    }
}