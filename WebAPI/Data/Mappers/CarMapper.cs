using CarRental.WebAPI.Data.Models;

namespace WebAPI.Data.Mappers
{
    public static class CarMapper
    {
        public static CarDTO ToDto(Car c)
        {
            var carDto = new CarDTO{
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
                CarProvider = CarProviderMapper.ToDto(c.CarProvider)
            };

            return carDto;
        }
    }
}