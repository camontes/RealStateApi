using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace RealStateApi.Models
{
    public class PropertyImage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdPropertyImage { get; set; }

        public string IdProperty { get; set; }

        public string File { get; set; }

        public bool Enabled { get; set; }
    }
}
