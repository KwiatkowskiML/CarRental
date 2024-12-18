using WebAPI.Data.DTOs;
using WebAPI.Data.Models;

namespace WebAPI.Mappers;

public static class CarMapper
{
    public static CarDto ToDto(Car c)
    {
        var carDto = new CarDto{
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
            CarProvider = CarProviderMapper.ToDto(c.CarProvider!) // get rid of nullable fields
        };

        return carDto;
    }
}