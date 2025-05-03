using RealStateApi.Dtos;

namespace RealStateApi.Repositories.Owners
{
    public interface IOwnerRepository
    {
        Task<OwnerDto> GetByIdAsync(string id);
    }
}
