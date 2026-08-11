using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRent.Database.AppDbContextModels
{
    public class TblUser
    {
        public int UserId { get; set; }

        public string Name { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Role { get; set; } = null!;

        public virtual ICollection<TblRental> TblRentals { get; set; } = new List<TblRental>();
    }

}
