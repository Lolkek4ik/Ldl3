using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;

namespace Ldl3
{
    public class GitBackgroundThread
    {
        private readonly git gitInstance;
        private readonly int intervalMilliseconds;
        private bool isRunning;
        private Thread backgroundThread;

        public GitBackgroundThread(git gitInstance, int intervalMilliseconds)
        {
            this.gitInstance = gitInstance;
            this.intervalMilliseconds = intervalMilliseconds;
            this.isRunning = false;
        }

        public void Start()
        {
            if (isRunning)
                return;

            isRunning = true;

            backgroundThread = new Thread(BackgroundTask);
            backgroundThread.IsBackground = true;
            backgroundThread.Start();
        }

        public void Stop()
        {
            isRunning = false;
        }

        private void BackgroundTask()
        {
            while (isRunning)
            {
                gitInstance.pasivestatus();

                Thread.Sleep(intervalMilliseconds);
            }
        }
    }



    public class git
    {
        private string rootDirectory;
        private string snapshotFilePath;
        public git()
        {
            rootDirectory = InitializeRootDirectory();
            string solutionDirectory = Directory.GetParent(rootDirectory).FullName;
            snapshotFilePath = Path.Combine(solutionDirectory, "snapshot.txt");

        }
        private string InitializeRootDirectory()
        {
            rootDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            while (!Directory.GetFiles(rootDirectory).Any(file => file.EndsWith(".csproj")))
            {
                rootDirectory = Directory.GetParent(rootDirectory).FullName;
            }
            rootDirectory = Path.Combine(rootDirectory, "test");
            return rootDirectory;
        }


        public void info(string input)
        {
            string[] parts = input.Split('<');
            if (parts[1].EndsWith('>')) { parts[1] = parts[1].Remove(parts[1].LastIndexOf('>')); }
            string fileName = parts[1];


            string rootDirectory = InitializeRootDirectory(); //Can be changed to the users input with another root, but im too lazy to do it.


            string fullPath = Path.Combine(rootDirectory, fileName);

            if (File.Exists(fullPath))
            {
                DateTime crtime = File.GetCreationTime(fullPath);
                DateTime uptime = File.GetLastWriteTime(fullPath);

                Console.WriteLine($"Name:{parts[1]}");
                Console.WriteLine($"Created on: {crtime}");
                Console.WriteLine($"Updated on: {uptime}");
            }
            else { Console.WriteLine($"File {fileName} does not exist"); return; }

            string[] ext = fileName.Split(".");
            /*==========================TEXT=====================================TEXT====================================TEXT=========================*/
            if (ext[1] == "txt")
            {

                string[] lines = File.ReadAllLines(fullPath);
                int lcount = lines.Length;
                Console.WriteLine($"Lines:{lcount}");

                string allText = string.Join(" ", lines);
                string[] words = allText.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                int wcount = words.Length;
                Console.WriteLine($"Words:{wcount}");

                int ccount = allText.Length;
                Console.WriteLine($"Characters:{ccount}");

            }

            /*==========================IMAGE=====================================IMAGE====================================IMAGE=========================*/

            if ((ext[1] == "jpg" || ext[1] == "jpeg") || (ext[1] == "png" || ext[1] == "svg"))
            {
                using (var bitmap = new Bitmap(fullPath))
                {
                    int width = bitmap.Width;
                    int height = bitmap.Height;
                    Console.WriteLine($"Image size:{width}x{height} ");
                }
            }

            /*=========================PROG========================================PROG======================================PROG===========================*/

            if (ext[1] == "cs" || ext[1] == "java" || ext[1] == "py")
            {
                string[] lines = File.ReadAllLines(fullPath);
                int lcount = lines.Length;
                Console.WriteLine($"Lines:{lcount}");

                string text = string.Join(" ", lines);
                int clcount = 0;
                foreach (string word in text.Split(" ")) { if (word == "class") clcount++; }
                Console.WriteLine($"Classes in code:{clcount}");
            }



        }
        /*========================COMMIT====================================COMMIT================================================COMMIT====================*/
        public void commit()
        {
            string rootDirectory = InitializeRootDirectory(); //Can be changed to the users input with another root, but im too lazy to do it.

            Console.WriteLine($"[SNAPSHOT CREATED AT {DateTime.Now}]");

            string[] files = Directory.GetFiles(rootDirectory, "*.*", SearchOption.AllDirectories);

            Dictionary<string, string> currentSnapshot = new Dictionary<string, string>();

            Dictionary<string, string> previousSnapshot = LoadPreviousSnapshot();

            foreach (string file in files)
            {
                string currentHash = CalculateFileHash(file);

                currentSnapshot[file] = currentHash;
                Console.WriteLine($"{Path.GetFileName(file)}");
            }




            SaveSnapshot(currentSnapshot);
        }
        /*================LoadPreviousSnapshot====================================LoadPreviousSnapshot================================LoadPreviousSnapshot=================================*/
        private Dictionary<string, string> LoadPreviousSnapshot()
        {
            Dictionary<string, string> previousSnapshot = new Dictionary<string, string>();

            if (File.Exists(snapshotFilePath))
            {
                string[] lines = File.ReadAllLines(snapshotFilePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 2)
                    {
                        previousSnapshot[parts[0]] = parts[1];
                    }
                }
            }

            return previousSnapshot;
        }
        /*=======================SaveSnapshot=====================================SaveSnapshot===========================================SaveSnapshot=================================*/
        private void SaveSnapshot(Dictionary<string, string> currentSnapshot)
        {
            using (StreamWriter writer = new StreamWriter(snapshotFilePath))
            {
                foreach (var kvp in currentSnapshot)
                {
                    writer.WriteLine($"{kvp.Key}|{kvp.Value}");
                }
            }
        }
        /*=================FILEHASH=============================================FILEHASH=====================================================FILEHASH================================*/
        private string CalculateFileHash(string filePath)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        /*==================status===================================================status===========================================================status===========================================*/

        public void status()
        {
            Dictionary<string, string> previousSnapshot = LoadPreviousSnapshot();

            string rootDirectory = InitializeRootDirectory();

            string[] files = Directory.GetFiles(rootDirectory, "*.*", SearchOption.AllDirectories);
            Dictionary<string, string> currentSnapshot = new Dictionary<string, string>();
            foreach (string file in files)
            {
                string currentHash = CalculateFileHash(file);
                currentSnapshot[file] = currentHash;
            }

            Console.WriteLine("State of files since last snapshot:");
            foreach (var kvp in currentSnapshot)
            {
                string file = kvp.Key;
                string currentHash = kvp.Value;

                if (previousSnapshot.ContainsKey(file))
                {
                    string previousHash = previousSnapshot[file];
                    if (currentHash != previousHash)
                    {
                        Console.WriteLine($" {Path.GetFileName(file)} - Edited");
                    }
                    else
                    {
                        Console.WriteLine($" {Path.GetFileName(file)} - No changes");
                    }
                }
                else
                {
                    Console.WriteLine($" {Path.GetFileName(file)} - Added");
                }
            }

            foreach (var kvp in previousSnapshot)
            {
                string file = kvp.Key;
                if (!currentSnapshot.ContainsKey(file))
                {
                    Console.WriteLine($" {Path.GetFileName(file)} - Deleted");
                }
            }
        }

        /*=============================PASIVESTATUS===============================================PASIVESTATUS===================================================PASIVESTATUS===============================*/

        public void pasivestatus()
        {
            Dictionary<string, string> previousSnapshot = LoadPreviousSnapshot();

            string rootDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            while (!Directory.GetFiles(rootDirectory).Any(file => file.EndsWith(".csproj")))
            {
                rootDirectory = Directory.GetParent(rootDirectory).FullName;
            }
            rootDirectory = Path.Combine(rootDirectory, "test");

            string[] files = Directory.GetFiles(rootDirectory, "*.*", SearchOption.AllDirectories);
            Dictionary<string, string> currentSnapshot = new Dictionary<string, string>();
            foreach (string file in files)
            {
                string currentHash = CalculateFileHash(file);
                currentSnapshot[file] = currentHash;
            }

            foreach (var kvp in currentSnapshot)
            {
                string file = kvp.Key;
                string currentHash = kvp.Value;

                if (previousSnapshot.ContainsKey(file))
                {
                    string previousHash = previousSnapshot[file];
                    if (currentHash != previousHash)
                    {
                        Console.Write($" {Path.GetFileName(file)} - Edited \n>");
                    }
                    
                }
                else
                {
                    Console.Write($" {Path.GetFileName(file)} - Added \n>");
                }
            }

            foreach (var kvp in previousSnapshot)
            {
                string file = kvp.Key;
                if (!currentSnapshot.ContainsKey(file))
                {
                    Console.Write($" {Path.GetFileName(file)} - Deleted \n>");
                }
            }
        }


    }
}
