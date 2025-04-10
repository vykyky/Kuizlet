using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Domain.Exceptions
{
    public class CardSetNotFoundException : Exception
    {
        public CardSetNotFoundException(string message) : base(message)
        {
        }
    }
}
