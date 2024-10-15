using Microsoft.AspNetCore.Mvc;
using Renci.SshNet;
using System;
using System.IO;

[ApiController]
[Route("api/[controller]")]
public class SftpController : ControllerBase
{
    private SftpClient CreateSftpClient(string host, int port, string username, string password)
    {
        var client = new SftpClient(host, port, username, password);
        client.Connect();
        return client;
    }

    [HttpPost("transfer")]
    public IActionResult TransferFiles()
    {
        try
        {
            var receiverClient = CreateSftpClient("192.168.3.114", 23, "receiver", "root1234");  // Client for downloading files
            var senderClient = CreateSftpClient("192.168.3.114", 22, "sender", "root1234");  // Client for uploading files

            // Ensure both clients are connected
            if (!receiverClient.IsConnected || !senderClient.IsConnected)
            {
                throw new Exception("One or both SFTP clients failed to connect.");
            }

            var sourceDirectory = "/source";  // Directory on receiver server
            var destinationDirectory = "/destination";  // Directory on sender server

            var filesToTransfer = receiverClient.ListDirectory(sourceDirectory);
            foreach (var file in filesToTransfer)
            {
                if (!file.IsDirectory)
                {
                    Console.WriteLine($"Transferring file: {file.FullName}");

                    using (var memoryStream = new MemoryStream())
                    {
                        receiverClient.DownloadFile(file.FullName, memoryStream);
                        memoryStream.Position = 0;  // Reset stream position before uploading
                        senderClient.UploadFile(memoryStream, Path.Combine(destinationDirectory, file.Name));
                    }

                    Console.WriteLine($"Transferred file: {file.FullName}");
                }
            }

            receiverClient.Disconnect();
            senderClient.Disconnect();

            return Ok("Files transferred successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
