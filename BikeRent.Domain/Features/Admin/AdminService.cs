using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeRent.Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;

namespace BikeRent.Domain.Features.Admin
{
    public class AdminService
    {
        private readonly AppDbContext _db;

        public AdminService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<decimal> GetDailyRentalTotalAsync(DateTime date)
        {
            return await _db.TblRentals
                .Where(r => r.RentDatetime.Date == date.Date)
                .SumAsync(r => r.TotalPrice);
        }
    }
}
