using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRent.Domain.Models.Bike
{
    public class BikePatchRequestModel
    {
        public int BikeId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
