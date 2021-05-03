using System;
using System.IO;

namespace CDN.BLL.FileSystem
{
    public class FileHandler
    {
        public FileHandler()
        {

        }

        public byte[] ReadFile(string path)
        {
            var file = File.ReadAllBytes(path);
            return file;
        }


        public bool WriteFile(string path, byte[] content)
        {
            try
            {
                System.IO.File.WriteAllBytes(path, content);
            }
            catch (Exception ex)
            {

                return false;
            }

            return true;
        }

        public void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

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
