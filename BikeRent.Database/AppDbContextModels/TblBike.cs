using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRent.Database.AppDbContextModels
{
     public class TblBike
    {
        private ICollection<TblRental> tblRentals = new List<TblRental>();

        public int BikeId { get; set; }

        public string Name { get; set; } = null!;

        public string Type { get; set; } = null!;

        public decimal PricePerHour { get; set; }

        public string Status { get; set; } = null!;

        public string Condition { get; set; } = "good";

        public virtual ICollection<TblRental> TblRentals { get => tblRentals; set => tblRentals = value; }
    }
}
