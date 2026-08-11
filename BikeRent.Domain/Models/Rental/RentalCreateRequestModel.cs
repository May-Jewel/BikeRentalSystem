using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRent.Domain.Models.Rental
{
    public class RentalCreateRequestModel
    {
        public int UserId { get; set; }
        public int BikeId { get; set; }
        public DateTime RentDatetime { get; set; } = DateTime.Now;
        public DateTime ExpectedReturn { get; set; }
    }
}
