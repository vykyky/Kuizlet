using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MongoDB.Bson;

namespace Kuizlet.Domain.Entities
{
    public class Card
    {
        public Guid Id { get; set; }
        public string Term { get; set; }
        public string Definition { get; set; }
        public Guid CardSetId { get; set; }
    }
}
