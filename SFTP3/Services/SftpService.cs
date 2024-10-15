using Renci.SshNet;
using SFTP3.Configurations;
using SFTP3.Factories;

namespace SFTP3.Services
{
    public class SftpService
    {
        private readonly SftpClient _receiveClient;
        private readonly SftpClient _sendClient;

        public SftpService(SftpConnectionInfo receiveConnectionInfo, SftpConnectionInfo sendConnectionInfo)
        {
            var (receiveClient, receiveSuccess) = SftpClientFactory.CreateSftpClient(receiveConnectionInfo);
            var (sendClient, sendSuccess) = SftpClientFactory.CreateSftpClient(sendConnectionInfo);

            if (!receiveSuccess || !sendSuccess)
            {
                throw new Exception("Failed to establish SFTP connections.");
            }

            _receiveClient = receiveClient;
            _sendClient = sendClient;
        }

        public void ReceiveAndSendFiles(string remoteReceivePath, string localPath, string remoteSendPath)
        {
            try
            {
                // Receive files from receiveClient
                var files = _receiveClient.ListDirectory(remoteReceivePath);
                foreach (var file in files)
                {
                    if (!file.IsDirectory)
                    {
                        using (var fileStream = File.OpenWrite(Path.Combine(localPath, file.Name)))
                        {
                            _receiveClient.DownloadFile(file.FullName, fileStream);
                        }

                        // Upload the file to sendClient
                        using (var fileStream = File.OpenRead(Path.Combine(localPath, file.Name)))
                        {
                            _sendClient.UploadFile(fileStream, Path.Combine(remoteSendPath, file.Name));
                        }
                    }
                }
            }
            finally
            {
                _receiveClient.Disconnect();
                _sendClient.Disconnect();
            }
        }
    }

}
