using Kuizlet.Application.Interfaces;
using Kuizlet.Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kuizlet.Web.Controllers
{
    [Route("cards")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly ICardService _cardService;

        public CardsController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [HttpGet("allCards/{cardSetId}")]  // получение всех карточек набора для всех
        public async Task<ActionResult<List<CardDto>>> GetAllCardsByCardSetId(Guid cardSetId)
        {
            var cards = await _cardService.GetCardsByCardSetIdAsync(cardSetId);
            return Ok(cards);
        }

        [HttpPost("{cardSetId}")] //добавление карточки почему то для всех работает
        public async Task<ActionResult<CardDto>> AddCard(Guid cardSetId, [FromBody] CardDto cardDto)
        {
            var createdCard = await _cardService.AddCardAsync(cardSetId, cardDto);
            return Ok(createdCard);
        }

        [HttpPut("{cardId}")] //редактирование карточки ????? тоже для всех
        public async Task<ActionResult<CardDto>> UpdateCard(Guid cardId, [FromBody] CardDto cardDto)
        {
            var updatedCard = await _cardService.UpdateCardAsync(cardId, cardDto);
            return Ok(updatedCard);
        }
    }
}
