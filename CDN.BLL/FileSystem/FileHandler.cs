using Google.Protobuf;
using Grpc.Core;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CDN.BLL.FileSystem
{
    public class FileHandler
    {
        public FileHandler()
        {
          
        }

 
        public  System.Collections.Generic.IEnumerable<CDN.GRPC.protobuf.FileDetails> ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
           yield   return   ProcessFile(fileName);
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }

        public static CDN.GRPC.protobuf.FileDetails ProcessFile(string path)
        {
           var details=new CDN.GRPC.protobuf.FileDetails();
            details.Filepath = setRelativepath(path);
            details.MD5Hash = CalculateMD5(path);
            details.Content = ByteString.CopyFrom(new FileHandler().ReadFile(path)); ;
            return details;
        }
  
        public static string CalculateMD5(string path)
        {

            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(path))
                    {
                        var hash = md5.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
            }
            catch (Exception)
            {

                return string.Empty;
            }
            
        }

        internal async Task FileCompareByMD5AndreplaceAsync(CDN.GRPC.protobuf.FileDetails current)
        {
            var code = CalculateMD5( GetAbsolutePath(  current.Filepath));
            if (!code.Equals(current.MD5Hash))
            {
              RenameFile(null, current);
            }
            
           
        }

        public MemoryStream ByteStringToMemoryStream(ByteString content)
        {

            MemoryStream ms = new MemoryStream();
            content.WriteTo(ms);
            return ms;
        }

       

        public  byte[] ReadFile(string path)
        {
            var file = File.ReadAllBytes(path);
            return file;
        }


        public bool WriteFile(string path, ByteString content)
        {
            try
            {


                using (var ms = ByteStringToMemoryStream(content))
                {
                    System.IO.File.WriteAllBytes(GetAbsolutePath(path), ms.ToArray());
                }
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

        public void RenameFile(CDN.GRPC.protobuf.FileOnChangeData request, CDN.GRPC.protobuf.FileDetails response)
        {
            if (request != null)
            {
                DeleteFile(request.OldPath);
                WriteFile(request.NewPath, request.Content);
            }
            else
            {
                DeleteFile(response.Filepath);
                WriteFile(response.Filepath, response.Content);
            }
           
        }

        private string GetAbsolutePath(string filepath)
        {
            var path = BOD.SystemParameters.FileHostPath + filepath;
            return path.Replace("//", "/");
        }

        private static string setRelativepath(string filepath)
        {
            var path= filepath.Replace("//", "/");
            return path.Replace(BOD.SystemParameters.FileHostPath, string.Empty).Trim();
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
