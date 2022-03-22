using Google.Apis.Drive.v3;
using System;
using System.IO;

namespace DriveServices
{
    public class FileService
    {
        private readonly DriveService _service;
        private readonly Google.Apis.Drive.v3.Data.File _file;
        public FileService(DriveService service, Google.Apis.Drive.v3.Data.File file)
        {
            _service = service;
            _file = file;
        }
        private void DownloadFile(string saveTo)
        {
            var request = _service.Files.Get(_file.Id);
            var stream = new MemoryStream();

            // Add a handler which will be notified on progress changes.
            // It will notify on each chunk download and when the
            // download is completed or failed.
            request.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case Google.Apis.Download.DownloadStatus.Downloading:
                        {
                            Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                    case Google.Apis.Download.DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download complete.");
                            SaveStream(stream, saveTo);
                            break;
                        }
                    case Google.Apis.Download.DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
                            break;
                        }
                }
            };

            request.Download(stream);
        }

        private static void SaveStream(MemoryStream stream, string saveTo)
        {
            using FileStream file = new FileStream(saveTo, FileMode.Create, FileAccess.Write);
            stream.WriteTo(file);
        }

        private void ConnectToDrive()
        {
            // Connect to Google
            var service = GoogleSamplecSharpSample.Drivev3.Auth.Oauth2Example.AuthenticateOauth(@"[Location of the Json cred file]", "test");
            //List the files with the word 'make' in the name.
            var files = DriveListExample.ListFiles(service,
                             new DriveListExample.FilesListOptionalParms() { Q = "name contains 'Make'", Fields = "*" });
            foreach (var item in files.Files)
            {
                // download each file
                DriveDownload.Download(service, item, string.Format(@"C:\FilesFromGoogleDrive\{0}", item.Name));
            }
        }
    }
}
