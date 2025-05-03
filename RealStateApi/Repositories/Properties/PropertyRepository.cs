using AutoMapper;
using MongoDB.Driver;
using RealStateApi.Data;
using RealStateApi.Dtos;
using RealStateApi.Models;
using RealStateApi.Repositories.Owners;
using RealStateApi.Repositories.propertyImages;
using RealStateApi.Repositories.PropertyTraces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealStateApi.Repositories.Properties
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly MongoContext _context;
        private readonly IPropertyImageRepository _propertyImageRepository;
        private readonly IPropertyTraceRepository _propertyTraceRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IMapper _mapper;

        public PropertyRepository(
            MongoContext context,
            IMapper mapper,
            IOwnerRepository ownerRepository,
            IPropertyImageRepository propertyImageRepository,
            IPropertyTraceRepository propertyTraceRepository
            )
        {
            _context = context;
            _mapper = mapper;
            _ownerRepository = ownerRepository;
            _propertyImageRepository = propertyImageRepository;
            _propertyTraceRepository = propertyTraceRepository;
        }


        /// <summary>
        /// get all properties
        /// </summary>
        /// <returns>return List<PropertyDto></returns>
        /// <exception cref="RepositoryException"></exception>
        public async Task<List<PropertyDto>> GetAllAsync()
        {
            try
            {

                var properties = await _context.Properties.Find(_ => true).ToListAsync();

                if (properties == null || properties.Count == 0)
                {
                    return new List<PropertyDto>();
                }

                return _mapper.Map<List<PropertyDto>>(properties);
            }
            catch (MongoException ex)
            {
                throw new RepositoryException("Could not retrieve properties due to database error", ex);
            }
            catch (AutoMapperMappingException ex)
            {
                throw new RepositoryException("Error processing property data", ex);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// get property by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>PropertyDto</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="RepositoryException"></exception>
        public async Task<PropertyDto?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Property ID cannot be empty", nameof(id));
            }

            try
            {

                var property = await _context.Properties
                    .Find(p => p.IdProperty == id)
                    .FirstOrDefaultAsync();

                if (property == null)
                {
                    return null;
                }

                return _mapper.Map<PropertyDto>(property);
            }
            catch (MongoException ex)
            {
                throw new RepositoryException($"Could not retrieve property with ID {id}", ex);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// Get all properties filtered by name, address, minPrice and maxPrice
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <returns>List<PropertyDto></returns>
        /// <exception cref="RepositoryException"></exception>
        public async Task<List<PropertyDto>> GetFilteredAsync(string? name, string? address, decimal? minPrice, decimal? maxPrice)
        {
            try
            {
                // Validate price range
                if (minPrice.HasValue && minPrice < 0)
                    throw new ArgumentException("Minimum price cannot be negative", nameof(minPrice));

                if (maxPrice.HasValue && maxPrice < 0)
                    throw new ArgumentException("Maximum price cannot be negative", nameof(maxPrice));

                if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                    throw new ArgumentException("Minimum price cannot be greater than maximum price");


                var filterBuilder = Builders<Property>.Filter;
                var filters = new List<FilterDefinition<Property>>();

                if (!string.IsNullOrEmpty(name))
                    filters.Add(filterBuilder.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(name, "i")));

                if (!string.IsNullOrEmpty(address))
                    filters.Add(filterBuilder.Regex(p => p.Address, new MongoDB.Bson.BsonRegularExpression(address, "i")));

                if (minPrice.HasValue)
                    filters.Add(filterBuilder.Gte(p => p.Price, minPrice.Value));

                if (maxPrice.HasValue)
                    filters.Add(filterBuilder.Lte(p => p.Price, maxPrice.Value));

                var combinedFilter = filters.Count > 0
                    ? filterBuilder.And(filters)
                    : filterBuilder.Empty;

                var properties = await _context.Properties.Find(combinedFilter).ToListAsync();

                if (properties == null || properties.Count == 0)
                {
                    return new List<PropertyDto>();
                }

                return _mapper.Map<List<PropertyDto>>(properties);
            }
            catch (MongoException ex)
            {
                throw new RepositoryException("Could not retrieve filtered properties due to database error", ex);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// get detail of property by id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>PropertyDetailDto</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="RepositoryException"></exception>
        public async Task<PropertyDetailDto> GetPropertyDetailIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Property ID cannot be empty", nameof(id));
            }

            try
            {

                var property = await GetByIdAsync(id) ??
                    throw new KeyNotFoundException($"Property with ID {id} not found");

                if (string.IsNullOrWhiteSpace(property.IdOwner))
                    throw new InvalidOperationException($"Property with ID {id} has no owner assigned");

                // Execute all repository calls in parallel
                var imagesTask = _propertyImageRepository.GetImagesByPropertyIdAsync(id);
                var tracesTask = _propertyTraceRepository.GetByPropertyIdAsync(id);
                var ownerTask = _ownerRepository.GetByIdAsync(property.IdOwner);

                await Task.WhenAll(imagesTask, tracesTask, ownerTask);

                var owner = await ownerTask ??
                    throw new KeyNotFoundException($"Owner with ID {property.IdOwner} not found for property {id}");

                return new PropertyDetailDto
                {
                    Images = await imagesTask,
                    Traces = await tracesTask,
                    Owner = owner
                };
            }
            catch (MongoException ex)
            {
                throw new RepositoryException($"Could not retrieve details for property with ID {id}", ex);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public class RepositoryException : Exception
    {
        public RepositoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}