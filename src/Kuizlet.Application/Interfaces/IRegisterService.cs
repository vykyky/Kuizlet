using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Application.Interfaces
{
    public interface IRegisterService
    {
        public Task RegisterAsync(string name, string login, string password);
    }
}
