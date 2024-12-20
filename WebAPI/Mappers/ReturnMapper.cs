using WebAPI.Data.DTOs;
using WebAPI.Data.Models;

namespace WebAPI.Mappers;

public static class ReturnMapper
{
    public static ReturnDto ToDto(Return r)
    {
        return new ReturnDto{
            ReturnId = r.ReturnId,
            RentalId = r.RentalId,
            ReturnDate = r.ReturnDate,
            ConditionDescription = r.ConditionDescription,
            PhotoUrl = r.PhotoUrl,
            ProcessedBy = r.ProcessedBy,
            CreatedAt = r.CreatedAt
        };
    }
}