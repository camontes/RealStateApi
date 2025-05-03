using Microsoft.AspNetCore.Mvc;
using RealStateApi.Repositories.Properties;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ILogger<PropertiesController> _logger;

    public PropertiesController(IPropertyRepository propertyRepository, ILogger<PropertiesController> logger)
    {
        _propertyRepository = propertyRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get all properties
    /// </summary>
    /// <param name="name"></param>
    /// <param name="address"></param>
    /// <param name="minPrice"></param>
    /// <param name="maxPrice"></param>
    /// <returns>return all properties according to the filters</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(
    [FromQuery] string? name,
    [FromQuery] string? address,
    [FromQuery] decimal? minPrice,
    [FromQuery] decimal? maxPrice)
    {
        try
        {
            // parameters validation
            if (minPrice < 0 || maxPrice < 0)
                return BadRequest("Price values cannot be negative");

            if (minPrice > maxPrice)
                return BadRequest("Minimum price cannot be greater than maximum price");

            var properties = await _propertyRepository.GetFilteredAsync(name, address, minPrice, maxPrice);

            _logger.LogInformation("Retrieved {Count} properties", properties.Count);
            return Ok(properties);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument in GetAll properties");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving properties");
            return StatusCode(500, "An error occurred while retrieving properties");
        }
    }

    /// <summary>
    ///  property by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>return a property by Id</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("Property ID is required");
        try
        {

            var property = await _propertyRepository.GetByIdAsync(id);

            if (property == null)
            {
                _logger.LogWarning("Property with ID {PropertyId} not found", id);
                return NotFound($"Property with ID {id} not found");
            }

            _logger.LogInformation("Retrieved property with ID {PropertyId}", id);
            return Ok(property);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving property with ID {PropertyId}", id);
            return StatusCode(500, "An error occurred while retrieving the property");
        }
    }

    /// <summary>
    /// Details of a property by Id (Owner, PropertyImages, PropertyTraces)
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Return an object with peroperty detail (Owner, PropertyImages, PropertyTraces) </returns>
    [HttpGet("PropertyDetail/{id}")]
    public async Task<IActionResult> GetPropertyDetailById(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Property ID is required");

            var propertyDetail = await _propertyRepository.GetPropertyDetailIdAsync(id);

            if (propertyDetail == null)
            {
                _logger.LogWarning("Property details for ID {PropertyId} not found", id);
                return NotFound($"Property details for ID {id} not found");
            }

            _logger.LogInformation("Retrieved property details for ID {PropertyId}", id);
            return Ok(propertyDetail);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found for property ID {PropertyId}", id);
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation for property ID {PropertyId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving property details for ID {PropertyId}", id);
            return StatusCode(500, "An error occurred while retrieving property details");
        }
    }
}