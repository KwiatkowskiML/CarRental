// Data/Repositories/Interfaces/ICarRentalRepository.cs
using CarRental.WebAPI.Data.DTOs;
using CarRental.WebAPI.Data.Models;
using WebAPI.DTOs;
using WebAPI.filters;

namespace CarRental.WebAPI.Data.Repositories.Interfaces
{
    public interface ICarRentalRepository
    {
        Task<List<Car>> GetAllCarsAsync();
        Task<OfferDTO?> GetOffer(OfferFilter request);
        Task<List<Car>> GetAvailableCarsAsync(CarFilter filter);
        Task<Car?> GetCarByIdAsync(int carId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<Customer?> GetCustomerByUserId(int userId);
        Task<List<RentalDTO>> GetUserRentalsAsync(int userId);
        Task<bool> CreateRentalAsync(Rental rental);
        Task<bool> ProcessReturnAsync(Return carReturn);
        Task<Insurance?> GetInsuranceByIdAsync(int insuranceId);
        Task CreateOfferAsync(Offer offer);
        Task<User?> GetUserByEmail(string email);
        Task<User> CreateUser(User user);
        Task<RentalDTO?> CreateRentalFromOfferAsync(int offerId);
        Task<Customer> CreateCustomer(Customer customer);
        Task<RentalDTO?> GetRentalByOfferId(int offerId);


    }
}