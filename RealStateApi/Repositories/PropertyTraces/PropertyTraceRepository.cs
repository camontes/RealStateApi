using AutoMapper;
using MongoDB.Driver;
using RealStateApi.Data;
using RealStateApi.Dtos;

namespace RealStateApi.Repositories.PropertyTraces
{
    public class PropertyTraceRepository : IPropertyTraceRepository
    {
        private readonly MongoContext _context;
        private readonly IMapper _mapper;

        public PropertyTraceRepository(MongoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        /// <summary>
        /// Get PropertyTraces by PropertyId
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List<PropertyTraceDto></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="RepositoryException"></exception>
        public async Task<List<PropertyTraceDto>> GetByPropertyIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Id cannot be empty or null", nameof(id));
            }
            try
            {
                var response = await _context.PropertyTraces
                .Find(p => p.IdProperty == id)
                .ToListAsync();

                return _mapper.Map<List<PropertyTraceDto>>(response);
            }
            catch (MongoException ex)
            {
                throw new RepositoryException("Could not retrieve PropertyTraces due to database error", ex);
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
