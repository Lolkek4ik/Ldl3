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
    public class git
    {
        public void commit() { Console.WriteLine("Nothing, for now"); }
        public void status() { Console.WriteLine("Nothing, for now"); }

        public void info(string input) 
        {
            string[] parts = input.Split('<');
            if (parts[1].EndsWith('>')) { parts[1] = parts[1].Remove(parts[1].LastIndexOf('>')); }
            string fileName = parts[1];

            string rootDirectory = @"C:\Users\denis\source\repos\Ldl3\test\";
            string fullPath = Path.Combine(rootDirectory, fileName);

            if (File.Exists(fullPath))
            {
                DateTime crtime = File.GetCreationTime(fullPath); 
                DateTime uptime = File.GetLastWriteTime(fullPath);
                
                Console.WriteLine($"Name:{parts[1]}");
                Console.WriteLine($"Created on: {crtime}");
                Console.WriteLine($"Updated on: {uptime}");
            }
            else {Console.WriteLine($"File {fileName} does not exist"); return; }

            string[] ext = fileName.Split(".");
/*==========================TEXT=====================================TEXT====================================TEXT=========================*/
            if (ext[1]=="txt") 
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

            if ((ext[1] == "jpg" || ext[1]=="jpeg") || (ext[1] =="png" || ext[1]== "svg")) 
            {
                using (var bitmap = new Bitmap(fullPath))
                {
                    int width = bitmap.Width;
                    int height = bitmap.Height;
                    Console.WriteLine($"Image size:{width}x{height} ");
                }
            }

/*=========================PROG========================================PROG======================================PROG===========================*/
            
            if(ext[1] =="cs" || ext[1] =="java" || ext[1]=="py") 
            {
                string[] lines = File.ReadAllLines(fullPath);
                int lcount = lines.Length;
                Console.WriteLine($"Lines:{lcount}");

                string text = string.Join(" ", lines);
                int clcount = 0;
                foreach (string word in text.Split(" ")) { if (word == "class") clcount++;}
                Console.WriteLine($"Classes in code:{clcount}");
            }



        }
    }
}
