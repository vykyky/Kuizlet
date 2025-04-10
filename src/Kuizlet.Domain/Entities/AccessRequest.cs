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
        public Guid Id { get; set; }
        public Guid RequesterId { get; set; }

        public Guid CardSetId { get; set; }

        public string Status { get; set; }

    }
}
