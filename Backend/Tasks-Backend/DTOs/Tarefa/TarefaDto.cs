using System.ComponentModel.DataAnnotations;
using Tasks_Backend.enums;

namespace Tasks_Backend.DTOs.Tarefa
{
    public class TarefaDto
    {
        public int? Id { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        [Required]
        public int ResponsavelId { get; set; }

        public string NomeResponsavel { get; set; }

        public string EmailResponsavel { get; set; }

        public Status Status { get; set; }

        public DateTime? DataCriacao { get; set; }
    }
}