using CarRental.WebAPI.Data.Models;

namespace WebAPI.Data.Mappers
{
    public class CarProviderMapper: IMapper<CarProvider, CarProviderDTO>
    {
        public CarProviderDTO ToDto(CarProvider carProvider)
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