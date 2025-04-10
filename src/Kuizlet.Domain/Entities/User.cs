using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Kuizlet.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; } // Добавляем поле для соли
        public string FullName { get; set; }
    }
}
