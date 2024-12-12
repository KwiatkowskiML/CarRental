namespace WebAPI.Data.Mappers
{
    public interface IMapper<in TSource, out TDto>
    {
        TDto ToDto(TSource source);
    }
}