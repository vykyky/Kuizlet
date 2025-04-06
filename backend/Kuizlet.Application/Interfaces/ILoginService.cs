using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Application.Interfaces
{
    public interface ILoginService
    {
        public Task<string> LoginAsync(string login, string password);
    }
}
