using CDN.BLL.FileSystem;
using CDN.GRPC.protobuf;
using Google.Protobuf;
using Grpc.Core;
using System.IO;
using System.Threading.Tasks;

namespace CDN.BLL.GRPC.FileSystem.Service
{
    public class GRPCService_FileSystem : CDN.GRPC.protobuf.FileSystemGRPC.FileSystemGRPCBase
    {
        public FileHandler FileSystem { get; private set; }

        public GRPCService_FileSystem()
        {
            FileSystem = new BLL.FileSystem.FileHandler();
        }

        public override Task<Response> FileOnChange(FileOnChangeData request, ServerCallContext context)
        {
            Task.Run(() =>
            {
                var fs = new FileHandler();
                switch (request.OperationType)
                {
                    case "Change":
                        break;
                    case "Rename":
                        FileSystem.RenameFile(request);
                        break;
                    case "Delete":
                        FileSystem.DeleteFile(request.OldPath);
                        break;
                }
            });
            return Task.FromResult(new Response());
        }


        public override Task<Response> FileOnCreate(FileOnCreateData request, ServerCallContext context)
        {
            Task.Run(() =>
            {

                FileSystem.WriteFile(request.Filepath,request.Content);

            });
            return Task.FromResult(new Response());
        }



        public override async Task FileSystemOnCheck(FileDetails request, IServerStreamWriter<FileDetails> responseStream, ServerCallContext context)
        {
            var fh = new FileHandler();

            var obj = fh.ProcessDirectory(BOD.SystemParameters.FileHostPath).GetEnumerator();
            while (obj.MoveNext())
            {
            await  responseStream.WriteAsync(obj.Current);
            }

       

        }

    }
}

