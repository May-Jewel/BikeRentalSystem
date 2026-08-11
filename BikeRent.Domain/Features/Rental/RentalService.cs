using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeRent.Database.AppDbContextModels;
using BikeRent.Domain.Models.Rental;
using Microsoft.EntityFrameworkCore;

namespace BikeRent.Domain.Features.Rental
{
    public class RentalService
    {
        private readonly AppDbContext _db;

        public RentalService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<TblRental?> RentBikeAsync(RentalCreateRequestModel request)
        {
            var bike = await _db.TblBikes.FindAsync(request.BikeId);
            if (bike == null || bike.Status != "available") return null;

            // Calculate upfront total price based on expected rental duration in hours
            var totalHours = (decimal)(request.ExpectedReturn - request.RentDatetime).TotalHours;
            if (totalHours <= 0) totalHours = 1; // Default minimum 1 hour

            var initialPrice = totalHours * bike.PricePerHour;

            var rental = new TblRental
            {
                UserId = request.UserId,
                BikeId = request.BikeId,
                RentDatetime = request.RentDatetime,
                ExpectedReturn = request.ExpectedReturn,
                TotalPrice = initialPrice,
                LateFee = 0.00m
            };

            // Update bike status to rented
            bike.Status = "rented";

            _db.TblRentals.Add(rental);
            await _db.SaveChangesAsync();
            return rental;
        }

        public async Task<TblRental?> ReturnBikeAsync(RentalReturnRequestModel request)
        {
            var rental = await _db.TblRentals.Include(r => r.Bike).FirstOrDefaultAsync(r => r.RentalId == request.RentalId);
            if (rental == null || rental.ActualReturn != null) return null;

            rental.ActualReturn = request.ActualReturn;

            // Calculate late fee if returned after expected time (e.g. 1.5x hourly rate for late hours)
            if (rental.ActualReturn > rental.ExpectedReturn)
            {
                var lateHours = (decimal)(rental.ActualReturn.Value - rental.ExpectedReturn).TotalHours;
                rental.LateFee = lateHours * (rental.Bike.PricePerHour * 1.5m);
                rental.TotalPrice += rental.LateFee;
            }

            // Return bike back to available status
            rental.Bike.Status = "available";

            await _db.SaveChangesAsync();
            return rental;
        }

        public async Task<List<TblRental>> GetRentalHistoryAsync()
        {
            return await _db.TblRentals
                .Include(r => r.User)
                .Include(r => r.Bike)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<TblRental?> GetRentalAsync(int id)
        {
            return await _db.TblRentals
                .Include(r => r.User)
                .Include(r => r.Bike)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RentalId == id);
        }

        public async Task<bool> DeleteRentalAsync(int id)
        {
            var rental = await _db.TblRentals.Include(r => r.Bike).FirstOrDefaultAsync(r => r.RentalId == id);
            if (rental == null) return false;

            // If the bike is still out on this rental, restore it to available status
            if (rental.ActualReturn == null && rental.Bike != null)
            {
                rental.Bike.Status = "available";
            }

            _db.TblRentals.Remove(rental);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
