using Api.Controllers;
using Api.Dto;
using AutoMapper;
using AutoMapperProfiles;
using DataLayer;
using Moq;

namespace UnitTests
{
    [TestFixture]
    public class TweetsControllerTests
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
            var cancellationToken = new CancellationToken();

            _mockedRepository.Setup(repo => repo.GetAllAsync(cancellationToken))
               .ReturnsAsync(FullTweetsList);

            var tweetsResult = await _tweetsController.All(cancellationToken);

            Assert.That(tweetsResult.Count, Is.EqualTo(2));
            Assert.That(tweetsResult, Is.InstanceOf(typeof(List<TweetDto>)));
        }


        [Test]
        public async Task GetAll_RequestListByUserName_ReturnList()
        {
            var cancellationToken = new CancellationToken();
            var userName = "userName1";

            _mockedRepository.Setup(repo => repo.GetByUsernameAsync(userName, cancellationToken))
               .ReturnsAsync(FullTweetsList.Where(x => x.UserName == userName).ToList());

            var tweetsResult = await _tweetsController.All(userName, cancellationToken);

            Assert.That(tweetsResult.Count, Is.EqualTo(1));
            Assert.That(tweetsResult, Is.InstanceOf(typeof(List<TweetDto>)));
        }

        [Test]
        public async Task GetAll_RequestListByWrongUserName_ReturNothing()
        {
            var cancellationToken = new CancellationToken();
            var userName = "userName3";

            _mockedRepository.Setup(repo => repo.GetByUsernameAsync(userName, cancellationToken))
               .ReturnsAsync(FullTweetsList.Where(x => x.UserName == userName).ToList());

            var tweetsResult = await _tweetsController.All(userName, cancellationToken);

            Assert.That(tweetsResult.Count, Is.EqualTo(0));
            Assert.That(tweetsResult, Is.InstanceOf(typeof(List<TweetDto>)));
        }

        #endregion

        private List<Tweet> FullTweetsList =>
            new List<Tweet>
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
            new List<TweetDto>
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