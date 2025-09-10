using System.ComponentModel.DataAnnotations;

namespace RestApiCase.Api.Requests
{
    public class CreateTask
    {
        [Required(ErrorMessage = "Titulo é obrigatório")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Descrição é obrigatório")]
        [MaxLength(500, ErrorMessage = "Descrição muito longa")]
        public string Description { get; set; } = "";

        [MaxLength(100, ErrorMessage = "Resumo muito longo")]
        public string Summary { get; set; } = "";

        [Required(ErrorMessage = "É obrigatório ter uma data")]
        public DateTime? DueDate { get; set; }
    }
}