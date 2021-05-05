using Google.Protobuf;
using System;
using System.IO;

namespace CDN.BLL.FileSystem
{
    public class FileHandler
    {
        public FileHandler()
        {

        }
        public MemoryStream ByteStringToMemoryStream(ByteString content)
        {
            //byte[] bytes =new  byte[];
            MemoryStream ms = new MemoryStream();
            content.WriteTo(ms);
            return ms;
        }
        public byte[] ReadFile(string path)
        {
            var file = File.ReadAllBytes(path);
            return file;
        }


        public bool WriteFile(string path,ByteString content)
        {
            try
            {
                var ms = ByteStringToMemoryStream(content);
                System.IO.File.WriteAllBytes(GetAbsolutePath(path), ms.ToArray());
            }
            catch (Exception ex)
            {

                return false;
            }

            return true;
        }

        public void DeleteFile(string path)
        {
            if (File.Exists(GetAbsolutePath(path)))
            {
                File.Delete(GetAbsolutePath(path));
            }
          
        }

        public void RenameFile(CDN.GRPC.protobuf.FileOnChangeData request)
        {
            DeleteFile(request.OldPath);
            WriteFile(request.NewPath, request.Content);
        }

        private string GetAbsolutePath(string filepath)
        {
            var path = BOD.SystemParameters.FileHostPath + filepath;
            return path.Replace("//","/");
        }
        public bool CreateDirectory(string path)
        {
            try
            {
                String folder = Path.GetDirectoryName(path);
                if (!Directory.Exists(folder))
                {

                    DirectoryInfo di = Directory.CreateDirectory(folder);
                }
            }
            catch (IOException ex)
            {
                return false;
            }
            return true;
        }
    }
}
