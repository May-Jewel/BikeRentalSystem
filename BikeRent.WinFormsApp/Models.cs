namespace BikeRent.WinFormsApp;

public record BikeRow(int Id, string Name, string Type, decimal PricePerHour, string Status, string Condition);

public record RentalRow(int Id, string Customer, string Bike, DateTime Rented, DateTime ExpectedBack, DateTime? Returned, decimal LateFee, decimal Total);

public record DailyTotalRow(DateTime Date, decimal Total);
