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
        public List<Guid?> ApprovedUserIds { get; set; } = new List<Guid?>();

    }
}
