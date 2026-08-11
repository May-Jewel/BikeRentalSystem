using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeRent.Database.AppDbContextModels;
using BikeRent.Domain.Models.Bike;
using Microsoft.EntityFrameworkCore;


namespace BikeRent.Domain.Features.Bike
{
    public class BikeService
    {
        private readonly AppDbContext _db;

        public BikeService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<TblBike>> GetBikesAsync()
        {
            return await _db.TblBikes.AsNoTracking().ToListAsync();
        }

        public async Task<TblBike?> GetBikeAsync(int id)
        {
            return await _db.TblBikes.AsNoTracking().FirstOrDefaultAsync(b => b.BikeId == id);
        }

        public async Task<List<TblBike>> GetAvailableBikesAsync()
        {
            return await _db.TblBikes.Where(b => b.Status == "available").ToListAsync();
        }

        public async Task<TblBike> CreateBikeAsync(BikeCreateRequestModel request)
        {
            var bike = new TblBike
            {
                Name = request.Name,
                Type = request.Type,
                PricePerHour = request.PricePerHour,
                Status = "available"
            };

            _db.TblBikes.Add(bike);
            await _db.SaveChangesAsync();
            return bike;
        }

        public async Task<bool> EditBikeAsync(BikeEditRequestModel request)
        {
            var bike = await _db.TblBikes.FindAsync(request.BikeId);
            if (bike == null) return false;

            bike.Name = request.Name;
            bike.Type = request.Type;
            bike.PricePerHour = request.PricePerHour;
            bike.Status = request.Status;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PatchStatusAsync(BikePatchRequestModel request)
        {
            var bike = await _db.TblBikes.FindAsync(request.BikeId);
            if (bike == null) return false;

            bike.Status = request.Status;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBikeAsync(int id)
        {
            var bike = await _db.TblBikes.FindAsync(id);
            if (bike == null) return false;

            _db.TblBikes.Remove(bike);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
