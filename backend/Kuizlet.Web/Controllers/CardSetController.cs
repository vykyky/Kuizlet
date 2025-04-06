using Kuizlet.Application.Interfaces;
using Kuizlet.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kuizlet.Web.Controllers
{
   
    [ApiController]
    [Route("cardsets")]
    public class CardSetsController : ControllerBase
    {
        private readonly ICardSetService _cardSetService;

        public CardSetsController(ICardSetService cardSetService)
        {
            _cardSetService = cardSetService;
        }

        [HttpGet("all")] //работает(для home и library)
        public async Task<ActionResult<List<CardSetDto>>> GetAllCardSets()
        {
            var cardSets = await _cardSetService.GetAllCardSetsAsync();
            return Ok(cardSets);
        }

        [HttpPost]  //работает(для library)
        public async Task<ActionResult<CardSetDto>> CreateCardSet([FromBody] CardSetDto cardSetDto)
        {
            var createdCardSet = await _cardSetService.CreateCardSetAsync(cardSetDto);
            return CreatedAtAction(nameof(GetCardSetById), new { cardSetId = createdCardSet.Id }, createdCardSet);
        }

        [HttpGet("{cardSetId}")] // работает
        public async Task<ActionResult<CardSetDto>> GetCardSetById(Guid cardSetId)
        {
            var cardSet = await _cardSetService.GetCardSetByIdAsync(cardSetId);
            if (cardSet == null)
            {
                return NotFound();
            }
            return Ok(cardSet);
        }

        [HttpDelete("{cardSetId}")] //работает(для создателя)
        public async Task<IActionResult> DeleteCardSet(Guid cardSetId)
        {
            await _cardSetService.DeleteCardSetAsync(cardSetId);
            return NoContent();
        }

        [HttpPut("{cardSetId}")] //работает(для создателя)
        public async Task<ActionResult<CardSetDto>> UpdateCardSet(Guid cardSetId, [FromBody] CardSetDto cardSetDto)
        {
            if (cardSetId != cardSetDto.Id)
            {
                return BadRequest();
            }

            var updatedCardSet = await _cardSetService.UpdateCardSetAsync(cardSetId, cardSetDto);
            return Ok(updatedCardSet);
        }
    }
}
