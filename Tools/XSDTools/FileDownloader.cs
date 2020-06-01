using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace XSDTools
{
    public class FileDownloader : IDisposable
    {
        private readonly WebClient client;

        public FileDownloader()
        {
            client = new WebClient();
        }

        public string Download(string filePath, string targetPath)
        {
            var fileName = Path.GetFileName(filePath);
            var downloadFilePath = Path.Combine(targetPath, fileName);
            client.DownloadFile(filePath, downloadFilePath);
            return downloadFilePath;
        }

        public void DownloadAs(string filePath, string targetFilePath)
        {
            client.DownloadFile(filePath, targetFilePath);
        }

        public List<string> Download(List<string> filePaths, string targetPath)
        {
            var files = new List<string>();
            foreach (var filePath in filePaths)
            {
                files.Add(Download(filePath, targetPath));
            }
            return files;
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}