using Api.Dto;
using AutoMapper;
using DataLayer;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/v1.0/tweets")]
    [ApiController]
    public class TweetsController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public TweetsController(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        [HttpGet("all")]
        public async Task<List<TweetDto>> All(CancellationToken cancellationToken)
        {
            return _mapper.Map<List<TweetDto>>(await _messageRepository.GetAllAsync(cancellationToken));
        }

        [HttpGet("{username}")]
        public async Task<List<TweetDto>> All(string username, CancellationToken cancellationToken)
        {
            return _mapper.Map<List<TweetDto>>(await _messageRepository.GetByUsernameAsync(username, cancellationToken));
        }

        [HttpPost("{username}/add")]
        public async Task Add([FromBody] TweetEditDto tweet, string username, CancellationToken cancellationToken)
        {
            await _messageRepository.CreateAsync(new Tweet
            {
                UserName = username,
                Text = tweet.Text,
            },
            cancellationToken);
        }

        [HttpPut("{username}/update/{id}")]
        public async Task Update([FromBody] TweetEditDto tweet, string username, string id, CancellationToken cancellationToken)
        {
            await _messageRepository.EditAsync(
                new Tweet
                {
                    Id = id,
                    UserName = username,
                    Text = tweet.Text,
                },
                cancellationToken);
        }

        [HttpDelete("{username}/delete/{id}")]
        public async Task Delete(string username, string id, CancellationToken cancellationToken)
        {
            await _messageRepository.DeleteAsync(username, id, cancellationToken);
        }
    }
}
