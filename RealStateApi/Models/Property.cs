using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace RealStateApi.Models
{
    public class Property
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public  string IdProperty { get; set; }

        public required string Name { get; set; }
        public required string Address { get; set; }
        public required decimal Price { get; set; }
        public required string CodeInternal { get; set; }
        public int Year { get; set; }
        public required string IdOwner { get; set; }
    }
}
