using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Application.Models
{
    public class RegisterRequestDto
    {
        public string Name { get; set; }      // Было: FullName
        public string Login { get; set; }     // Было: Login
        public string Password { get; set; }  // Было: Password
    }
}
