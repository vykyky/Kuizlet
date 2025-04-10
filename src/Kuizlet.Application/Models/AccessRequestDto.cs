using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Application.Models
{
    public class AccessRequestDto
    {
        public Guid Id { get; set; }
        public Guid CardSetId { get; set; }
        public string? CardSetName { get; set; }
        public string? RequesterLogin { get; set; }
        public string Status { get; set; }

        public AccessRequestDto(
            Guid id,
            Guid cardSetId,
            string cardSetName,
            string requesterLogin,
            string status)
        {
            Id = id;
            CardSetId = cardSetId;
            CardSetName = cardSetName;
            RequesterLogin = requesterLogin;
            Status = status;
        }
    }
}
