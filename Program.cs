using System.Globalization;

namespace Ldl3
{
    internal class Program
    {
        static git Git = new git();
        static void Main()
        {
            string input;

        start:;

            Console.Write(">");
            input = Console.ReadLine();
            if (input.StartsWith("info ")) { Git.info(input); Console.WriteLine("\n"); goto start; }
            else if (input == "commit") { Git.commit(); Console.WriteLine("\n"); goto start; }
            else if (input == "status") { Git.status(); Console.WriteLine("\n"); goto start; }
            else if (input == "end") { return; }
            else { Console.WriteLine($"Operation {input} is not a valid operation"); goto start; }


        }
    }
}
