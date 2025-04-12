using Kuizlet.Application.Interfaces.Repositories;
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
    public class AccessRequestServiceTests
    {
        private readonly Mock<IAccessRequestRepository> _requestRepoMock = new();
        private readonly Mock<ICardSetRepository> _cardSetRepoMock = new();
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<ICurrentUserInfo> _currentUserMock = new();
        private readonly AccessRequestService _service;

        public AccessRequestServiceTests()
        {
            _service = new AccessRequestService(
                _requestRepoMock.Object,
                _cardSetRepoMock.Object,
                _userRepoMock.Object,
                _currentUserMock.Object);
        }

        [Fact]
        public async Task RequestAccessAsync_WhenUserNotFound_ReturnsError()
        {
            _currentUserMock.Setup(x => x.GetCurrentLogin()).Returns("unknown");
            _userRepoMock.Setup(x => x.GetIdByLoginAsync("unknown")).ReturnsAsync((Guid?)null);

            var result = await _service.RequestAccessAsync(Guid.NewGuid());

            Assert.Equal("error", result.Response);
            Assert.Equal("User not found", result.Message);
        }

        [Fact]
        public async Task RequestAccessAsync_WhenCardSetNotFound_ReturnsError()
        {
            var userId = Guid.NewGuid();
            _currentUserMock.Setup(x => x.GetCurrentLogin()).Returns("testuser");
            _userRepoMock.Setup(x => x.GetIdByLoginAsync("testuser")).ReturnsAsync(userId);
            _cardSetRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((CardSet?)null);

            var result = await _service.RequestAccessAsync(Guid.NewGuid());

            Assert.Equal("error", result.Response);
            Assert.Equal("Card set not found", result.Message);
        }

        [Fact]
        public async Task RequestAccessAsync_WhenAlreadyCreator_ReturnsMessage()
        {
            var userId = Guid.NewGuid();
            var cardSet = new CardSet { CreatorId = userId };

            _currentUserMock.Setup(x => x.GetCurrentLogin()).Returns("creator");
            _userRepoMock.Setup(x => x.GetIdByLoginAsync("creator")).ReturnsAsync(userId);
            _cardSetRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(cardSet);

            var result = await _service.RequestAccessAsync(Guid.NewGuid());

            Assert.Equal("message", result.Response);
            Assert.Equal("You are the creator of this card set", result.Message);
        }

        [Fact]
        public async Task RequestAccessAsync_WhenAlreadyHasAccess_ReturnsMessage()
        {
            var userId = Guid.NewGuid();
            var cardSet = new CardSet
            {
                CreatorId = Guid.NewGuid(),
                ApprovedUserIds = new List<Guid?> { userId }
            };

            _currentUserMock.Setup(x => x.GetCurrentLogin()).Returns("user");
            _userRepoMock.Setup(x => x.GetIdByLoginAsync("user")).ReturnsAsync(userId);
            _cardSetRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(cardSet);

            var result = await _service.RequestAccessAsync(Guid.NewGuid());

            Assert.Equal("message", result.Response);
            Assert.Equal("You already have access to this card set", result.Message);
        }


        [Fact]
        public async Task RequestAccessAsync_NewRequest_CreatesSuccessfully()
        {
            var userId = Guid.NewGuid();
            var cardSet = new CardSet { CreatorId = Guid.NewGuid() };

            _currentUserMock.Setup(x => x.GetCurrentLogin()).Returns("user");
            _userRepoMock.Setup(x => x.GetIdByLoginAsync("user")).ReturnsAsync(userId);
            _cardSetRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(cardSet);
            _requestRepoMock.Setup(x => x.FindByCardSetAndUserAsync(It.IsAny<Guid>(), userId))
                           .ReturnsAsync((AccessRequest?)null);

            var result = await _service.RequestAccessAsync(Guid.NewGuid());

            Assert.Equal("success", result.Response);
            Assert.Equal("Request sent successfully", result.Message);
            _requestRepoMock.Verify(x => x.AddAsync(It.IsAny<AccessRequest>()), Times.Once);
        }

        [Fact]
        public async Task RespondToRequestAsync_WhenRequestNotFound_ThrowsException()
        {
            _requestRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                           .ReturnsAsync((AccessRequest?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.RespondToRequestAsync(Guid.NewGuid(), Guid.NewGuid(), true));
        }

        [Fact]
        public async Task RespondToRequestAsync_WhenCardSetMismatch_ThrowsException()
        {
            var request = new AccessRequest { CardSetId = Guid.NewGuid() };
            _requestRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(request);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.RespondToRequestAsync(Guid.NewGuid(), Guid.NewGuid(), true));
        }

        [Fact]
        public async Task RespondToRequestAsync_WhenNotCreator_ThrowsUnauthorized()
        {
            var request = new AccessRequest { CardSetId = Guid.NewGuid() };
            var cardSet = new CardSet { CreatorId = Guid.NewGuid() };

            _requestRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(request);
            _cardSetRepoMock.Setup(x => x.GetByIdAsync(request.CardSetId)).ReturnsAsync(cardSet);
            _currentUserMock.Setup(x => x.GetCurrentLogin()).Returns("notcreator");
            _userRepoMock.Setup(x => x.GetIdByLoginAsync("notcreator")).ReturnsAsync(Guid.NewGuid());

            await Assert.ThrowsAsync<UnauthorizedException>(
                () => _service.RespondToRequestAsync(request.CardSetId, Guid.NewGuid(), true));
        }

        [Fact]
        public async Task RespondToRequestAsync_ApproveRequest_WorksCorrectly()
        {
            var cardSetId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var creatorId = Guid.NewGuid();

            var request = new AccessRequest
            {
                Id = requestId,
                CardSetId = cardSetId,
                RequesterId = requesterId
            };

            var cardSet = new CardSet { CreatorId = creatorId };

            _requestRepoMock.Setup(x => x.GetByIdAsync(requestId)).ReturnsAsync(request);
            _cardSetRepoMock.Setup(x => x.GetByIdAsync(cardSetId)).ReturnsAsync(cardSet);
            _currentUserMock.Setup(x => x.GetCurrentLogin()).Returns("creator");
            _userRepoMock.Setup(x => x.GetIdByLoginAsync("creator")).ReturnsAsync(creatorId);

            var result = await _service.RespondToRequestAsync(cardSetId, requestId, true);

            Assert.Equal("success", result.Response);
            Assert.Equal("Request has been approved", result.Message);
            _cardSetRepoMock.Verify(x => x.AddApprovedUserAsync(cardSetId, requesterId), Times.Once);
            _requestRepoMock.Verify(x => x.DeleteAsync(requestId), Times.Once);
        }

        [Fact]
        public async Task RespondToRequestAsync_RejectRequest_WorksCorrectly()
        {
            var cardSetId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var creatorId = Guid.NewGuid();

            var request = new AccessRequest
            {
                Id = requestId,
                CardSetId = cardSetId,
            };

            var cardSet = new CardSet { CreatorId = creatorId };

            _requestRepoMock.Setup(x => x.GetByIdAsync(requestId)).ReturnsAsync(request);
            _cardSetRepoMock.Setup(x => x.GetByIdAsync(cardSetId)).ReturnsAsync(cardSet);
            _currentUserMock.Setup(x => x.GetCurrentLogin()).Returns("creator");
            _userRepoMock.Setup(x => x.GetIdByLoginAsync("creator")).ReturnsAsync(creatorId);

            var result = await _service.RespondToRequestAsync(cardSetId, requestId, false);

            Assert.Equal("success", result.Response);
            Assert.Equal("Request has been rejected", result.Message);
            _requestRepoMock.Verify(x => x.UpdateAsync(request), Times.Once);
        }

        [Fact]
        public async Task GetPendingRequestsAsync_WhenNotCreator_ThrowsUnauthorized()
        {
            var cardSetId = Guid.NewGuid();
            var cardSet = new CardSet { CreatorId = Guid.NewGuid() };

            _cardSetRepoMock.Setup(x => x.GetByIdAsync(cardSetId)).ReturnsAsync(cardSet);
            _currentUserMock.Setup(x => x.GetCurrentLogin()).Returns("notcreator");
            _userRepoMock.Setup(x => x.GetIdByLoginAsync("notcreator")).ReturnsAsync(Guid.NewGuid());

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.GetPendingRequestsAsync(cardSetId));
        }

        [Fact]
        public async Task GetPendingRequestsAsync_ReturnsCorrectData()
        {
            var cardSetId = Guid.NewGuid();
            var creatorId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();

            var cardSet = new CardSet
            {
                Id = cardSetId,
                CreatorId = creatorId,
                Name = "Test Set"
            };

            var request = new AccessRequest
            {
                CardSetId = cardSetId,
                RequesterId = requesterId,
            
            };

            var user = new User { Login = "requester" };

            _cardSetRepoMock.Setup(x => x.GetByIdAsync(cardSetId)).ReturnsAsync(cardSet);
            _currentUserMock.Setup(x => x.GetCurrentLogin()).Returns("creator");
            _userRepoMock.Setup(x => x.GetIdByLoginAsync("creator")).ReturnsAsync(creatorId);
            _requestRepoMock.Setup(x => x.GetPendingByCardSetAsync(cardSetId))
                           .ReturnsAsync(new List<AccessRequest> { request });
            _userRepoMock.Setup(x => x.GetByIdAsync(requesterId)).ReturnsAsync(user);

            var result = await _service.GetPendingRequestsAsync(cardSetId);

            Assert.Single(result);
            Assert.Equal("requester", result[0].RequesterLogin);
            Assert.Equal("Test Set", result[0].CardSetName);
           
        }
    }

}
