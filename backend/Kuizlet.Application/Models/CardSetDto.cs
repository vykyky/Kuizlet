using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Application.Models
{ 
    public class CardSetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public Guid CreatorId { get; set; }
        public string CreatorName { get; set; }
        public string AccessType { get; set; }

        public CardSetDto(
            Guid id,
            string name,
            bool isPublic,
            Guid creatorId,
            string creatorName,
            string accessType)
        {
            Id = id;
            Name = name;
            IsPublic = isPublic;
            CreatorId = creatorId;
            CreatorName = creatorName;
            AccessType = accessType;
        }
    }
}
