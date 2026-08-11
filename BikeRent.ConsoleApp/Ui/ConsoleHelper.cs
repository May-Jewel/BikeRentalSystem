using BikeRent.Domain.Services;

namespace BikeRent.ConsoleApp.Ui;

public static class ConsoleHelper
{
    public static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out var value))
                return value;
            Console.WriteLine("Invalid number.Please try again.");
        }
    }

    public static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (decimal.TryParse(Console.ReadLine(), out var value))
                return value;
            Console.WriteLine("Invalid amount.Please try again.");
        }
    }

    public static string ReadNonEmpty(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
                return input.Trim();
            Console.WriteLine("This field is required.");
        }
    }

    public static void Pause(string message = "Press any key to continue...")
    {
        Console.WriteLine();
        Console.WriteLine(message);
        Console.ReadKey(true);
    }

    public static void PrintResult(OperationResult result)
    {
        var color = result.Success ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine();
        Console.ForegroundColor = color;
        Console.WriteLine(result.Message);
        Console.ResetColor();
        Pause();
    }
}
