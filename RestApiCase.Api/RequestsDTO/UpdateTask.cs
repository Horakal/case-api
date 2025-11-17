using System.ComponentModel.DataAnnotations;

namespace RestApiCase.Api.Requests
{
    public class UpdateTask
    {
        public string? Title { get; set; } = null!;

        [MaxLength(500, ErrorMessage = "Descrição muito longa")]
        public string? Description { get; set; }

        [MaxLength(100, ErrorMessage = "Resumo muito longo")]
        public string? Summary { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskStatus? Status { get; set; }
    }
}