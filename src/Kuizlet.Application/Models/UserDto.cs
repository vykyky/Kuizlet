using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Application.Models
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Login { get; set; }

        public UserDto(Guid id, string login)
        {
            Id = id;
            Login = login;
        }
    }
}
