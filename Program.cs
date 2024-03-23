using System.Globalization;

namespace Ldl3
{
    internal class Program
    {
        static git Git = new git();
        static void Main()
        {
            string input="0";

        start:;
            
            Console.Write(">");
            input = Console.ReadLine();
            if (input.StartsWith("info ")) { Git.info(input); goto start; }
            else if (input == "commit") { Git.commit(); goto start; }
            else if (input == "status") { Git.status(); goto start; }
            else if (input == "end") { return; }
            else { Console.WriteLine($"Operation {input} is not a valid operation"); Console.ReadLine(); goto start; }


        }
    }
}
