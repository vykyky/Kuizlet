using Kuizlet.Application.Interfaces;
using Kuizlet.Application.Models;
using Kuizlet.Serializers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

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

        [HttpPost("import/{cardSetId}")]
        public async Task<IActionResult> ImportCards(Guid cardSetId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не был загружен.");

            try
            {
                using var stream = new StreamReader(file.OpenReadStream());
                var content = await stream.ReadToEndAsync();

                IDataSerializer serializer;

                if (file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase) || 
                    file.ContentType == "application/json")
                {
                    serializer = new JsonDataSerializer();  // Для JSON
                }
                else if (file.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) || 
                        file.ContentType == "text/plain")
                {
                    serializer = new TxtDataSerializer();  // Для текстовых данных
                }
                else if (file.FileName.EndsWith(".docx"))
                {
                    serializer = new WordDataSerializer();
                    
                }
                else
                {
                    return BadRequest("Неподдерживаемый формат файла.");
                }
                
                List<CardDto> cards;
                if (serializer is WordDataSerializer w)
                {
                    cards = await w.DeserializeAsync<List<CardDto>>(file.OpenReadStream());
                }
                else{
                    cards = serializer.Deserialize<List<CardDto>>(content);

                }
                if (cards == null || !cards.Any())
                    return BadRequest("Не удалось прочитать карточки из файла.");

                await _cardService.AddCardsAsync(cardSetId, cards);

                return Ok(new { message = "Карточки успешно импортированы", cardCount = cards.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при импорте: {ex.Message}");
            }
        }

        [HttpGet("export/{format}")]
        public async Task<IActionResult> ExportCards([FromRoute] string format, [FromQuery] Guid cardSetId)
        {
            var cards = await _cardService.GetCardsByCardSetIdAsync(cardSetId);

            if (cards == null || !cards.Any())
                return NotFound("Нет карточек для экспорта");

            string fileName = $"cards_{DateTime.Now:yyyyMMdd_HHmmss}";
            string contentType;
            byte[] fileBytes;

            switch (format.ToLower())
            {
                case "txt":
                    var txtSerializer = new TxtDataSerializer();
                    var txt = txtSerializer.Serialize(cards);
                    fileBytes = Encoding.UTF8.GetBytes(txt);
                    contentType = "text/plain";
                    fileName += ".txt";
                    break;

                case "json":
                    var jsonSerializer = new JsonDataSerializer();
                    var json = JsonSerializer.Serialize(cards.Select(card => new {
                    card.Term,
                    card.Definition
                }));

                    fileBytes = Encoding.UTF8.GetBytes(json);
                    contentType = "application/json";
                    fileName += ".json";
                    break;

                case "word":
                case "docx":
                    var wordSerializer = new WordDataSerializer();
                    fileBytes = wordSerializer.SerializeToBytes(cards.Cast<object>());
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    fileName += ".docx";
                    break;
                default:
                    return BadRequest("Неподдерживаемый формат: " + format);
            }

            return File(fileBytes, contentType, fileName);
        }

    }
}
