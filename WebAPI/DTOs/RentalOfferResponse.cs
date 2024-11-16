using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.DTOs
{
    public class RentalOfferResponse
    {
        public decimal TotalPrice { get; set; }
        public string InsuranceType { get; set; }
        public bool HasGps { get; set; }
        public bool HasChildSeat { get; set; }
        public CarProviderDTO CarProvider { get; set; }
    }
}