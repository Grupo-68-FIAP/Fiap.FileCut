namespace Fiap.FileCut.Core.Objects
{
    public class SmtpProperties(string server, int port, string username, string password)
    {
        public string Server { get; set; } = server;
        public int Port { get; set; } = port;
        public string Username { get; set; } = username;
        public string Password { get; set; } = password;
    }
}
