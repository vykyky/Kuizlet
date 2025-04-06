using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Kuizlet.Domain.Entities
{
    public class AccessRequest
    {
        public Guid RequesterId { get; }

        public Guid CardSetId { get; }

        public string Status { get;  }
        protected AccessRequest() { }

        public AccessRequest(
            Guid requesterId,
            Guid cardSetId,
            string status)
        {
            RequesterId = requesterId;
            CardSetId = cardSetId;
            Status = status;
        }
    }
}
