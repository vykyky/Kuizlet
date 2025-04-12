using Kuizlet.Application.Interfaces.Repositories;
using Kuizlet.Application.Models;
using Kuizlet.Application.Services;
using Kuizlet.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuizlet.Tests.LogicTests
{
    public class CardSetServiceTests
    {
        private readonly Mock<ICardSetRepository> _cardSetRepoMock = new();
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<ICurrentUserInfo> _currentUserMock = new();
        private readonly Mock<IAccessRequestRepository> _accessRequestRepoMock = new();
        private readonly CardSetService _service;

        public CardSetServiceTests()
        {
            _service = new CardSetService(
                _cardSetRepoMock.Object,
                _currentUserMock.Object,
                _userRepoMock.Object,
                _accessRequestRepoMock.Object);
        }

        [Fact]
        public async Task GetAllCardSetsAsync_ReturnsCorrectData()
        {
            var userId = Guid.NewGuid();
            var testCardSets = new List<CardSet>{
                new() { Id = Guid.NewGuid(), Name = "Set1", IsPublic = true, CreatorId = Guid.NewGuid() },
                new() { Id = Guid.NewGuid(), Name = "Set2", IsPublic = false, CreatorId = Guid.NewGuid() }
                };

            _currentUserMock.Setup(x => x.GetCurrentLogin()).Returns("testuser");
            _userRepoMock.Setup(x => x.GetIdByLoginAsync("testuser")).ReturnsAsync(userId);
            _cardSetRepoMock.Setup(x => x.FindAllPublicAndAccessibleCardsetsAsync(userId)).ReturnsAsync(testCardSets);
            _userRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new User { Login = "creator" });


            var result = await _service.GetAllCardSetsAsync();


            Assert.Equal(2, result.Count);
            Assert.Equal("Set1", result[0].Name);
            _cardSetRepoMock.Verify(x => x.FindAllPublicAndAccessibleCardsetsAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetCardSetByIdAsync_WhenPublic_ReturnsCardSet()
        {
            var cardSetId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var testCardSet = new CardSet { Id = cardSetId, IsPublic = true, CreatorId = Guid.NewGuid() };

            _currentUserMock.Setup(x => x.GetCurrentLogin()).Returns("testuser");
            _userRepoMock.Setup(x => x.GetIdByLoginAsync("testuser")).ReturnsAsync(userId);
            _cardSetRepoMock.Setup(x => x.FindPublicAndAccessibleCardsetsAsync(cardSetId, userId)).ReturnsAsync(testCardSet);

  
            var result = await _service.GetCardSetByIdAsync(cardSetId);


            Assert.Equal(cardSetId, result.Id);
        }

        [Fact]
        public async Task GetCardSetByIdAsync_WhenRequested_ReturnsRequestedStatus()
        {
            var cardSetId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _currentUserMock.Setup(x => x.GetCurrentLogin()).Returns("testuser");
            _userRepoMock.Setup(x => x.GetIdByLoginAsync("testuser")).ReturnsAsync(userId);
            _cardSetRepoMock.Setup(x => x.FindPublicAndAccessibleCardsetsAsync(cardSetId, userId)).ReturnsAsync((CardSet?)null);
            _accessRequestRepoMock.Setup(x => x.GetRequestsByUserIdAsync(userId))
                .ReturnsAsync(new List<AccessRequest> { new() { CardSetId = cardSetId } });
            _cardSetRepoMock.Setup(x => x.FindByIdAsync(cardSetId)).ReturnsAsync(new CardSet { Id = cardSetId });


            var result = await _service.GetCardSetByIdAsync(cardSetId);

            Assert.Equal("REQUESTED", result.AccessType);
        }

        [Fact]
        public async Task DeleteCardSetAsync_WhenOwner_DeletesCardSet()
        {
            var cardSetId = Guid.NewGuid();
            var testUser = new User { Login = "owner" };
            var testCardSet = new CardSet { Id = cardSetId, CreatorId = testUser.Id };

            _currentUserMock.Setup(x => x.GetCurrentLogin()).Returns("owner");
            _cardSetRepoMock.Setup(x => x.FindByIdAsync(cardSetId)).ReturnsAsync(testCardSet);
            _userRepoMock.Setup(x => x.GetByIdAsync(testCardSet.CreatorId)).ReturnsAsync(testUser);

          
            await _service.DeleteCardSetAsync(cardSetId);

         
            _cardSetRepoMock.Verify(x => x.DeleteAsync(cardSetId), Times.Once);
        }

        [Fact]
        public async Task DeleteCardSetAsync_WhenNotOwner_ThrowsException()
        {
            var cardSetId = Guid.NewGuid();
            var testCardSet = new CardSet { Id = cardSetId, CreatorId = Guid.NewGuid() };

            _currentUserMock.Setup(x => x.GetCurrentLogin()).Returns("notowner");
            _cardSetRepoMock.Setup(x => x.FindByIdAsync(cardSetId)).ReturnsAsync(testCardSet);
            _userRepoMock.Setup(x => x.GetByIdAsync(testCardSet.CreatorId)).ReturnsAsync(new User { Login = "owner" });

          
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.DeleteCardSetAsync(cardSetId));
        }

        [Fact]
        public async Task UpdateCardSetAsync_UpdatesCardSet()
        {
            var cardSetId = Guid.NewGuid();
            var testUser = new User { Login = "owner" };
            var testCardSet = new CardSet { Id = cardSetId, Name = "Old", CreatorId = testUser.Id };
            var updateDto = new CardSetDto(cardSetId, "New", true, testUser.Id, null, "OWNER");

            _currentUserMock.Setup(x => x.GetCurrentLogin()).Returns("owner");
            _cardSetRepoMock.Setup(x => x.FindByIdAsync(cardSetId)).ReturnsAsync(testCardSet);
            _userRepoMock.Setup(x => x.GetByIdAsync(testCardSet.CreatorId)).ReturnsAsync(testUser);

           
            var result = await _service.UpdateCardSetAsync(cardSetId, updateDto);

           
            Assert.Equal("New", result.Name);
            _cardSetRepoMock.Verify(x => x.UpdateAsync(It.Is<CardSet>(cs => cs.Name == "New")), Times.Once);
        }
       
    }
}
