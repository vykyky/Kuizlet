using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kuizlet.Application.Models;

namespace Kuizlet.Application.Interfaces
{
    public interface ICardSetService
    {
        Task<List<CardSetDto>> GetAllCardSetsAsync();
        Task<CardSetDto> CreateCardSetAsync(CardSetDto cardSetDto);
        Task<CardSetDto?> GetCardSetByIdAsync(Guid cardSetId);
        Task DeleteCardSetAsync(Guid cardSetId);
        Task<CardSetDto> UpdateCardSetAsync(Guid cardSetId, CardSetDto cardSetDto);
    }
}
