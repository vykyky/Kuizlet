using Kuizlet.Application.Interfaces.Repositories;
using Kuizlet.Application.Models;
using Kuizlet.Application.Services;
using Kuizlet.Domain.Entities;
using Kuizlet.Domain.Exceptions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Tests.LogicTests
{
    public class CardServiceTests
    {
        private readonly Mock<ICardRepository> _cardRepoMock = new();
        private readonly Mock<ICardSetRepository> _cardSetRepoMock = new();
        private readonly CardService _cardService;

        public CardServiceTests()
        {
            _cardService = new CardService(
                _cardRepoMock.Object,
                _cardSetRepoMock.Object);
        }
        private Card CreateTestCard(Guid? id = null) => new()
        {
            Id = id ?? Guid.NewGuid(),
            Term = "Test Term",
            Definition = "Test Definition",
            CardSetId = Guid.NewGuid()
        };

        [Fact]
        public async Task GetCardsByCardSetIdAsync_ReturnsCards()
        {
            var cardSetId = Guid.NewGuid();
            var testCards = new List<Card> { CreateTestCard(), CreateTestCard() };

            _cardRepoMock.Setup(x => x.GetByCardSetIdAsync(cardSetId)).ReturnsAsync(testCards);

            var result = await _cardService.GetCardsByCardSetIdAsync(cardSetId);

            Assert.Equal(2, result.Count);
            Assert.Equal(testCards[0].Term, result[0].Term);
            _cardRepoMock.Verify(x => x.GetByCardSetIdAsync(cardSetId), Times.Once);
        }

        [Fact]
        public async Task AddCardAsync_ValidData_CreatesCard()
        {
            var cardSetId = Guid.NewGuid();
            var cardDto = new CardDto(Guid.Empty, "New Term", "New Definition");
            var testCardSet = new CardSet { Id = cardSetId };

            _cardSetRepoMock.Setup(x => x.GetByIdAsync(cardSetId)).ReturnsAsync(testCardSet);

            _cardRepoMock.Setup(x => x.AddAsync(It.IsAny<Card>())).ReturnsAsync((Card c) => c);

            // сам метод
            var result = await _cardService.AddCardAsync(cardSetId, cardDto);

            // проверка
            Assert.Equal(cardDto.Term, result.Term);
            Assert.Equal(cardDto.Definition, result.Definition);
            Assert.NotEqual(Guid.Empty, result.Id);
            _cardRepoMock.Verify(x => x.AddAsync(It.Is<Card>(c =>
                c.CardSetId == cardSetId &&
                c.Term == cardDto.Term &&
                c.Definition == cardDto.Definition
            )), Times.Once);
        }

        [Fact]
        public async Task AddCardAsync_InvalidCardSetId_ThrowsException()
        {
            var invalidCardSetId = Guid.NewGuid();
            var cardDto = new CardDto(Guid.Empty, "Term", "Definition");

            _cardSetRepoMock.Setup(x => x.GetByIdAsync(invalidCardSetId)).ReturnsAsync((CardSet?)null);

            await Assert.ThrowsAsync<CardSetNotFoundException>(() =>
                _cardService.AddCardAsync(invalidCardSetId, cardDto));
        }

        [Fact]
        public async Task UpdateCardAsync_ValidData_UpdatesCard()
        {
            var cardId = Guid.NewGuid();
            var existingCard = CreateTestCard(cardId);
            var updatedDto = new CardDto(cardId, "Updated Term", "Updated Definition");

            _cardRepoMock.Setup(x => x.GetByIdAsync(cardId)).ReturnsAsync(existingCard);

            _cardRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Card>())).ReturnsAsync((Card c) => c);

            //сам метод
            var result = await _cardService.UpdateCardAsync(cardId, updatedDto);

            Assert.Equal(updatedDto.Term, result.Term);
            Assert.Equal(updatedDto.Definition, result.Definition);
            _cardRepoMock.Verify(x => x.UpdateAsync(It.Is<Card>(c =>
                c.Id == cardId &&
                c.Term == updatedDto.Term &&
                c.Definition == updatedDto.Definition
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateCardAsync_InvalidCardId_ThrowsException()
        {
            var invalidCardId = Guid.NewGuid();
            var cardDto = new CardDto(invalidCardId, "Term", "Definition");

            _cardRepoMock.Setup(x => x.GetByIdAsync(invalidCardId))
                        .ReturnsAsync((Card?)null);

            // сам метод
            await Assert.ThrowsAsync<CardNotFoundException>(() =>
                _cardService.UpdateCardAsync(invalidCardId, cardDto));
        }

    }
}
