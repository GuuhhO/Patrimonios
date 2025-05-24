using System.ComponentModel.DataAnnotations;

namespace Patrimonios.Models
{
    public class PatrimoniosModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Número de Patrimônio é obrigátorio.")]
        public int NumeroPatrimonio { get; set; }

        public string? Categoria { get; set; }

        public string? Responsavel { get; set; }

        public string? Equipamento { get; set; }

        public string? Marca { get; set; }

        [Required(ErrorMessage = "Descrição é obrigátorio.")]
        public string Descricao { get; set; }

        public string? Modelo { get; set; }

        public string Localidade { get; set; }

        [Required(ErrorMessage = "Unidade é obrigátorio.")]
        public string Unidade { get; set; }

        [Required(ErrorMessage = "Setor é obrigátorio.")]
        public string Setor { get; set; }

        public string? EstadoConservacao { get; set; }

        public string? Status { get; set; }

        public string? NumeroSerie { get; set; }

        public string? NotaFiscal { get; set; }

        public string? DataAquisicao { get; set; }

        [Required(ErrorMessage = "Valor é obrigátorio.")]
        public string Valor { get; set; }
    }
}