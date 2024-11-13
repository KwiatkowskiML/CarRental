namespace CarRental.WebAPI.Data.Repositories.Interfaces
{
    public interface ICarRentalRepository
    {
        Task<List<Car>> GetAllCarsAsync();
        Task<List<Car>> GetAvailableCarsAsync(CarFilter filter);
        Task<Car?> GetCarByIdAsync(int carId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<List<Rental>> GetUserRentalsAsync(int userId);
        Task<bool> CreateRentalAsync(Rental rental);
        Task<bool> ProcessReturnAsync(Return carReturn);
    }
}