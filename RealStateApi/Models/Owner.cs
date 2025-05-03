using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace RealStateApi.Models
{
    public class Owner
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdOwner { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Photo { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Birthday { get; set; }
    }
}
