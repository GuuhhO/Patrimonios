using System.ComponentModel.DataAnnotations;

namespace Patrimonios.Models
{
    public class EditarUsuarioViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "E-mail é obrigatório.")]
        [EmailAddress]
        public string UserName { get; set; }

        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Grupo é obrigatório.")]
        public int Grupo { get; set; }
    }
}
