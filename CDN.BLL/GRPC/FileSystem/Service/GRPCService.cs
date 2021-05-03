using CDN.BLL.FileSystem;
using CDN.GRPC.protobuf;
using Google.Protobuf;
using Grpc.Core;
using System.Threading.Tasks;

namespace CDN.BLL.GRPC.FileSystem.Service
{
    public class GRPCService_FileSystem : CDN.GRPC.protobuf.FileSystemGRPC.FileSystemGRPCBase
    {

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
                        break;
                    case "Delete":
                        break;
                }
            });
            return Task.FromResult(new Response());
        }


        public override Task<Response> FileOnCreate(FileOnCreateData request, ServerCallContext context)
        {
            Task.Run(() =>
            {
                var fs = new FileHandler();
                var arr = ByteStringTobyteArray(request.Content);
                fs.WriteFile(GetAbsolutePath(request.Filepath), arr);

            });
            return Task.FromResult(new Response());
        }



        public override Task FileSystemOnStart(IAsyncStreamReader<FileDetailsMD5> requestStream, IServerStreamWriter<FileDetails> responseStream, ServerCallContext context)
        {
            return base.FileSystemOnStart(requestStream, responseStream, context);
        }

        private string GetAbsolutePath(string filepath)
        {
            return BOD.SystemParameters.FileHostPath + filepath;
        }

        private byte[] ByteStringTobyteArray(ByteString content)
        {
            byte[] bytes = null;
            content.CopyTo(bytes, 0);
            return bytes;
        }
    }
}

