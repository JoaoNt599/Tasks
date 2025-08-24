using Tasks_Backend.enums;

namespace Tasks_Backend.DTOs.Tarefa
{
    public class TarefaResponseDto
    {

        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public int ResponsavelId { get; set; }
        public string NomeResponsavel { get; set; }
        public Status Status { get; set; }
        public DateTime? DataCriacao { get; set; }
    }
}