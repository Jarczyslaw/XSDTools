using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace XSDTools
{
    public class FileDownloader : IDisposable
    {
        private readonly WebClient client;

        public FileDownloader()
        {
            client = new WebClient();
        }

        public async Task<string> Download(string filePath, string targetPath)
        {
            var fileName = Path.GetFileName(filePath);
            var downloadFilePath = Path.Combine(targetPath, fileName);
            await client.DownloadFileTaskAsync(filePath, downloadFilePath);
            return downloadFilePath;
        }

        public Task DownloadAs(string filePath, string targetFilePath)
        {
            return client.DownloadFileTaskAsync(filePath, targetFilePath);
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}