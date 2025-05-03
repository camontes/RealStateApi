using AutoMapper;
using MongoDB.Driver;
using RealStateApi.Data;
using RealStateApi.Dtos;
using RealStateApi.Repositories.Owners;

namespace RealStateApi.Repositories.propertyImages
{
    public class PropertyImageRepository : IPropertyImageRepository
    {
        private readonly MongoContext _context;
        private readonly IMapper _mapper;
        public PropertyImageRepository(IMapper mapper, MongoContext context) {
            _mapper = mapper;
            _context = context;
        }

        /// <summary>
        /// Get PropertyImages by PropertyId
        /// </summary>
        /// <param name="propertyId"></param>
        /// <returns>List<PropertyImageDto></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="RepositoryException"></exception>
        public async Task<List<PropertyImageDto>> GetImagesByPropertyIdAsync(string propertyId)
        {
            if (string.IsNullOrWhiteSpace(propertyId))
            {
                throw new ArgumentException("propertyId cannot be empty or null", nameof(propertyId));
            }
            try
            {
                var response = await _context.PropertyImages
                .Find(p => p.IdProperty == propertyId && p.Enabled)
                .ToListAsync();

                return _mapper.Map<List<PropertyImageDto>>(response);
            }
            catch (MongoException ex)
            {
                throw new RepositoryException("Could not retrieve PropertyImages due to database error", ex);
            }
        }
    }
    public class RepositoryException : Exception
    {
        public RepositoryException(string message) : base(message) { }
        public RepositoryException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
