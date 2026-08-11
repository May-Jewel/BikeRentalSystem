using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeRent.Database.AppDbContextModels;
using BikeRent.Domain.Models.User;
using Microsoft.EntityFrameworkCore;

namespace BikeRent.Domain.Features.User
{
    public class UserService
    {
        private readonly AppDbContext _db;

        public UserService(AppDbContext db)
        {
            _db = db;
        }

        // Added method to sync with UserController's GetAllUsers endpoint
        public async Task<List<TblUser>> GetAllUsersAsync()
        {
            return await _db.TblUsers.AsNoTracking().ToListAsync();
        }

        public async Task<TblUser?> GetUserAsync(int id)
        {
            return await _db.TblUsers.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<bool> EditUserAsync(UserEditRequestModel request)
        {
            var user = await _db.TblUsers.FindAsync(request.UserId);
            if (user == null) return false;

            var phoneTaken = await _db.TblUsers.AnyAsync(u =>
                u.Phone == request.Phone && u.UserId != request.UserId);
            if (phoneTaken) return false;

            user.Name = request.Name;
            user.Phone = request.Phone;
            user.Password = request.Password;
            user.Role = request.Role;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _db.TblUsers.FindAsync(id);
            if (user == null) return false;

            _db.TblUsers.Remove(user);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<TblUser?> RegisterAsync(UserRegisterRequestModel request)
        {
            var exists = await _db.TblUsers.AnyAsync(u => u.Phone == request.Phone);
            if (exists) return null;

            var user = new TblUser
            {
                Name = request.Name,
                Phone = request.Phone,
                Password = request.Password, // Simple string per schema; hash in production
                Role = request.Role
            };

            _db.TblUsers.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<TblUser?> LoginAsync(UserLoginRequestModel request)
        {
            return await _db.TblUsers.FirstOrDefaultAsync(u =>
                u.Phone == request.Phone && u.Password == request.Password);
        }
    }
}