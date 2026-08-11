using BikeRent.ConsoleApp.Ui;
using BikeRent.Database.AppDbContextModels;
using BikeRent.Domain.Services;

namespace BikeRent.ConsoleApp.Menus;

public class AdminMenu
{
    private readonly BikeRentService _service;
    private readonly TblUser _admin;

    public AdminMenu(BikeRentService service, TblUser admin)
    {
        _service = service;
        _admin = admin;
    }

    public void Show()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"=== ADMIN MENU ===");
            Console.WriteLine("1. Add new bike");
            Console.WriteLine("2. Update bike");
            Console.WriteLine("3. View past rentals");
            Console.WriteLine("4. View daily rental totals");
            Console.WriteLine("5. Remove bikes(old / broken)");
            Console.WriteLine("0. Logout");
            Console.Write("Select: ");

            switch (Console.ReadLine())
            {
                case "1": AddBike(); break;
                case "2": EditBike(); break;
                case "3": ViewPastRentals(); break;
                case "4": ViewDailyTotals(); break;
                case "5": RemoveBikes(); break;
                case "0": return;
            }
        }
    }

    private void AddBike()
    {
        Console.Clear();
        Console.WriteLine("=== ADD NEW BIKE ===");
        var name = ConsoleHelper.ReadNonEmpty("Bike name: ");
        var type = ConsoleHelper.ReadNonEmpty("Bike type: ");
        var price = ConsoleHelper.ReadDecimal("Rental price per hour: ");
        var result = _service.AddBike(name, type, price);
        ConsoleHelper.PrintResult(result);
    }

    private void EditBike()
    {
        Console.Clear();
        PrintBikeTable(_service.GetAllBikes());
        var bikeId = ConsoleHelper.ReadInt("\nEnter bike ID to edit: ");
        var bike = _service.GetBike(bikeId);
        if (bike is null)
        {
            Console.WriteLine("Bike not found.");
            ConsoleHelper.Pause();
            return;
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"=== EDIT BIKE #{bike.BikeId} - {bike.Name} ===");
            Console.WriteLine($"Current status   : {bike.Status}");
            Console.WriteLine($"Current price/hr : {bike.PricePerHour:N2}");
            Console.WriteLine($"Current condition: {bike.Condition}");
            Console.WriteLine();
            Console.WriteLine("1. Change status");
            Console.WriteLine("2. Change rental price");
            Console.WriteLine("3. Change condition");
            Console.WriteLine("4. Save and back");
            Console.Write("Select: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Write("New status: ");
                    var status = Console.ReadLine();
                    PrintResult(_service.UpdateBike(bikeId, status, null, null));
                    break;
                case "2":
                    var price = ConsoleHelper.ReadDecimal("New rental price per hour: ");
                    PrintResult(_service.UpdateBike(bikeId, null, price, null));
                    break;
                case "3":
                    Console.Write("New condition: ");
                    var condition = Console.ReadLine();
                    PrintResult(_service.UpdateBike(bikeId, null, null, condition));
                    break;
                case "4":
                    return;
            }
        }
    }

    private void ViewPastRentals()
    {
        Console.Clear();
        var rentals = _service.GetPastRentals();
        if (rentals.Count == 0)
        {
            Console.WriteLine("No past rentals found.");
            ConsoleHelper.Pause();
            return;
        }
        Console.WriteLine("=== PAST RENTALS ===");
        Console.WriteLine("+----+---------------------+----------------------+----------------------+---------------------+--------------+----------------+-------------+");
        Console.WriteLine("| ID | Rented               | Customer             | Bike                 | Actual Return       | Base + Late  | Late Fee       | Total Price |");
        Console.WriteLine("+----+---------------------+----------------------+----------------------+---------------------+--------------+----------------+-------------+");
        foreach (var rental in rentals)
        {
            var customer = rental.User?.Name ?? "?";
            var bikeName = rental.Bike?.Name ?? "?";
            Console.WriteLine(
                $"| {rental.RentalId,2} | {rental.RentDatetime,19:yyyy-MM-dd HH:mm} | {customer,-20} | {bikeName,-20} | {rental.ActualReturn,19:yyyy-MM-dd HH:mm} | {rental.TotalPrice - rental.LateFee,12:N2} | {rental.LateFee,14:N2} | {rental.TotalPrice,11:N2} |");
        }
        Console.WriteLine("+----+---------------------+----------------------+----------------------+---------------------+--------------+----------------+-------------+");
        ConsoleHelper.Pause();
    }

    private void ViewDailyTotals()
    {
        Console.Clear();
        var totals = _service.GetDailyTotals();
        if (totals.Count == 0)
        {
            Console.WriteLine("No rentals found.");
            ConsoleHelper.Pause();
            return;
        }
        Console.WriteLine("=== DAILY RENTAL TOTALS ===");
        Console.WriteLine("+------------+---------------+");
        Console.WriteLine("| Date       | Total Revenue |");
        Console.WriteLine("+------------+---------------+");
        foreach (var (date, total) in totals)
        {
            Console.WriteLine($"| {date:yyyy-MM-dd} | {total,13:N2} |");
        }
        Console.WriteLine("+------------+---------------+");
        var grandTotal = totals.Sum(t => t.Total);
        Console.WriteLine($"\nGrand total: {grandTotal:N2}");
        ConsoleHelper.Pause();
    }

    private void RemoveBikes()
    {
        Console.Clear();
        Console.WriteLine("=== REMOVE BIKES BY CONDITION ===");
        Console.WriteLine("Which condition of bikes do you want to remove?");
        Console.WriteLine("1. old");
        Console.WriteLine("2. broken");
        Console.Write("Select: ");
        var condition = Console.ReadLine() switch
        {
            "1" => "old",
            "2" => "broken",
            _ => null
        };
        if (condition is null)
        {
            Console.WriteLine("Invalid selection.");
            ConsoleHelper.Pause();
            return;
        }

        var bikes = _service.GetAllBikes().Where(b => b.Condition == condition).ToList();
        if (bikes.Count == 0)
        {
            Console.WriteLine($"No bikes found with condition '{condition}'.");
            ConsoleHelper.Pause();
            return;
        }
        Console.WriteLine($"\nBikes with condition '{condition}':");
        PrintBikeTable(bikes);
        Console.Write($"Remove these {bikes.Count} bike(s)? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() != "y")
        {
            Console.WriteLine("Removal cancelled.");
            ConsoleHelper.Pause();
            return;
        }
        var result = _service.RemoveBikesByCondition(condition);
        ConsoleHelper.PrintResult(result);
    }

    private static void PrintBikeTable(List<TblBike> bikes)
    {
        Console.WriteLine("+----+----------------------+----------------+--------------+--------------+-----------+");
        Console.WriteLine("| ID | Name                 | Type           | Price / Hour | Availability | Condition |");
        Console.WriteLine("+----+----------------------+----------------+--------------+--------------+-----------+");
        foreach (var bike in bikes)
        {
            Console.WriteLine($"| {bike.BikeId,2} | {bike.Name,-20} | {bike.Type,-14} | {bike.PricePerHour,12:N2} | {BikeRentService.AvailabilityLabel(bike),-12} | {bike.Condition,-9} |");
        }
        Console.WriteLine("+----+----------------------+----------------+--------------+--------------+-----------+");
    }

    private static void PrintResult(OperationResult result)
    {
        ConsoleHelper.PrintResult(result);
    }
}
