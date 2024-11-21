using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.DTOs
{
    public class ChooseOfferRequest
    {
        public int OfferId { get; set; }
        public int UserId { get; set; }
    }
}