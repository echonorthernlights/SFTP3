using Renci.SshNet;
using SFTP3.Configurations;
using ConnectionInfo = Microsoft.AspNetCore.Http.ConnectionInfo;

namespace SFTP3.Factories
{
    public class SftpClientFactory
    {
        public static (SftpClient, bool) CreateSftpClient(SftpConnectionInfo connectionInfo)
        {
            SftpClient client = null;

            try
            {
                if (!string.IsNullOrEmpty(connectionInfo.PrivateKeyPath))
                {
                    var keyFile = new PrivateKeyFile(connectionInfo.PrivateKeyPath, connectionInfo.Passphrase);
                    var keyAuthMethod = new PrivateKeyAuthenticationMethod(connectionInfo.Username, keyFile);
                    var connInfo = new Renci.SshNet.ConnectionInfo(connectionInfo.Host, connectionInfo.Port, connectionInfo.Username, keyAuthMethod);

                    client = new SftpClient(connInfo);
                }
                else if (!string.IsNullOrEmpty(connectionInfo.Password))
                {
                    client = new SftpClient(connectionInfo.Host, connectionInfo.Port, connectionInfo.Username, connectionInfo.Password);
                }
                else
                {
                    throw new ArgumentException("Invalid authentication method provided.");
                }

                client.Connect();
                return (client, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                client?.Dispose();
                return (null, false);
            }
        }
    }

}
