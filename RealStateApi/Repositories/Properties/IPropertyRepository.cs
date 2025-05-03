using RealStateApi.Dtos;

namespace RealStateApi.Repositories.Properties
{
    public interface IPropertyRepository
    {
        Task<List<PropertyDto>> GetAllAsync();
        Task<PropertyDto?> GetByIdAsync(string id);
        Task<List<PropertyDto>> GetFilteredAsync(string? name, string? address, decimal? minPrice, decimal? maxPrice);
        Task<PropertyDetailDto> GetPropertyDetailIdAsync(string id);
    }
}
