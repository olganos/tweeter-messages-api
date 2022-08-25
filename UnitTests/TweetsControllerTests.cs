using Api.Controllers;
using Api.Dto;
using Api.Dto.Requests;
using AutoMapper;
using AutoMapperProfiles;
using Core;
using Core.Entities;
using DataLayer;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests
{
    [TestFixture]
    internal class TweetsControllerTests
    {
        private TweetsController _tweetsController;
        private Mock<IMessageRepository> _mockedRepository;

        [SetUp]
        public void Setup()
        {
            _mockedRepository = new Mock<IMessageRepository>();

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new TweetProfile());
            }).CreateMapper();

            _tweetsController = new TweetsController(_mockedRepository.Object, mapper);
        }

        #region Test list methonds

        [Test]
        public async Task GetAll_RequestList_ReturnList()
        {
            _mockedRepository.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(FullTweetsList);

            var tweetsResult = await _tweetsController.All(It.IsAny<CancellationToken>());

            Assert.That(tweetsResult, Has.Count.EqualTo(2));
            Assert.That(tweetsResult, Is.TypeOf(typeof(List<TweetDto>)));
        }

        [Test]
        public async Task GetAll_RequestListByUserName_ReturnList()
        {
            var userName = "userName1";

            _mockedRepository.Setup(repo => repo.GetByUsernameAsync(userName, It.IsAny<CancellationToken>()))
               .ReturnsAsync(FullTweetsList.Where(x => x.UserName == userName).ToList());

            var tweetsResult = await _tweetsController.All(userName, It.IsAny<CancellationToken>());

            Assert.That(tweetsResult, Has.Count.EqualTo(1));
            Assert.That(tweetsResult, Is.TypeOf(typeof(List<TweetDto>)));
        }

        [Test]
        public async Task GetAll_RequestListByWrongUserName_ReturNothing()
        {
            var userName = "userName3";

            _mockedRepository.Setup(repo => repo.GetByUsernameAsync(userName, It.IsAny<CancellationToken>()))
               .ReturnsAsync(FullTweetsList.Where(x => x.UserName == userName).ToList());

            var tweetsResult = await _tweetsController.All(userName, It.IsAny<CancellationToken>());

            Assert.That(tweetsResult, Is.Empty);
            Assert.That(tweetsResult, Is.TypeOf(typeof(List<TweetDto>)));
        }

        #endregion

        #region Create tests

        [Test]
        public async Task Add_CreateCorrectTweet_ReturnNew()
        {
            var id = "62f55d7d3925e583c4cc737a";
            var username = "username";
            var text = "New message";

            _mockedRepository.Setup(repo => repo.CreateAsync(It.IsAny<Tweet>(), It.IsAny<CancellationToken>()))
               .Callback<Tweet, CancellationToken>((newTweet, cancellationToken) =>
               {
                   newTweet.Id = id;
               });

            var addResult = await _tweetsController.Add(
                new TweetEditRequest
                {
                    Text = text
                },
                username,
                It.IsAny<CancellationToken>());

            Assert.That(addResult, Is.TypeOf(typeof(ActionResult<TweetDto>)));
            Assert.That(addResult.Result, Is.TypeOf(typeof(CreatedAtActionResult)));

            var actualTweet = (addResult.Result as CreatedAtActionResult).Value as TweetDto;
            Assert.Multiple(() =>
            {
                Assert.That(actualTweet.Id, Is.EqualTo(id));
                Assert.That(actualTweet.Text, Is.EqualTo(text));
                Assert.That(actualTweet.UserName, Is.EqualTo(username));
            });
        }

        [Test]
        public async Task Add_Create_144_CharactersMessage_BadRequest()
        {
            _tweetsController.ModelState.AddModelError("Text", "Not longer than 144 characters");

            var addResult = await _tweetsController.Add(
                It.IsAny<TweetEditRequest>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.That(addResult, Is.TypeOf(typeof(ActionResult<TweetDto>)));
            Assert.That(addResult.Result, Is.TypeOf(typeof(BadRequestObjectResult)));
        }

        #endregion

        #region Update tests

        [Test]
        public async Task Update_UpdateCorrectTweet_ReturnUpdated()
        {
            var id = "62f55d7d3925e583c4cc737a";
            var username = "username";

            _mockedRepository.Setup(repo => repo.GetOneAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new Tweet
               {
                   Id = id,
                   UserName = username,
               });

            _mockedRepository.Setup(repo => repo.EditAsync(It.IsAny<Tweet>(), It.IsAny<CancellationToken>()));

            var text = "New message";

            var addResult = await _tweetsController.Update(
                new TweetEditRequest
                {
                    Text = text
                },
                username,
                id,
                It.IsAny<CancellationToken>());

            Assert.That(addResult, Is.TypeOf(typeof(ActionResult<TweetDto>)));

            var actualTweet = addResult.Value;
            Assert.Multiple(() =>
            {
                Assert.That(actualTweet.Id, Is.EqualTo(id));
                Assert.That(actualTweet.Text, Is.EqualTo(text));
                Assert.That(actualTweet.UserName, Is.EqualTo(username));
            });
        }

        [Test]
        public async Task Update_Create_144_CharactersMessage_BadRequest()
        {
            _tweetsController.ModelState.AddModelError("Text", "Not longer than 144 characters");

            var addResult = await _tweetsController.Update(
                It.IsAny<TweetEditRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.That(addResult, Is.TypeOf(typeof(ActionResult<TweetDto>)));
            Assert.That(addResult.Result, Is.TypeOf(typeof(BadRequestObjectResult)));
        }

        [Test]
        public async Task Update_TweetDoesntExist_NotFound()
        {
            _mockedRepository.Setup(repo => repo.GetOneAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

            var addResult = await _tweetsController.Update(
                It.IsAny<TweetEditRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.That(addResult, Is.TypeOf(typeof(ActionResult<TweetDto>)));
            Assert.That(addResult.Result, Is.TypeOf(typeof(NotFoundResult)));
        }

        #endregion

        #region Delete tests

        [Test]
        public async Task Delete_DeleteCorrectly_ReturnOk()
        {
            _mockedRepository
                .Setup(repo => repo
                    .GetOneAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Tweet());

            _mockedRepository
                .Setup(repo => repo
                    .DeleteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

            var addResult = await _tweetsController.Delete(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.That(addResult, Is.TypeOf(typeof(OkResult)));
        }

        [Test]
        public async Task Delete_TweetDoesntExist_NotFound()
        {
            _mockedRepository.Setup(repo => repo.GetOneAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

            var addResult = await _tweetsController.Delete(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.That(addResult, Is.TypeOf(typeof(NotFoundResult)));
        }

        #endregion

        #region Replies

        [Test]
        public async Task Reply_AddToNonexistingTweet_NotFound()
        {
            _mockedRepository.Setup(repo => repo.GetOneAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

            var addResult = await _tweetsController.Reply(
                It.IsAny<TweetEditRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.That(addResult, Is.TypeOf(typeof(ActionResult<TweetDto>)));
            Assert.That(addResult.Result, Is.TypeOf(typeof(NotFoundResult)));
        }

        [Test]
        public async Task Reply_Add_144_CharactersMessage_BadRequest()
        {
            _tweetsController.ModelState.AddModelError("Text", "Not longer than 144 characters");

            var addResult = await _tweetsController.Reply(
                It.IsAny<TweetEditRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.That(addResult, Is.TypeOf(typeof(ActionResult<TweetDto>)));
            Assert.That(addResult.Result, Is.TypeOf(typeof(BadRequestObjectResult)));
        }

        [Test]
        public async Task Reply_AddNew_Return201()
        {
            _mockedRepository
                .Setup(repo => repo
                    .GetOneAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Tweet());

            _mockedRepository.Setup(repo => repo.EditAsync(It.IsAny<Tweet>(), It.IsAny<CancellationToken>()));

            var addResult = await _tweetsController.Reply(
                new TweetEditRequest(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.That(addResult, Is.TypeOf(typeof(ActionResult<TweetDto>)));
            Assert.That(addResult.Result, Is.TypeOf(typeof(CreatedAtActionResult)));
        }

        #endregion

        private List<Tweet> FullTweetsList =>
            new()
            {
                new Tweet
                {
                    Id = "61a6058e6c43f32854e51f52",
                    Text = "new message",
                    UserName = "userName1"
                },
                new Tweet
                {
                    Id = "62f55d7d3925e583c4cc737a",
                    Text = "another new message",
                    UserName = "userName2"
                },
            };

        private List<TweetDto> FullTweetsDtoList =>
            new()
            {
                new TweetDto
                {
                    Id = "61a6058e6c43f32854e51f52",
                    Text = "new message",
                    UserName = "userName1"
                },
                new TweetDto
                {
                    Id = "62f55d7d3925e583c4cc737a",
                    Text = "another new message",
                    UserName = "userName2"
                },
            };
    }
}