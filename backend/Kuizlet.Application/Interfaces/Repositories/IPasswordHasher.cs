using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Application.Interfaces.Repositories
{
    public interface IPasswordHasher
    {
        (string Hash, string Salt) CreateHashWithSalt(string password);
        bool Verify(string password, string hash, string salt);
    }
}
