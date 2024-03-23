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
            
            Console.WriteLine(">");
            Console.ReadLine();
            if (input.StartsWith("info ")) { Git.info(input); Console.ReadLine(); goto start; }
            else if (input == "commit") { Git.commit; Console.ReadLine(); goto start; }
            else if (input == "status") { Git.status; Console.ReadLine(); goto start; }

            else { Console.WriteLine($"Operation {input} is not a valid operation"); Console.ReadLine(); goto start; }


        }
    }
}
