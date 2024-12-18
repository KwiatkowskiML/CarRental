using WebAPI.Data.DTOs;
using WebAPI.Data.Models;

namespace WebAPI.Mappers;

public static class CustomerMapper
{
    public static CustomerDto ToDto(Customer c)
    {
        return new CustomerDto
        {
            CustomerId = c.CustomerId,
            DrivingLicenseYears = c.DrivingLicenseYears,
            UserDto = UserMapper.ToDto(c.User!),
        };
    }
}