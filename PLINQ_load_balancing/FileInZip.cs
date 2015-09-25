using System;

namespace PLINQ_load_balancing
{
    public class FileInZip
    {
        public readonly string ParentZip;

        public readonly long Size;

        public FileInZip(string parentZip, string filePath, long size)
        {
            ParentZip = parentZip;
            FilePath = filePath;
            Size = size;
        }

        public string FilePath { get; set; }

        public bool ContainsString(string str)
        {
            return ParentZip != null && UnZipper.ContainsString(ParentZip, FilePath, str);
        }
    }
}