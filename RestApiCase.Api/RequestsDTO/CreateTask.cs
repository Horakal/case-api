using System.ComponentModel.DataAnnotations;

namespace RestApiCase.Api.Requests
{
    public class CreateTask
    {
        [Required(ErrorMessage = "Titulo é obrigatório")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Descrição é obrigatório")]
        public string Description { get; set; } = "";

        [Required(ErrorMessage = "Resumo é obrigatório")]
        public string Summary { get; set; } = "";

        [Required(ErrorMessage = "É obrigatório ter uma data")]
        public DateTime? DueDate { get; set; }
    }
}