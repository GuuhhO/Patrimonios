using System.ComponentModel.DataAnnotations;

namespace Patrimonios.Models
{
    public class LogEntry
    {
        [Key]
        public int Id { get; set; }

        public string Usuario { get; set; }

        public string Acao { get; set; }

        public DateTime Timestamp { get; set; }

        public string Descricao { get; set; }
    }
}
