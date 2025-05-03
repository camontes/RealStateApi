using AutoMapper;
using MongoDB.Driver;
using RealStateApi.Data;
using RealStateApi.Dtos;

namespace RealStateApi.Repositories.Owners
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly MongoContext _context;
        private readonly IMapper _mapper;

        public OwnerRepository(MongoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// get Owners by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return OwnerDto</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="RepositoryException"></exception>
        public async Task<OwnerDto> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Id cannot be empty or null", nameof(id));
            }
            try
            {
                var response = await _context.Owners
                .Find(p => p.IdOwner == id)
                .FirstOrDefaultAsync();

                return _mapper.Map<OwnerDto>(response);
            }
            catch(MongoException ex)
            {
                throw new RepositoryException("Could not retrieve owner due to database error", ex);
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
