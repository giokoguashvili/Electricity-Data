using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Runtime.InteropServices.ObjectiveC;


namespace EDA.Web.Api.Domain
{
    public class DownloadElectricityDatasets : IAction<string>
    {
        private readonly string[] urls;
        private readonly string destinationFolderPath;

        public DownloadElectricityDatasets(string[] urls, string destinationFolderPath)
        {
            this.urls = urls;
            this.destinationFolderPath = destinationFolderPath;
        }

        public ActionResult<string> Do()
        {
            var runningTasks = urls
                        .Select(url =>
                        {
                            var uri = new Uri(url);
                            var fileName = uri.Segments.Last();
                            var filePath = Path.Combine(destinationFolderPath, fileName);
                            return new { url = url, filePath = filePath };
                        })
                        .Select(t =>
                        {
                            return Task.Run(() =>
                            {
                                ActionResult<string> result = null;
                                try
                                {
                                    result = new DownloadFileToFolder(t.url, t.filePath).Do();
                                }
                                catch (Exception e)
                                {
                                    result = new ActionResult<string>(false);
                                }
                                return result;
                            });
                        })
                        .ToList();
                Task.WaitAll(runningTasks.ToArray());
            return new ActionResult<string>(destinationFolderPath);
        }
    }

    public class DownloadFileToFolder : IAction<string>
    {
        private readonly string fileUrl;
        private readonly string destinationFilePath;

        public DownloadFileToFolder(string fileUrl, string destinationFilePath)
        {
            this.fileUrl = fileUrl;
            this.destinationFilePath = destinationFilePath;
        }
        public ActionResult<string> Do()
        {
            HttpClient client = new HttpClient();
            var uri = new Uri(fileUrl);
            var response = client.GetAsync(uri).Result;

            response.EnsureSuccessStatusCode();

            using (var fs = new FileStream(destinationFilePath, FileMode.CreateNew))
            {
                response.Content.CopyTo(fs, null, CancellationToken.None);
            }

            return new ActionResult<string>(destinationFilePath);
        }
    }

    public class ActionResult<TContent>
    {
        public ActionResult(bool status) : this(status, default)
        {
            
        }
        public ActionResult(TContent content) : this(true, content)
        {
            
        }
        public ActionResult(bool successed, TContent content)
        {
            Content = content;
            Successed = successed;
        }
        public bool Successed { get; set; }
        public TContent Content { get; set; }
    }

    public interface IAction<TContent>
    {
        ActionResult<TContent> Do();
    }
}
