using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MongoDB.Bson;

namespace Kuizlet.Domain.Entities
{
    internal class CardSet
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public List<ObjectId> Cards { get; set; }  // Ссылка на коллекцию Card

        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId CreatorId { get; set; }  // Ссылка на пользователя, который создал набор


        public List<ObjectId> ApprovedUserIds { get; set; }  // Ссылка на пользователей, которые могут использовать набор

        public bool IsPublic { get; set; }

        public string FirstLanguage { get; set; }

        public string SecondLanguage { get; set; }

        // Переопределение Equals и GetHashCode для корректной работы в коллекциях
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;

            var other = (CardSet)obj;
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
