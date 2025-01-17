using WebAPI.Data.DTOs;
using WebAPI.Data.Models;

namespace WebAPI.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(User u)
    {
        return new UserDto{
            UserId = u.UserId,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            BirthDate = u.BirthDate
        };
    }
}