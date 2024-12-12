using CarRental.WebAPI.Data.Models;

namespace WebAPI.Data.Mappers
{
    public static class CarProviderMapper
    {
        public static CarProviderDTO ToDto(CarProvider carProvider)
        {
            var carProviderDto = new CarProviderDTO{
                CarProviderId = carProvider.CarProviderId,
                Name = carProvider.Name,
                ContactEmail = carProvider.ContactEmail,
                ContactPhone = carProvider.ContactEmail
            };

            return carProviderDto;
        }
    }
}