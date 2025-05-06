using Kuizlet.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Application.Interfaces
{
    public interface ICardService
    {
        Task<List<CardDto>> GetCardsByCardSetIdAsync(Guid cardSetId);
        Task<CardDto> AddCardAsync(Guid cardSetId, CardDto cardDto);
        Task<CardDto> UpdateCardAsync(Guid cardId, CardDto cardDto);
        Task AddCardsAsync(Guid cardSetId, List<CardDto> cards);
    }
}
