using CDN.BLL.FileSystem;
using CDN.BLL.GRPC.FileSystem;
using CDN.BLL.Zookeeper;
using Google.Protobuf;
using System.Collections.Generic;
using System.IO;

namespace CDN.BLL.Services
{
    public class FileSync
    {
        private ZookeeperService zk;
        private List<GRPCClient_FileSystem> clients;

        public FileSync()
        {
            zk = new Zookeeper.ZookeeperService().GetZookeeperService();
            var nodes = zk.GetClusterNodes(BOD.NodeDetails.ClusterName);
            clients = CreateGRPCClients(nodes);
        }

        private List<GRPCClient_FileSystem> CreateGRPCClients(List<KeyValuePair<long, string>> lstSelected)
        {
            List<GRPCClient_FileSystem> clients = new List<GRPCClient_FileSystem>();
            foreach (var item in lstSelected)
            {
                if (item.Value != BOD.NodeDetails.Ip)
                {
                    var client = new GRPC.FileSystem.GRPCClient_FileSystem(item.Value, BOD.SystemPorts.Fileshare);
                    clients.Add(client);
                }
            }
            return clients;
        }

        private void SendRequestsToLstClients(object obj, BOD.StaticLists.FileOperations Operation)
        {
            foreach (var client in clients)
            {
                switch (Operation)
                {
                    case BOD.StaticLists.FileOperations.Change:
                    case BOD.StaticLists.FileOperations.Rename:
                    case BOD.StaticLists.FileOperations.Delete:
                        CDN.GRPC.protobuf.FileOnChangeData ChangedFile = (CDN.GRPC.protobuf.FileOnChangeData)obj;
                        client.Client.FileOnChange(ChangedFile);
                        break;
                    case BOD.StaticLists.FileOperations.Create:
                        CDN.GRPC.protobuf.FileOnCreateData CreatedFile = (CDN.GRPC.protobuf.FileOnCreateData)obj;
                        client.Client.FileOnCreate(CreatedFile);
                        break;
                }
            }
        }

        internal void OnFileChange(FileSystemEventArgs e)
        {

            var obj = new CDN.GRPC.protobuf.FileOnChangeData();
            obj.OldPath = GetRelativeFilePath(e.FullPath); ;

            obj.OperationType = BOD.StaticLists.FileOperations.Change.ToString();
            obj.Content = ByteString.CopyFrom(new FileHandler().ReadFile(e.FullPath.ToString()));
            SendRequestsToLstClients(obj, BOD.StaticLists.FileOperations.Change);
        }

        internal void OnFileCreated(FileSystemEventArgs e)
        {
            var obj = new CDN.GRPC.protobuf.FileOnChangeData();
            obj.NewFileName = e.Name;
            obj.NewPath = GetRelativeFilePath(e.FullPath); ;
            obj.OperationType = BOD.StaticLists.FileOperations.Create.ToString();
            obj.Content = ByteString.CopyFrom(new FileHandler().ReadFile(e.FullPath.ToString()));

            SendRequestsToLstClients(obj, BOD.StaticLists.FileOperations.Create);
        }

        private string GetRelativeFilePath(string path)
        {
            return path.Replace(BOD.SystemParameters.FileHostPath, "").Trim();
        }

        internal void OnFileDeleted(FileSystemEventArgs e)
        {
            var obj = new CDN.GRPC.protobuf.FileOnChangeData();
            obj.OldPath = GetRelativeFilePath(e.FullPath); ;
            obj.OperationType = BOD.StaticLists.FileOperations.Delete.ToString();
            SendRequestsToLstClients(obj, BOD.StaticLists.FileOperations.Delete);
        }

        internal void OnFileRenamed(RenamedEventArgs e)
        {
            var obj = new CDN.GRPC.protobuf.FileOnChangeData();
            obj.NewPath = GetRelativeFilePath(e.FullPath); ;
            obj.OldPath = GetRelativeFilePath(e.OldFullPath);
            obj.OldFileName = e.OldName;
            obj.NewFileName = e.Name;
            obj.OperationType = BOD.StaticLists.FileOperations.Rename.ToString();
            SendRequestsToLstClients(obj, BOD.StaticLists.FileOperations.Rename);
        }
    }
}
