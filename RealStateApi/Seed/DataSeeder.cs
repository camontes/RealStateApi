using MongoDB.Driver;
using RealStateApi.Data;
using RealStateApi.Models;

namespace RealStateApi.Seed
{
    public class DataSeeder
    {
        private readonly MongoContext _context;

        public DataSeeder(MongoContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            var owners = new List<Owner>();
            var properties = new List<Property>();
            var propertyImages = new List<PropertyImage>();
            var propertyTraces = new List<PropertyTrace>();

            if (await _context.Owners.CountDocumentsAsync(Builders<Owner>.Filter.Empty) == 0)
            {
                // 1. Insertar Owners
                owners = new List<Owner>
                {
                    new Owner
                    {
                        Name = "Jan Piere",
                        Address = "Street 34-23",
                        Photo = "https://randomuser.me/api/portraits/men/4.jpg",
                        Birthday = new DateTime(1980, 5, 15)
                    },
                    new Owner
                    {
                        Name = "Bob Shirani",
                        Address = "Avenue 34 south",
                        Photo = "https://randomuser.me/api/portraits/men/1.jpg",
                        Birthday = new DateTime(1997, 11, 15)
                    }
                };
                await _context.Owners.InsertManyAsync(owners);

            }
            if (await _context.Properties.CountDocumentsAsync(Builders<Property>.Filter.Empty) == 0)
            {
                properties = new List<Property>
                {
                    new Property
                    {
                        IdOwner = owners[0].IdOwner,
                        Name = "Casa Moderna",
                        Address = "Av. Libertad 456",
                        Price = 350000,
                        CodeInternal = "CASA-001",
                        Year = 2020
                    },
                    new Property
                    {
                        IdOwner = owners[1].IdOwner,
                        Name = "Apartamento Urbano",
                        Address = "Calle Centro 101",
                        Price = 220000,
                        CodeInternal = "APTO-002",
                        Year = 2018
                    },
                    new Property
                    {
                        IdOwner = owners[0].IdOwner,
                        Name = "Casa de Campo",
                        Address = "Camino Rural 789",
                        Price = 180000,
                        CodeInternal = "CASA-003",
                        Year = 2015
                    },
                    new Property
                    {
                        IdOwner = owners[1].IdOwner,
                        Name = "Apartamento Vista Mar",
                        Address = "Playa Norte 202",
                        Price = 300000,
                        CodeInternal = "APTO-004",
                        Year = 2021
                    },
                    new Property
                    {
                        IdOwner = owners[0].IdOwner,
                        Name = "Casa Familiar",
                        Address = "Barrio Norte 123",
                        Price = 400000,
                        CodeInternal = "CASA-005",
                        Year = 2019
                    },
                    new Property
                    {
                        IdOwner = owners[1].IdOwner,
                        Name = "Apartamento Lujo",
                        Address = "Calle Principal 456",
                        Price = 500000,
                        CodeInternal = "APTO-006",
                        Year = 2022
                    }
                };

                await _context.Properties.InsertManyAsync(properties);
            }

            if (await _context.PropertyImages.CountDocumentsAsync(Builders<PropertyImage>.Filter.Empty) == 0)
            {
                propertyImages = new List<PropertyImage>
                {
                    new PropertyImage
                    {
                        IdProperty = properties[0].IdProperty,
                        File = "https://images.pexels.com/photos/1546168/pexels-photo-1546168.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2",
                        Enabled = true
                    },
                    new PropertyImage
                    {
                        IdProperty = properties[1].IdProperty,
                        File = "https://images.pexels.com/photos/280222/pexels-photo-280222.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2",
                        Enabled = true
                    },
                    new PropertyImage
                    {
                        IdProperty = properties[2].IdProperty,
                        File = "https://images.pexels.com/photos/1396132/pexels-photo-1396132.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2",
                        Enabled = true
                    },
                    new PropertyImage
                    {
                        IdProperty = properties[3].IdProperty,
                        File = "https://images.pexels.com/photos/210617/pexels-photo-210617.jpeg?auto=compress&cs=tinysrgb&w=600",
                        Enabled = true
                    },
                    new PropertyImage
                    {
                        IdProperty = properties[4].IdProperty,
                        File = "https://images.pexels.com/photos/280221/pexels-photo-280221.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2",
                        Enabled = true
                    },
                    new PropertyImage
                    {
                        IdProperty = properties[5].IdProperty,
                        File = "https://images.pexels.com/photos/280222/pexels-photo-280222.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2",
                        Enabled = true
                    }
                };

                await _context.PropertyImages.InsertManyAsync(propertyImages);
            }

            if(await _context.PropertyTraces.CountDocumentsAsync(Builders<PropertyTrace>.Filter.Empty) == 0)
            {
                propertyTraces = new List<PropertyTrace>
                {
                    new PropertyTrace
                    {
                        IdProperty = properties[0].IdProperty,
                        DateSale = new DateTime(2023, 1, 15),
                        Name = "Reparación de cocina",
                        Value = 350000,
                        Tax = 0.06m
                    },
                    new PropertyTrace
                    {
                        IdProperty = properties[1].IdProperty,
                        DateSale = new DateTime(2023, 2, 20),
                        Name = "Reparación de baño",
                        Value = 220000,
                        Tax = 0.07m
                    },
                    new PropertyTrace
                    {
                        IdProperty = properties[2].IdProperty,
                        DateSale = new DateTime(2023, 3, 10),
                        Name = "Reparación de jardín",
                        Value = 180000,
                        Tax = 0.08m
                    },
                    new PropertyTrace
                    {
                        IdProperty = properties[3].IdProperty,
                        DateSale = new DateTime(2023, 4, 5),
                        Name = "Reparación de piscina",
                        Value = 300000,
                        Tax = 0.09m
                    },
                    new PropertyTrace
                    {
                        IdProperty = properties[4].IdProperty,
                        DateSale = new DateTime(2023, 5, 25),
                        Name = "Reparación de techos",
                        Value = 400000,
                        Tax = 0.05m
                    },
                    new PropertyTrace
                    {
                        IdProperty = properties[5].IdProperty,
                        DateSale = new DateTime(2023, 6, 30),
                        Name = "Reparación de ventanas",
                        Value = 500000,
                        Tax = 0.04m
                    }
                };

                await _context.PropertyTraces.InsertManyAsync(propertyTraces);
            }
        }
    }
}
