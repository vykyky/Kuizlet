using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MongoDB.Bson;

namespace Kuizlet.Domain.Entities
{
    public class CardSet
    {
        public Guid Id { get; set; } 
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public Guid CreatorId { get; set; }   

        public List<Guid> CardIds { get; set; } = new List<Guid>();
        public List<Guid?> ApprovedUserIds { get; set; } = new List<Guid?>();// Ссылка на пользователей, которые могут использовать набор

        public override bool Equals(object obj)
        {
            // 1. Проверка на null и сравнение типов
            if (obj == null || GetType() != obj.GetType())
                return false;

            // 2. Приведение типа и сравнение Id
            User other = (User)obj;
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode(); // Хеш-код на основе Guid
        }

    }
}
