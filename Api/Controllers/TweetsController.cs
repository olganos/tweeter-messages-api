using Api.Dto;
using DataLayer;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    //[Route("api/v1.0/tweets")]
    //[ApiController]
    //public class TweetsController : ControllerBase
    //{

    //    private List<TweetDto> _tweets = new List<TweetDto> {
    //            new TweetDto
    //            {
    //                Id = Guid.NewGuid().ToString(),
    //                Text = "Text",
    //            },
    //            new TweetDto
    //            {
    //                Id = Guid.NewGuid().ToString(),
    //                Text = "Text 2",
    //            }};

    //    private readonly IMessageRepository messageRepository;

    //    public TweetsController(IMessageRepository messageRepository)
    //    {
    //        this.messageRepository = messageRepository;
    //    }

    //    [HttpGet("all")]
    //    public async Task<List<Tweet>> All()
    //    {
    //        return await messageRepository.GetAllAsync();
    //    }

    //    [HttpGet("{username}")]
    //    public List<TweetDto> All(string username)
    //    {
    //        return _tweets;
    //    }

    //    [HttpPost("{username}/add")]
    //    public async Task Add(string username, [FromBody] Tweet tweet)
    //    {
    //        await messageRepository.CreateAsync(tweet);
    //    }

    //    [HttpPut("{username}/update/{id}")]
    //    public void Update(string username, Guid id, [FromBody] Tweet tweet)
    //    {
    //        //return _tweets;
    //    }

    //    [HttpDelete("{username}/delete/{id}")]
    //    public void Delete(string username, Guid id)
    //    {
    //        //return _tweets;
    //    }
    //}

    [Route("api/v1.0/tweets")]
    [ApiController]
    public class TweetsController : ControllerBase
    {

        private List<TweetDto> _tweets = new List<TweetDto> {
                new TweetDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = "Text",
                },
                new TweetDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = "Text 2",
                }};

        private readonly TweeterMessageService _tweeterMessageService;

        public TweetsController(TweeterMessageService tweeterMessageService)
        {
            this._tweeterMessageService = tweeterMessageService;
        }

        [HttpGet("all")]
        public async Task<List<Tweet>> All()
        {
            return await _tweeterMessageService.GetAsync();
        }

        [HttpGet("{username}")]
        public List<TweetDto> All(string username)
        {
            return _tweets;
        }

        [HttpPost("{username}/add")]
        public async Task Add(string username, [FromBody] Tweet tweet)
        {
            await _tweeterMessageService.CreateAsync(tweet);
        }

        [HttpPut("{username}/update/{id}")]
        public void Update(string username, Guid id, [FromBody] Tweet tweet)
        {
            //return _tweets;
        }

        [HttpDelete("{username}/delete/{id}")]
        public void Delete(string username, Guid id)
        {
            //return _tweets;
        }
    }
}
