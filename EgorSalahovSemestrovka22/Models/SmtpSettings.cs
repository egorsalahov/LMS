namespace EgorSalahovSemestrovka22.Models
{
    public class SmtpSettings
    {
        public SmtpConfig Primary { get; set; }
        public SmtpConfig Fallback { get; set; }
    }

    public class SmtpConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
    }
}
