using AutoMapper;
using RealStateApi.Dtos;
using RealStateApi.Models;

namespace RealStateApi.Mapping
{
    public class OwnerProfile : Profile
    {
        public OwnerProfile()
        {
            CreateMap<Owner, OwnerDto>();
        }
    }
}
