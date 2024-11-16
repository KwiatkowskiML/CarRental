using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.DTOs
{
    public class RentalCalculationRequest
    {
        public int CarId { get; set; }
        public int UserId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int InsuranceId { get; set; }
        public bool HasGps { get; set; }
        public bool HasChildSeat { get; set; }
    }
}