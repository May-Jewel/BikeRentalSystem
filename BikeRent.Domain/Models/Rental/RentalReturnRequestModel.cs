using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRent.Domain.Models.Rental
{
    public class RentalReturnRequestModel
    {
        public int RentalId { get; set; }
        public DateTime ActualReturn { get; set; } = DateTime.Now;
    }
}
