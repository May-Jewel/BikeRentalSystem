using BikeRent.ConsoleApp.Ui;
using BikeRent.Database.AppDbContextModels;
using BikeRent.Domain.Services;

namespace BikeRent.ConsoleApp.Menus;

public class CustomerMenu
{
    private readonly BikeRentService _service;
    private readonly TblUser _user;

    public CustomerMenu(BikeRentService service, TblUser user)
    {
        _service = service;
        _user = user;
    }

    public void Show()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"=== CUSTOMER MENU ===");
            Console.WriteLine("1. Browse available bikes");
            Console.WriteLine("2. View bike details");
            Console.WriteLine("3. Rent a bike");
            Console.WriteLine("4. Calculate rental fee (hourly)");
            Console.WriteLine("5. Checkout (total price and complete rental)");
            Console.WriteLine("6. Return a bike");
            Console.WriteLine("7. My rentals");
            Console.WriteLine("0. Logout");
            Console.Write("Select: ");

            switch (Console.ReadLine())
            {
                case "1": BrowseAvailableBikes(); break;
                case "2": ViewBikeDetails(); break;
                case "3": RentBike(); break;
                case "4": CalculateFee(); break;
                case "5": Checkout(); break;
                case "6": ReturnBike(); break;
                case "7": MyRentals(); break;
                case "0": return;
            }
        }
    }

    private void BrowseAvailableBikes()
    {
        Console.Clear();
        var bikes = _service.GetAvailableBikes();
        if (bikes.Count == 0)
        {
            Console.WriteLine("No bikes are available right now.");
            ConsoleHelper.Pause();
            return;
        }
        Console.WriteLine("=== AVAILABLE BIKES ===");
        PrintBikeTable(bikes);
        ConsoleHelper.Pause();
    }

    private void ViewBikeDetails()
    {
        Console.Clear();
        PrintBikeTable(_service.GetAllBikes());
        var bikeId = ConsoleHelper.ReadInt("\nEnter bike ID to view details: ");
        var bike = _service.GetBike(bikeId);
        if (bike is null)
        {
            Console.WriteLine("Bike not found.");
            ConsoleHelper.Pause();
            return;
        }
        Console.WriteLine();
        Console.WriteLine($"ID           : {bike.BikeId}");
        Console.WriteLine($"Name         : {bike.Name}");
        Console.WriteLine($"Type         : {bike.Type}");
        Console.WriteLine($"Price / hour : {bike.PricePerHour:N2}");
        Console.WriteLine($"Availability : {BikeRentService.AvailabilityLabel(bike)}");
        Console.WriteLine($"Condition    : {bike.Condition}");
        ConsoleHelper.Pause();
    }

    private void RentBike()
    {
        Console.Clear();
        var bikes = _service.GetAvailableBikes();
        if (bikes.Count == 0)
        {
            Console.WriteLine("No bikes are available right now.");
            ConsoleHelper.Pause();
            return;
        }
        PrintBikeTable(bikes);
        var bikeId = ConsoleHelper.ReadInt("\nEnter bike ID to rent: ");
        var bike = _service.GetBike(bikeId);
        if (bike is null)
        {
            Console.WriteLine("Bike not found.");
            ConsoleHelper.Pause();
            return;
        }
        if (bike.Status != "available")
        {
            Console.WriteLine($"Bike '{bike.Name}' is currently rented.");
            ConsoleHelper.Pause();
            return;
        }

        var hours = ConsoleHelper.ReadInt("Enter rental duration in hours (max 72): ");
        var fee = _service.CalculateFee(bike, hours);
        Console.WriteLine();
        Console.WriteLine("=== RENTAL SUMMARY ===");
        Console.WriteLine($"Bike          : {bike.Name} ({bike.Type})");
        Console.WriteLine($"Price / hour  : {bike.PricePerHour:N2}");
        Console.WriteLine($"Duration      : {hours} hour(s)");
        Console.WriteLine($"Total (hourly): {fee:N2}");
        Console.Write("Confirm rental? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() != "y")
        {
            Console.WriteLine("Rental cancelled.");
            ConsoleHelper.Pause();
            return;
        }

        var result = _service.RentBike(_user.UserId, bikeId, hours);
        if (result.Success)
        {
            Console.WriteLine($"\n{result.Message}");
            Console.WriteLine("Bike is now marked as rented.");
        }
        else
        {
            ConsoleHelper.PrintResult(result);
            return;
        }
        ConsoleHelper.Pause();
    }

    private void CalculateFee()
    {
        Console.Clear();
        PrintBikeTable(_service.GetAllBikes());
        var bikeId = ConsoleHelper.ReadInt("\nEnter bike ID: ");
        var bike = _service.GetBike(bikeId);
        if (bike is null)
        {
            Console.WriteLine("Bike not found.");
            ConsoleHelper.Pause();
            return;
        }
        var hours = ConsoleHelper.ReadInt("Enter number of hours: ");
        var fee = _service.CalculateFee(bike, hours);
        Console.WriteLine();
        Console.WriteLine($"Bike          : {bike.Name} ({bike.Type})");
        Console.WriteLine($"Price / hour  : {bike.PricePerHour:N2}");
        Console.WriteLine($"Duration      : {hours} hour(s)");
        Console.WriteLine($"Total (hourly): {fee:N2}");
        ConsoleHelper.Pause();
    }

    private void Checkout()
    {
        Console.Clear();
        var rentals = _service.GetActiveRentals(_user.UserId);
        if (rentals.Count == 0)
        {
            Console.WriteLine("You have no active rentals to check out.");
            ConsoleHelper.Pause();
            return;
        }
        Console.WriteLine("=== ACTIVE RENTALS (pending checkout) ===");
        PrintRentalTable(rentals);
        var rentalId = ConsoleHelper.ReadInt("\nEnter rental ID to check out: ");
        var rental = rentals.FirstOrDefault(r => r.RentalId == rentalId);
        if (rental is null)
        {
            Console.WriteLine("Invalid rental ID.");
            ConsoleHelper.Pause();
            return;
        }

        Console.WriteLine();
        Console.WriteLine("=== CHECKOUT RECEIPT ===");
        Console.WriteLine($"Rental ID     : {rental.RentalId}");
        Console.WriteLine($"Bike          : {rental.Bike.Name} ({rental.Bike.Type})");
        Console.WriteLine($"Rent time     : {rental.RentDatetime:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"Expected back : {rental.ExpectedReturn:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"Total price   : {rental.TotalPrice:N2}");
        Console.Write("\nConfirm payment and complete checkout? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() != "y")
        {
            Console.WriteLine("Checkout cancelled.");
            ConsoleHelper.Pause();
            return;
        }
        Console.WriteLine("\nCheckout complete. Rental finalized.");
        Console.WriteLine($"Amount paid: {rental.TotalPrice:N2}");
        ConsoleHelper.Pause();
    }

    private void ReturnBike()
    {
        Console.Clear();
        var rentals = _service.GetActiveRentals(_user.UserId);
        if (rentals.Count == 0)
        {
            Console.WriteLine("You have no active rentals to return.");
            ConsoleHelper.Pause();
            return;
        }
        Console.WriteLine("=== ACTIVE RENTALS ===");
        PrintRentalTable(rentals);
        var rentalId = ConsoleHelper.ReadInt("\nEnter rental ID to return: ");
        var rental = rentals.FirstOrDefault(r => r.RentalId == rentalId);
        if (rental is null)
        {
            Console.WriteLine("Invalid rental ID.");
            ConsoleHelper.Pause();
            return;
        }
        Console.WriteLine();
        Console.WriteLine($"Bike          : {rental.Bike.Name}");
        Console.WriteLine($"Expected back : {rental.ExpectedReturn:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"Now           : {DateTime.Now:yyyy-MM-dd HH:mm}");
        Console.Write("Confirm return? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() != "y")
        {
            Console.WriteLine("Return cancelled.");
            ConsoleHelper.Pause();
            return;
        }
        var result = _service.ReturnBike(rentalId);
        ConsoleHelper.PrintResult(result);
    }

    private void MyRentals()
    {
        Console.Clear();
        var rentals = _service.GetRentalsByUser(_user.UserId);
        if (rentals.Count == 0)
        {
            Console.WriteLine("You have no rentals yet.");
            ConsoleHelper.Pause();
            return;
        }
        Console.WriteLine("=== MY RENTALS ===");
        PrintRentalTable(rentals);
        ConsoleHelper.Pause();
    }

    private static void PrintBikeTable(List<TblBike> bikes)
    {
        Console.WriteLine("+----+----------------------+----------------+--------------+--------------+");
        Console.WriteLine("| ID | Name                 | Type           | Price / Hour | Availability |");
        Console.WriteLine("+----+----------------------+----------------+--------------+--------------+");
        foreach (var bike in bikes)
        {
            Console.WriteLine($"| {bike.BikeId,2} | {bike.Name,-20} | {bike.Type,-14} | {bike.PricePerHour,12:N2} | {BikeRentService.AvailabilityLabel(bike),-12} |");
        }
        Console.WriteLine("+----+----------------------+----------------+--------------+--------------+");
    }

    private static void PrintRentalTable(List<TblRental> rentals)
    {
        Console.WriteLine("+----+----------------------+---------------------+---------------------+---------------------+----------------+");
        Console.WriteLine("| ID | Bike                 | Rented              | Expected Return     | Actual Return       | Total Price    |");
        Console.WriteLine("+----+----------------------+---------------------+---------------------+---------------------+----------------+");
        foreach (var rental in rentals)
        {
            var bikeName = rental.Bike?.Name ?? "?";
            var actual = rental.ActualReturn.HasValue ? rental.ActualReturn.Value.ToString("yyyy-MM-dd HH:mm") : "not returned";
            Console.WriteLine($"| {rental.RentalId,2} | {bikeName,-20} | {rental.RentDatetime,19:yyyy-MM-dd HH:mm} | {rental.ExpectedReturn,19:yyyy-MM-dd HH:mm} | {actual,-19} | {rental.TotalPrice,14:N2} |");
        }
        Console.WriteLine("+----+----------------------+---------------------+---------------------+---------------------+----------------+");
    }
}
