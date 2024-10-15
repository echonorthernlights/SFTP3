namespace SFTP3.Configurations
{
    public class SftpConnectionInfo
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PrivateKeyPath { get; set; }
        public string Passphrase { get; set; }
    }

}
