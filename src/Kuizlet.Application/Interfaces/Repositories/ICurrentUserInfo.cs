using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Application.Interfaces.Repositories
{
    public interface ICurrentUserInfo
    {
        string GetCurrentLogin();
    }
}
