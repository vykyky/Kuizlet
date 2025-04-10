using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Application.Models
{
    public class CardDto
    {
        public Guid Id { get; set; }
        public string Term { get; set; }
        public string Definition { get; set; }

        public CardDto() { }
        public CardDto(Guid id, string term, string definition) 
        {
            Id = id;
            Term = term;
            Definition = definition;
        }
    }
}
