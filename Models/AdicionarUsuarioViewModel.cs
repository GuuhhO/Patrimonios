using System.ComponentModel.DataAnnotations;

namespace Patrimonios.Models
{
    public class AdicionarUsuarioViewModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "E-mail é obrigatório.")]
        public string UserName { get; set; }

        [Compare("UserName", ErrorMessage = "Os e-mails não coincidem.")]
        [Required(ErrorMessage = "Confirmação de e-mail é obrigatório.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefone é obrigatório.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Senha é obrigatório.")]
        [StringLength(20, ErrorMessage = "The {0} deve ser pelo menos {2} o no máximo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmação de senha é obrigatório.")]
        [Compare("Password", ErrorMessage = "As senhas não coincidem.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmação de senha")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Grupo é obrigatório.")]
        public int Grupo { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        public string Name {  get; set; }
    }
}
