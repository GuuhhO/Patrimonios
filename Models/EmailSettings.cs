namespace Patrimonios.Models
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }

        public int SmtpPort { get; set; }

        public string UserEmail { get; set; }

        public string Password { get; set; }
    }
}
