using CarRental.WebAPI.Data.Models;

namespace WebAPI.Data.Maps
{
    public static class Mapper
    {
        public static OfferDTO OfferToDTO(Offer offer)
        {
            var offerDTO = new OfferDTO{
                OfferId = offer.OfferId,
                TotalPrice = offer.TotalPrice,
                CustomerId = offer.CustomerId,
                StartDate = offer.StartDate,
                EndDate = offer.EndDate,
                CreatedAt = offer.CreatedAt,
                HasGps = offer.HasGps,
                HasChildSeat = offer.HasChildSeat,
                Insurance = offer.Insurance,
                Car = CarToDTO(offer.Car!)
            };

            return offerDTO;
        }

        public static CarProviderDTO CarProviderToDTO(CarProvider carProvider)
        {
            var carProviderDTO = new CarProviderDTO{
                CarProviderId = carProvider.CarProviderId,
                Name = carProvider.Name,
                ContactEmail = carProvider.ContactEmail,
                ContactPhone = carProvider.ContactEmail
            };

            return carProviderDTO;
        }

        public static CarDTO CarToDTO(Car c)
        {
            var carDTO = new CarDTO{
                CarId = c.CarId,
                LicensePlate = c.LicensePlate,
                Brand = c.Brand,
                Model = c.Model,
                Year = c.Year,
                Status = c.Status,
                Location = c.Location,
                EngineCapacity = c.EngineCapacity,
                Power = c.Power,
                FuelType = c.FuelType,
                Description = c.Description,
                CarProvider = CarProviderToDTO(c.CarProvider!)
            };

            return carDTO;
        }

        public static RentalDTO RentalToDTO(Rental r)
        {
            return new RentalDTO{
                RentalId = r.RentalId,
                OfferId = r.OfferId,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                Offer = OfferToDTO(r.Offer)
            };
        }
    }
}