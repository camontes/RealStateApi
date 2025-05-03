using AutoMapper;
using RealStateApi.Dtos;
using RealStateApi.Models;

namespace RealStateApi.Mapping
{
    public class PropertyImageProfile : Profile
    {
        public PropertyImageProfile()
        {
            CreateMap<PropertyImage, PropertyImageDto>();
        }
    }
}
