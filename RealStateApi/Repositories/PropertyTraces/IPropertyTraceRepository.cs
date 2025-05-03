using RealStateApi.Dtos;

namespace RealStateApi.Repositories.PropertyTraces
{
    public interface IPropertyTraceRepository
    {
        Task<List<PropertyTraceDto>> GetByPropertyIdAsync(string id);
    }
}
