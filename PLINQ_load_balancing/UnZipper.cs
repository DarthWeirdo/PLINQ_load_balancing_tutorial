using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;

namespace PLINQ_load_balancing
{
    public static class UnZipper
    {
        public static Dictionary<string, MemoryStream> ExtractFilesToStream(string zipSource, string mask)
        {
            Dictionary<string, MemoryStream> memoryStreams = new Dictionary<string, MemoryStream>();
            using (ZipFile zip = ZipFile.Read(zipSource))
            {
                var file = zip.SelectEntries(mask);
                foreach (var zipEntry in file)
                {
                    MemoryStream stream = new MemoryStream();
                    zipEntry.Extract(stream);
                    memoryStreams.Add(zipEntry.FileName, stream);
                }
            }
            return memoryStreams;
        }

        public static MemoryStream ExtractFileToStream(string zipSource, string filePath)
        {
            MemoryStream stream = new MemoryStream();
            using (ZipFile zip = ZipFile.Read(zipSource))
            {
                var matchFile = new ZipEntry();
                foreach (var file in zip.Where(file => file.FileName == filePath))
                    matchFile = file;                
                matchFile.Extract(stream);
            }
            return stream;
        }

        public static void ExtractFileToDisk(string zipSource, string fileName, string destination)
        {
            using (ZipFile zip = ZipFile.Read(zipSource))
                zip.ExtractSelectedEntries("name =" + fileName, null, destination, ExtractExistingFileAction.OverwriteSilently);            
        }        

        public static Dictionary<string, long> GetFileList(string zipSource)
        {
            Dictionary<string, long> result = new Dictionary<string, long>();
            using (ZipFile zip = ZipFile.Read(zipSource))
            {
                foreach (var zipEntry in zip.Where(zipEntry => !zipEntry.IsDirectory))
                    result.Add(zipEntry.FileName, zipEntry.UncompressedSize);                
            }
            return result;
        }

        public static bool ContainsString(string zipSource, string filePath, string str)
        {            
            using (MemoryStream fileStream = ExtractFileToStream(zipSource, filePath))
            {
                string strFile = Encoding.UTF8.GetString(fileStream.ToArray());
                if (strFile.Contains(str)) 
                    return true;
            }
            return false;
        }
        
    }
}