using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRent.Database.AppDbContextModels
{
    public class TblRental
    {
        public int RentalId { get; set; }

        public int UserId { get; set; }

        public int BikeId { get; set; }

        public DateTime RentDatetime { get; set; }

        public DateTime ExpectedReturn { get; set; }

        public DateTime? ActualReturn { get; set; }

        public decimal LateFee { get; set; }

        public decimal TotalPrice { get; set; }

        public virtual TblBike Bike { get; set; } = null!;

        public virtual TblUser User { get; set; } = null!;
    }
}
