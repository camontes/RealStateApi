using Moq;
using MongoDB.Driver;
using RealStateApi.Models;
using RealStateApi.Dtos;
using Mongo2Go;
using AutoMapper;
using RealStateApi.Data;
using RealStateApi.Repositories.Owners;
using RealStateApi.Repositories.Properties;
using RealStateApi.Repositories.propertyImages;
using RealStateApi.Repositories.PropertyTraces;
using Microsoft.Extensions.Configuration;

[TestFixture]
public class PropertyRepositoryIntegrationTests
{
    private MongoDbRunner _runner;
    private MongoContext _context;
    private PropertyRepository _repository;
    private Mock<IMapper> _mockMapper;
    private Mock<IOwnerRepository> _mockOwnerRepo;
    private Mock<IPropertyImageRepository> _mockImageRepo;
    private Mock<IPropertyTraceRepository> _mockTraceRepo;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _runner = MongoDbRunner.Start();

        //Set Up in memory database
        var client = new MongoClient(_runner.ConnectionString);
        var database = client.GetDatabase("TestDB");

        //Set Up configuration
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["MongoDbSettings:ConnectionString"])
                 .Returns(_runner.ConnectionString);
        mockConfig.Setup(c => c["MongoDbSettings:DatabaseName"])
                 .Returns("TestDB");

        _context = new MongoContext(mockConfig.Object);

        // Initialize the mocks
        _mockMapper = new Mock<IMapper>();
        _mockOwnerRepo = new Mock<IOwnerRepository>();
        _mockImageRepo = new Mock<IPropertyImageRepository>();
        _mockTraceRepo = new Mock<IPropertyTraceRepository>();

        _repository = new PropertyRepository(
            _context,
            _mockMapper.Object,
            _mockOwnerRepo.Object,
            _mockImageRepo.Object,
            _mockTraceRepo.Object);
    }

    [Test]
    public async Task GetAllAsync_ReturnsPropertiesFromDatabase()
    {
        // Arrange 
        var testProperties = new List<Property>
    {
        new Property { IdProperty = "5a9427648b0beebeb69579e7", Name = "Casa A", Price = 100000, Address = "", CodeInternal = "", IdOwner = "5a9427648b0beebeb6957910" },
        new Property { IdProperty = "5a9427648b0beebeb69579e1", Name = "Casa B", Price = 100000, Address = "", CodeInternal = "", IdOwner = "5a9427648b0beebeb6957918" }
    };

        await _context.Properties.InsertManyAsync(testProperties);

        //set up mock mapper to return the same properties
        _mockMapper.Setup(m => m.Map<List<PropertyDto>>(It.IsAny<List<Property>>()))
                   .Returns(new List<PropertyDto>
                   {
                   new PropertyDto { IdProperty = "5a9427648b0beebeb69579e7", Name = "Casa A", Price = 100000 },
                   new PropertyDto { IdProperty = "5a9427648b0beebeb69579e7", Name = "Casa B", Price = 200000 }
                   });

        // Act
        var result = await _repository.GetFilteredAsync(null, null, null, null);

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Casa A", result[0].Name);
        Assert.AreEqual(100000, result[0].Price);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsProperty_WhenExists()
    {
        // Arrange
        var testProperty = new Property
        {
            IdProperty = "5a9427648b0beebeb69579e7",
            Name = "Casa C",
            Price = 150000,
            IdOwner = "5a9427648b0beebeb6957910",
            Address = "123 Main St",
            CodeInternal = "ABC123",
        };

        await _context.Properties.InsertOneAsync(testProperty);

        _mockMapper.Setup(m => m.Map<PropertyDto>(It.IsAny<Property>()))
                   .Returns(new PropertyDto
                   {
                       IdProperty = "5a9427648b0beebeb69579e7",
                       Name = "Casa C",
                       Price = 150000
                   });

        // Act
        var result = await _repository.GetByIdAsync("5a9427648b0beebeb69579e7");

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual("Casa C", result.Name);
        Assert.AreEqual(150000, result.Price);
    }

    [Test]
    public async Task GetPropertyDetailIdAsync_ReturnsCompleteDetails_WhenDataExists()
    {
        // Arrange
        var propertyId = "507f1f77bcf86cd799439011";
        var ownerId = "507f191e810c19729de860ea";

        //Insert a property with the same IdProperty
        var testProperty = new Property
        {
            IdProperty = propertyId,
            Name = "Villa Las Palmas",
            IdOwner = ownerId,
            Price = 300000,
            Address = "123 Beach Ave",
            CodeInternal = "VLP123",
        };
        await _context.Properties.InsertOneAsync(testProperty);

        //set up mock mapper to return the same properties
        _mockMapper.Setup(m => m.Map<PropertyDto>(It.Is<Property>(p => p.IdProperty == propertyId)))
                   .Returns(new PropertyDto
                   {
                       IdProperty = propertyId,
                       Name = "Villa Las Palmas",
                       IdOwner = ownerId // ¡Este es el campo crítico que faltaba!
                   });

        var ownerDto = new OwnerDto { IdOwner = ownerId, Name = "María González" };
        _mockOwnerRepo.Setup(r => r.GetByIdAsync(ownerId))
                     .ReturnsAsync(ownerDto);

        var images = new List<PropertyImageDto>
    {
        new PropertyImageDto { IdPropertyImage = "1", File = "villa1.jpg" }
    };
        _mockImageRepo.Setup(r => r.GetImagesByPropertyIdAsync(propertyId))
                     .ReturnsAsync(images);

        var traces = new List<PropertyTraceDto>
    {
        new PropertyTraceDto { IdPropertyTrace = "1", Value = 300000 }
    };
        _mockTraceRepo.Setup(r => r.GetByPropertyIdAsync(propertyId))
                     .ReturnsAsync(traces);

        // Act
        var result = await _repository.GetPropertyDetailIdAsync(propertyId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("María González", result.Owner.Name);
        Assert.AreEqual(1, result.Images.Count);
        Assert.AreEqual(1, result.Traces.Count);
    }

    [TearDown]
    public void TearDown()
    {
        //clean database each time after each test
        _context.Properties.DeleteMany(_ => true);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        //stop Mongo2Go
        _runner.Dispose();
    }
}