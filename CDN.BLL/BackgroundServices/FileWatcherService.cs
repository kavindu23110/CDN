using CDN.BLL.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CDN.BLL.BackgroundServices
{
    public class FileWatcherService : BackgroundService
    {
        private FileSystemWatcher watcher;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
          
                watcher = new System.IO.FileSystemWatcher();
                watcher.Path = BOD.SystemParameters.FileHostPath;
                watcher.IncludeSubdirectories = true;
                watcher.EnableRaisingEvents = true;
                watcher.NotifyFilter = NotifyFilters.Attributes |
                NotifyFilters.CreationTime |
                NotifyFilters.DirectoryName |
                NotifyFilters.FileName |
                NotifyFilters.LastAccess |
                NotifyFilters.LastWrite |
                NotifyFilters.Security |
                 NotifyFilters.Size;
                watcher.Filter = "*.*";
                watcher.Changed += new FileSystemEventHandler(OnChanged);
                watcher.Created += new FileSystemEventHandler(OnCreated);
                watcher.Deleted += new FileSystemEventHandler(OnDeleted);
               watcher.Renamed += new RenamedEventHandler(OnRenamed);
            
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
        if (BOD.NodeDetails.Ip == BOD.NodeDetails.LeaderNode)
        {
                FileSync fileSync = new FileSync();
                fileSync.OnFileRenamed(e);
              
            
       }
    }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            if (BOD.NodeDetails.Ip == BOD.NodeDetails.LeaderNode)
            {

                FileSync fileSync = new FileSync();
                fileSync.OnFileDeleted(e);
            }
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (BOD.NodeDetails.Ip == BOD.NodeDetails.LeaderNode)
            {
                FileSync fileSync = new FileSync();
                fileSync.OnFileCreated(e);
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (BOD.NodeDetails.Ip == BOD.NodeDetails.LeaderNode)
            {
                FileSync fileSync = new FileSync();
                fileSync.OnFileChange(e);
            }
        }


    }
}
