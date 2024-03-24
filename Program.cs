using System.Globalization;

namespace Ldl3
{
    internal class Program
    {
        
         static git gitInstance = new git();
         static GitBackgroundThread backgroundThread = new GitBackgroundThread(gitInstance, 5000);
        static public void Main()
        {
            string input;
            

        start:;

            Console.Write(">");
            input = Console.ReadLine();
            if (input.StartsWith("info ")) { gitInstance.info(input); Console.WriteLine("\n"); goto start; }
            else if (input == "commit") { gitInstance.commit(); backgroundThread.Start(); Console.WriteLine("\n"); goto start; }
            else if (input == "status") { gitInstance.status(); Console.WriteLine("\n"); goto start; }
            else if (input == "end") { backgroundThread.Stop(); return; }
            else { Console.WriteLine($"Operation {input} is not a valid operation"); goto start; }


        }
    }
}
