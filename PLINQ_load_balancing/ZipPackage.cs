using System.Collections.Generic;
using System.IO;

namespace PLINQ_load_balancing
{
    public class ZipPackage
    {
        public string ZipPath { get; set; }

        public readonly List<FileInZip> Files;

        public ZipPackage(string zipPath)
        {
            ZipPath = zipPath;       
            Files = new List<FileInZip>();
            Dictionary<string, long> fileList = UnZipper.GetFileList(ZipPath);
            foreach (var item in fileList)
                Files.Add(new FileInZip(ZipPath, item.Key, item.Value));            
        }

        public void UnzipFile(string fileName, string destPath)
        {
            UnZipper.ExtractFileToDisk(ZipPath, fileName, destPath);
        }

        public void UnzipFile(List<string> fileNames, string destPath)
        {
            foreach (var fileName in fileNames)
                UnZipper.ExtractFileToDisk(ZipPath, fileName, destPath);            
        }
    }
}