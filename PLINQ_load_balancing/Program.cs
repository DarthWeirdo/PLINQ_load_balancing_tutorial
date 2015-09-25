using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PLINQ_load_balancing
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: PLINQ_load_balancing.exe <option>" + Environment.NewLine +
                                      Environment.NewLine +
                                      "Possible options:" + Environment.NewLine +
                                      "-----------------" + Environment.NewLine +
                                      "linq - one thread LINQ" + Environment.NewLine +
                                      "asParallel - default PLINQ" + Environment.NewLine +
                                      "dynamic - dynamic partitioner" + Environment.NewLine +
                                      "static - static partitioner" + Environment.NewLine +
                                      "custom - custom partitioner");                
            }
            else
            {                
                switch (args[0])
                {                                       
                    case "linq":
                        TestStringInZipFeature_LINQ();
                        break;
                    case "asParallel":
                        TestStringInZipFeature_AsParallel();
                        break;
                    case "dynamic":
                        TestStringInZipFeature_Dynamic_Partitioner();
                        break;
                    case "static":
                        TestStringInZipFeature_Static_Partitioner();
                        break;
                    case "custom":
                        TestStringInZipFeature_Custom_Partitioner();
                        break;
                }    
            }            
        }

        public static void TestStringInZipFeature_Custom_Partitioner()
        {
            // We prepared 15 sample zips (1.zip - 15.zip) with text files 
            // inside. Some of them contain the string 'test'.
            DirectoryInfo path = Directory.GetParent(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
            List<ZipPackage> packages = new List<ZipPackage>();
            for (int i = 1; i <= 15; i++)         
                packages.Add(new ZipPackage(string.Format("{0}\\zips\\{1}.zip", path, i)));                                    

            string str = "test";

            // Select all files 
            List<FileInZip> filesInZips = (from zipPackage in packages
                                           from file in zipPackage.Files
                                           select file).ToList();

            // Custom partitioner
            MyPartitioner customPartitioner = new MyPartitioner(filesInZips);            
            
            // PLINQ 
            var query = (from file in customPartitioner.AsParallel()
                         where file.ContainsString(str)
                         select file).ToList();                                       

            Debug.WriteLine("Number of files containing '{0}': {1}", str, query.Count);                        
        }

        
        public static void TestStringInZipFeature_Static_Partitioner()
        {
            // We prepared 15 sample zips (1.zip - 15.zip) with text files 
            // inside. Some of them contain the string 'test'.
            DirectoryInfo path = Directory.GetParent(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
            List<ZipPackage> packages = new List<ZipPackage>();
            for (int i = 1; i <= 15; i++)
                packages.Add(new ZipPackage(string.Format("{0}\\zips\\{1}.zip", path, i)));

            string str = "test";

            // Select all files 
            List<FileInZip> filesInZips = (from zipPackage in packages
                                           from file in zipPackage.Files
                                           select file).ToList();

            // Static partitioner
            Partitioner<FileInZip> customPartitioner = Partitioner.Create(filesInZips, false);

            // PLINQ 
            var query = (from file in customPartitioner.AsParallel()
                         where file.ContainsString(str)
                         select file).ToList();

            Debug.WriteLine("Number of files containing '{0}': {1}", str, query.Count);
        }


        public static void TestStringInZipFeature_Dynamic_Partitioner()
        {
            // We prepared 15 sample zips (1.zip - 15.zip) with text files 
            // inside. Some of them contain the string 'test'.
            DirectoryInfo path = Directory.GetParent(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
            List<ZipPackage> packages = new List<ZipPackage>();
            for (int i = 1; i <= 15; i++)
                packages.Add(new ZipPackage(string.Format("{0}\\zips\\{1}.zip", path, i)));

            string str = "test";

            // Select all files 
            List<FileInZip> filesInZips = (from zipPackage in packages
                                           from file in zipPackage.Files
                                           select file).ToList();

            // Dynamic partitioner
            Partitioner<FileInZip> customPartitioner = Partitioner.Create(filesInZips, true);

            // PLINQ 
            var query = (from file in customPartitioner.AsParallel()
                         where file.ContainsString(str)
                         select file).ToList();

            Debug.WriteLine("Number of files containing '{0}': {1}", str, query.Count);
        }


        public static void TestStringInZipFeature_AsParallel()
        {
            // We prepared 15 sample zips (1.zip - 15.zip) with text files 
            // inside. Some of them contain the string 'test'.
            DirectoryInfo path = Directory.GetParent(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
            List<ZipPackage> packages = new List<ZipPackage>();
            for (int i = 1; i <= 15; i++)
                packages.Add(new ZipPackage(string.Format("{0}\\zips\\{1}.zip", path, i)));

            string str = "test";

            // PLINQ 
            List<FileInZip> query = (from zip in packages.AsParallel()
                                     from file in zip.Files
                                     where file.ContainsString(str)
                                     select file).ToList();

            Debug.WriteLine("Number of files containing '{0}': {1}", str, query.Count);
        }


        public static void TestStringInZipFeature_LINQ()
        {
            // We prepared 15 sample zips (1.zip - 15.zip) with text files 
            // inside. Some of them contain the string 'test'.
            DirectoryInfo path = Directory.GetParent(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
            List<ZipPackage> packages = new List<ZipPackage>();
            for (int i = 1; i <= 15; i++)
                packages.Add(new ZipPackage(string.Format("{0}\\zips\\{1}.zip", path, i)));

            string str = "test";

            // LINQ 
            List<FileInZip> query = (from zip in packages
                                     from file in zip.Files
                                     where file.ContainsString(str)
                                     select file).ToList();

            Debug.WriteLine("Number of files containing '{0}': {1}", str, query.Count);
        }
    }
}
