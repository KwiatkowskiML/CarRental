using WebAPI.Data.DTOs;
using WebAPI.Data.Models;

namespace WebAPI.Mappers;

public static class CarProviderMapper
{
    public static CarProviderDto ToDto(CarProvider carProvider)
    {
        var carProviderDto = new CarProviderDto{
            CarProviderId = carProvider.CarProviderId,
            Name = carProvider.Name,
            ContactEmail = carProvider.ContactEmail,
            ContactPhone = carProvider.ContactEmail
        };

        return carProviderDto;
    }
}