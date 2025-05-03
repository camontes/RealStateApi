using RealStateApi.Dtos;

namespace RealStateApi.Repositories.propertyImages
{
    public interface IPropertyImageRepository
    {
        Task<List<PropertyImageDto>> GetImagesByPropertyIdAsync(string propertyId);
    }
}
