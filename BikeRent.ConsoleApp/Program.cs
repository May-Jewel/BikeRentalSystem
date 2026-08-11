using BikeRent.ConsoleApp.Menus;
using BikeRent.ConsoleApp.Ui;
using BikeRent.Domain.Services;
using BikeRent.Database.AppDbContextModels;

namespace BikeRent.ConsoleApp;

public static class Program
{
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    private const int SW_MAXIMIZE = 3;

    public static void Main()
    {
        MaximizeConsoleWindow();
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = "Bike Rental System";

        try
        {
            using var db = new AppDbContext();
            var service = new BikeRentService(db);
            MainMenu(service);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not connect to the database: {ex.Message}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }

    private static void MaximizeConsoleWindow()
    {
        try
        {
            IntPtr handle = GetConsoleWindow();
            if (handle != IntPtr.Zero)
            {
                ShowWindow(handle, SW_MAXIMIZE);
            }
        }
        catch
        {
        }
    }

    private static void MainMenu(BikeRentService service)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== BIKE RENTAL SYSTEM ===");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register as customer");
            Console.WriteLine("0. Exit");
            Console.Write("Select: ");

            switch (Console.ReadLine())
            {
                case "1": Login(service); break;
                case "2": Register(service); break;
                case "0": return;
            }
        }
    }

    private static void Login(BikeRentService service)
    {
        Console.Clear();
        Console.WriteLine("=== LOGIN ===");
        var phone = ConsoleHelper.ReadNonEmpty("Phone: ");
        var password = ConsoleHelper.ReadNonEmpty("Password: ");

        var user = service.Login(phone, password);
        if (user is null)
        {
            Console.WriteLine("\nInvalid phone or password.");
            ConsoleHelper.Pause();
            return;
        }

        Console.WriteLine($"\nWelcome, {user.Name}!");
        ConsoleHelper.Pause();
        if (user.Role == "admin")
        {
            new AdminMenu(service, user).Show();
        }
        else
        {
            new CustomerMenu(service, user).Show();
        }
    }

    private static void Register(BikeRentService service)
    {
        Console.Clear();
        Console.WriteLine("=== REGISTER AS CUSTOMER ===");
        var name = ConsoleHelper.ReadNonEmpty("Name: ");
        var phone = ConsoleHelper.ReadNonEmpty("Phone: ");
        var password = ConsoleHelper.ReadNonEmpty("Password: ");
        var confirm = ConsoleHelper.ReadNonEmpty("Confirm password: ");

        if (password != confirm)
        {
            Console.WriteLine("\nPasswords do not match.");
            ConsoleHelper.Pause();
            return;
        }

        var result = service.RegisterCustomer(name, phone, password);
        ConsoleHelper.PrintResult(result);
    }
}
