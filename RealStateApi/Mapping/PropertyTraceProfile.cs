using AutoMapper;
using RealStateApi.Dtos;
using RealStateApi.Models;

namespace RealStateApi.Mapping
{
    public class PropertyTraceProfile : Profile
    {
        public PropertyTraceProfile()
        {
            CreateMap<PropertyTrace, PropertyTraceDto>();
        }
    }
}
