using System;
using System.Collections.Generic;

public class Program
{
    public static void Main(string[] args)
    {
        while (true)
        {
            Console.Write("Enter the team name or code (or type 'all' to see all teams' scores, or 'exit' to quit): ");
            string input = Console.ReadLine();

            if (input.ToLower() == "exit")
            {
                break;
            }

            var result = Solution.Run(input);
            if (result != null)
            {
                foreach (var entry in result)
                {
                    Console.WriteLine($"Total goals scored by {entry.Key}: {entry.Value}");
                }
            }
            else
            {
                Console.WriteLine("An error occurred while calculating the goals.");
            }
        }
    }
}