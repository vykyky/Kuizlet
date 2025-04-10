using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Application.Models
{
    public class ResponseDto
    {
        public string Response { get; set; }
        public string Message { get; set; }

        public ResponseDto(string response, string message)
        {
            Response = response;
            Message = message;
        }
    }
}
