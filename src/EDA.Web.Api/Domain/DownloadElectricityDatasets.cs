using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Net;
using System.Reflection.Metadata;
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
            using var client                = new HttpClient();
            //using var responseStream        = client.GetStreamAsync(fileUrl).Result;
            using var response              = client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead).Result;
            using var responseStream        = response.Content.ReadAsStream();
            using var destinationFileStream = new FileStream(destinationFilePath, FileMode.CreateNew);

            responseStream.CopyTo(destinationFileStream);
            return new ActionResult<string>(destinationFilePath);
            return ActionResult.Ok(destinationFilePath);
        }
    }
    public class DownloadFileToFolderAction
    {
        public string FileUrl { get; set; }
        public string DestinationFilePath { get; set; }
    }

    public class ActionResult<TContent>
    {
        public ActionResult(bool status) : this(status, default)
        {
            
        }
        public ActionResult(TContent content) : this(true, content)
        {
            
        }
        public ActionResult(string error) : this(false, default, error)
        {
            
        }
        public ActionResult(bool successed, TContent content, params string[] errors)
        {
            Content = content;
            Errors = errors;
            Successed = successed;
        }
        public bool Successed { get; set; }
        public TContent Content { get; set; }
        public string[] Errors { get; set; }
    }

    public static class ActionResult
    {
        public static ActionResult<TContent> Ok<TContent>(TContent content)
        {
            return new ActionResult<TContent>(content);
        }

        public static ActionResult<TContent> Error<TContent>(string error)
        {
            return new ActionResult<TContent>(error);
        }
    }

    public interface IAction<TContent>
    {
        ActionResult<TContent> Do();
    }
}

