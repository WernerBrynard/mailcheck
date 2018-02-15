using System;

namespace MailCheck.ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            RunProgram();
        }

        //public static string[] FreshSecondLevelDomains = new string[] { "yahoo", "hotmail", "live", "outlook" };

        public static void RunProgram()
        {
            Console.WriteLine("What's your email");
            var input = Console.ReadLine();

            MailCheck mailCheck = new MailCheck();

            var result = mailCheck.Run(input, null, null);

            if (result != null)
                Console.WriteLine($"Did you mean: {result}");
            else
                Console.WriteLine($"No recommendations found");

            RunProgram();
        }
    }
}
