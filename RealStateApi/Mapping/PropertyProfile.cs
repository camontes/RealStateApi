using AutoMapper;
using RealStateApi.Dtos;
using RealStateApi.Models;

namespace RealStateApi.Mapping
{
    public class PropertyProfile : Profile
    {
        public PropertyProfile() {
            CreateMap<Property, PropertyDto>();
        }
    }
}
