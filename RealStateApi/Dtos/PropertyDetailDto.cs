namespace RealStateApi.Dtos
{
    public class PropertyDetailDto
    {
        public OwnerDto Owner { get; set; }
        public List<PropertyImageDto> Images { get; set; }
        public List<PropertyTraceDto> Traces { get; set; }
    }
}
