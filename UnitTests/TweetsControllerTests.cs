using Api.Controllers;
using Api.Dto;
using Api.Dto.Requests;
using Api.Dto.Responses;

using AutoMapper;

using AutoMapperProfiles;

using Core;
using Core.Commands;
using Core.Entities;

using Infrastructure.Exceptions;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace UnitTests
{
    [TestFixture]
    internal class TweetsControllerTests
    {
        private TweetsController _tweetsController;
        private Mock<ITweetCommandHandler> _mockedHandler;

        [SetUp]
        public void Setup()
        {
            _mockedHandler = new Mock<ITweetCommandHandler>();

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new TweetProfile());
            }).CreateMapper();

            _tweetsController = new TweetsController(_mockedHandler.Object, mapper);
        }

        #region Create tests

        [Test]
        public async Task Add_CreateCorrectTweet_ReturnNew()
        {
            var username = "username";
            var text = "New message";
            var tag = "tag";

            _mockedHandler.Setup(handler => handler.SendCommandAsync(It.IsAny<CreateTweetCommand>(), It.IsAny<CancellationToken>()));

            var addResult = await _tweetsController.Add(
                new TweetCreateRequest
                {
                    Text = text,
                    Tag = tag,
                },
                username,
                It.IsAny<CancellationToken>());

            Assert.That(addResult, Is.TypeOf(typeof(ActionResult<TweetCreateResponse>)));
            Assert.That(addResult.Result, Is.TypeOf(typeof(CreatedAtActionResult)));

            var actualTweet = (addResult.Result as CreatedAtActionResult).Value as TweetCreateResponse;
            Assert.Multiple(() =>
            {
                Assert.That(actualTweet.Tag, Is.EqualTo(tag));
                Assert.That(actualTweet.Text, Is.EqualTo(text));
                Assert.That(actualTweet.UserName, Is.EqualTo(username));
                Assert.That(actualTweet.Created, Is.EqualTo(DateTimeOffset.Now).Within(30).Seconds);
            });
        }

        [Test]
        public async Task Add_Create_144_CharactersMessage_BadRequest()
        {
            _tweetsController.ModelState.AddModelError("Text", "Not longer than 144 characters");

            var addResult = await _tweetsController.Add(
                It.IsAny<TweetCreateRequest>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.That(addResult, Is.TypeOf(typeof(ActionResult<TweetCreateResponse>)));
            Assert.That(addResult.Result, Is.TypeOf(typeof(BadRequestObjectResult)));
        }

        #endregion

        #region Update tests

        [Test]
        public async Task Update_UpdateCorrectTweet_ReturnUpdated()
        {
            _mockedHandler.Setup(handler => handler.SendCommandAsync(It.IsAny<UpdateTweetCommand>(), It.IsAny<CancellationToken>()));

            var addResult = await _tweetsController.Update(
                new TweetEditRequest(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.That(addResult, Is.TypeOf(typeof(OkResult)));
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

            Assert.That(addResult, Is.TypeOf(typeof(BadRequestObjectResult)));
        }

        [Test]
        public async Task Update_TweetDoesntExist_NotFound()
        {
            _mockedHandler.Setup(handler => handler.SendCommandAsync(It.IsAny<UpdateTweetCommand>(), It.IsAny<CancellationToken>()))
                .Throws(new TweetNotFoundExeption(""));

            var addResult = await _tweetsController.Update(
                new TweetEditRequest(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.That(addResult, Is.TypeOf(typeof(NotFoundResult)));
        }

        #endregion

        #region Delete tests

        [Test]
        public async Task Delete_DeleteCorrectly_ReturnOk()
        {
            _mockedHandler.Setup(handler => handler.SendCommandAsync(It.IsAny<DeleteTweetCommand>(), It.IsAny<CancellationToken>()));

            var addResult = await _tweetsController.Delete(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.That(addResult, Is.TypeOf(typeof(OkResult)));
        }

        [Test]
        public async Task Delete_TweetDoesntExist_NotFound()
        {
            _mockedHandler.Setup(handler => handler.SendCommandAsync(It.IsAny<DeleteTweetCommand>(), It.IsAny<CancellationToken>()))
                 .Throws(new TweetNotFoundExeption(""));

            var addResult = await _tweetsController.Delete(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.That(addResult, Is.TypeOf(typeof(NotFoundResult)));
        }

        #endregion

        #region Delete tests

        [Test]
        public async Task Like_LikeCorrectly_ReturnOk()
        {
            _mockedHandler.Setup(handler => handler.SendCommandAsync(It.IsAny<LikeCommand>(), It.IsAny<CancellationToken>()));

            var addResult = await _tweetsController.Like(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.That(addResult, Is.TypeOf(typeof(OkResult)));
        }

        [Test]
        public async Task Like_TweetDoesntExist_NotFound()
        {
            _mockedHandler.Setup(handler => handler.SendCommandAsync(It.IsAny<LikeCommand>(), It.IsAny<CancellationToken>()))
                 .Throws(new TweetNotFoundExeption(""));

            var addResult = await _tweetsController.Like(
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
            _mockedHandler.Setup(handler => handler.SendCommandAsync(It.IsAny<AddReplyCommand>(), It.IsAny<CancellationToken>()))
                .Throws(new TweetNotFoundExeption(""));

            var addResult = await _tweetsController.Reply(
                new AddReplyRequest(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.That(addResult, Is.TypeOf(typeof(ActionResult<AddReplyResponse>)));
            Assert.That(addResult.Result, Is.TypeOf(typeof(NotFoundResult)));
        }

        [Test]
        public async Task Reply_Add_144_CharactersMessage_BadRequest()
        {
            _tweetsController.ModelState.AddModelError("Text", "Not longer than 144 characters");

            var addResult = await _tweetsController.Reply(
                It.IsAny<AddReplyRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>());

            Assert.That(addResult, Is.TypeOf(typeof(ActionResult<AddReplyResponse>)));
            Assert.That(addResult.Result, Is.TypeOf(typeof(BadRequestObjectResult)));
        }

        [Test]
        public async Task Reply_AddNew_Return201()
        {
            var tweetId = "61a6058e6c43f32854e51f52";
            var username = "username";
            var text = "New message";
            var tag = "tag";

            _mockedHandler.Setup(handler => handler.SendCommandAsync(It.IsAny<AddReplyCommand>(), It.IsAny<CancellationToken>()));

            var addResult = await _tweetsController.Reply(
                new AddReplyRequest()
                {
                    Tag = tag,
                    Text = text
                },
                username,
                tweetId,
                It.IsAny<CancellationToken>());

            Assert.That(addResult, Is.TypeOf(typeof(ActionResult<AddReplyResponse>)));
            Assert.That(addResult.Result, Is.TypeOf(typeof(CreatedAtActionResult)));

            var actualReply = (addResult.Result as CreatedAtActionResult).Value as AddReplyResponse;
            Assert.Multiple(() =>
            {
                Assert.That(actualReply.Tag, Is.EqualTo(tag));
                Assert.That(actualReply.Text, Is.EqualTo(text));
                Assert.That(actualReply.UserName, Is.EqualTo(username));
                Assert.That(actualReply.Created, Is.EqualTo(DateTimeOffset.Now).Within(30).Seconds);
            });
        }

        #endregion
    }
}