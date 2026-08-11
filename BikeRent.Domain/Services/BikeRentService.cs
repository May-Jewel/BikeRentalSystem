using BikeRent.Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;

namespace BikeRent.Domain.Services;

public record OperationResult(bool Success, string Message);

public class BikeRentService
{
    private readonly AppDbContext _db;

    public BikeRentService(AppDbContext db)
    {
        _db = db;
    }

    // ---------- Auth ----------
    public TblUser? Login(string phone, string password)
    {
        return _db.TblUsers.FirstOrDefault(u => u.Phone == phone && u.Password == password);
    }

    public OperationResult RegisterCustomer(string name, string phone, string password)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(password))
            return new OperationResult(false, "Name, phone and password are required.");

        if (_db.TblUsers.Any(u => u.Phone == phone))
            return new OperationResult(false, "This phone number is already registered.");

        _db.TblUsers.Add(new TblUser
        {
            Name = name.Trim(),
            Phone = phone.Trim(),
            Password = password,
            Role = "customer"
        });
        _db.SaveChanges();
        return new OperationResult(true, $"Customer '{name.Trim()}' registered successfully.");
    }

    // ---------- Browse & details ----------
    public List<TblBike> GetAvailableBikes()
    {
        return _db.TblBikes
            .Where(b => b.Status == "available")
            .OrderBy(b => b.BikeId)
            .ToList();
    }

    public List<TblBike> GetAllBikes()
    {
        return _db.TblBikes.OrderBy(b => b.BikeId).ToList();
    }

    public TblBike? GetBike(int bikeId)
    {
        return _db.TblBikes.Find(bikeId);
    }

    public static string AvailabilityLabel(TblBike bike)
    {
        return bike.Status == "available" ? "Available" : "Rented";
    }

    // ---------- Rent ----------
    public decimal CalculateFee(TblBike bike, int hours)
    {
        return bike.PricePerHour * hours;
    }

    public OperationResult RentBike(int userId, int bikeId, int hours)
    {
        var bike = _db.TblBikes.Find(bikeId);
        if (bike is null)
            return new OperationResult(false, "Bike not found.");

        if (bike.Status != "available")
            return new OperationResult(false, $"Bike '{bike.Name}' is not available right now.");

        if (hours <= 0)
            return new OperationResult(false, "Rental duration must be greater than zero.");

        if (hours > 72)
            return new OperationResult(false, "Maximum rental duration is 72 hours.");

        var now = DateTime.Now;
        var rental = new TblRental
        {
            UserId = userId,
            BikeId = bikeId,
            RentDatetime = now,
            ExpectedReturn = now.AddHours(hours),
            ActualReturn = null,
            LateFee = 0m,
            TotalPrice = bike.PricePerHour * hours
        };

        bike.Status = "rented";
        _db.TblRentals.Add(rental);
        _db.SaveChanges();
        return new OperationResult(true,
            $"Bike '{bike.Name}' rented for {hours} hour(s). Expected return: {rental.ExpectedReturn:yyyy-MM-dd HH:mm}.");
    }

    // ---------- Checkout ----------
    public List<TblRental> GetActiveRentals(int userId)
    {
        return _db.TblRentals
            .Include(r => r.Bike)
            .Where(r => r.UserId == userId && r.ActualReturn == null)
            .OrderBy(r => r.RentalId)
            .ToList();
    }

    public List<TblRental> GetRentalsByUser(int userId)
    {
        return _db.TblRentals
            .Include(r => r.Bike)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.RentalId)
            .ToList();
    }

    public TblRental? GetRental(int rentalId)
    {
        return _db.TblRentals
            .Include(r => r.Bike)
            .Include(r => r.User)
            .FirstOrDefault(r => r.RentalId == rentalId);
    }

    // ---------- Return ----------
    public OperationResult ReturnBike(int rentalId)
    {
        var rental = _db.TblRentals
            .Include(r => r.Bike)
            .FirstOrDefault(r => r.RentalId == rentalId);

        if (rental is null)
            return new OperationResult(false, "Rental not found.");

        if (rental.ActualReturn.HasValue)
            return new OperationResult(false, "This rental has already been returned.");

        var now = DateTime.Now;
        var lateHours = (now - rental.ExpectedReturn).TotalHours;
        var lateFee = 0m;

        if (lateHours > 0)
        {
            lateFee = decimal.Round(rental.Bike.PricePerHour * (decimal)lateHours, 2);
        }

        rental.ActualReturn = now;
        rental.LateFee = lateFee;
        rental.TotalPrice += lateFee;
        rental.Bike.Status = "available";

        _db.SaveChanges();

        return new OperationResult(true, lateFee > 0
            ? $"Returned late by {lateHours:0.##} hour(s). Late fee {lateFee:N2} added. Total charged: {rental.TotalPrice:N2}."
            : $"Returned on time, no late fee. Total charged: {rental.TotalPrice:N2}.");
    }

    // ---------- Admin: bike management ----------
    public OperationResult AddBike(string name, string type, decimal pricePerHour)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(type))
            return new OperationResult(false, "Bike name and type are required.");

        if (pricePerHour <= 0)
            return new OperationResult(false, "Rental price must be greater than zero.");

        _db.TblBikes.Add(new TblBike
        {
            Name = name.Trim(),
            Type = type.Trim(),
            PricePerHour = pricePerHour,
            Status = "available",
            Condition = "good"
        });
        _db.SaveChanges();
        return new OperationResult(true, $"Bike '{name.Trim()}' added successfully.");
    }

    public OperationResult UpdateBike(int bikeId, string? status, decimal? pricePerHour, string? condition)
    {
        var bike = _db.TblBikes.Find(bikeId);
        if (bike is null)
            return new OperationResult(false, "Bike not found.");

        if (!string.IsNullOrWhiteSpace(status))
        {
            var s = status.Trim().ToLower();
            if (s is not ("available" or "rented"))
                return new OperationResult(false, "Status must be 'available' or 'rented'.");
            bike.Status = s;
        }

        if (pricePerHour.HasValue)
        {
            if (pricePerHour.Value <= 0)
                return new OperationResult(false, "Rental price must be greater than zero.");
            bike.PricePerHour = pricePerHour.Value;
        }

        if (!string.IsNullOrWhiteSpace(condition))
        {
            var c = condition.Trim().ToLower();
            if (c is not ("new" or "good" or "old" or "broken"))
                return new OperationResult(false, "Condition must be 'new', 'good', 'old' or 'broken'.");
            bike.Condition = c;
        }

        _db.SaveChanges();
        return new OperationResult(true, $"Bike '{bike.Name}' updated successfully.");
    }

    public OperationResult RemoveBikesByCondition(string condition)
    {
        var c = condition.Trim().ToLower();
        if (c is not ("old" or "broken"))
            return new OperationResult(false, "Removal condition must be 'old' or 'broken'.");

        var bikes = _db.TblBikes.Where(b => b.Condition == c).ToList();
        if (bikes.Count == 0)
            return new OperationResult(false, $"No bikes found with condition '{c}'.");

        foreach (var bike in bikes)
        {
            var rentals = _db.TblRentals.Where(r => r.BikeId == bike.BikeId).ToList();
            _db.TblRentals.RemoveRange(rentals);
            _db.TblBikes.Remove(bike);
        }

        _db.SaveChanges();
        return new OperationResult(true, $"Removed {bikes.Count} bike(s) with condition '{c}' (and their rental records).");
    }

    // ---------- Admin: reports ----------
    public List<TblRental> GetPastRentals()
    {
        return _db.TblRentals
            .Include(r => r.Bike)
            .Include(r => r.User)
            .Where(r => r.ActualReturn.HasValue)
            .OrderByDescending(r => r.RentDatetime)
            .ToList();
    }

    public List<(DateTime Date, decimal Total)> GetDailyTotals()
    {
        return _db.TblRentals
            .AsNoTracking()
            .AsEnumerable()
            .GroupBy(r => r.RentDatetime.Date)
            .Select(g => (Date: g.Key, Total: g.Sum(r => r.TotalPrice)))
            .OrderBy(g => g.Date)
            .ToList();
    }
}
