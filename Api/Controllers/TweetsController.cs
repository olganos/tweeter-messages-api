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
            var t = await _messageRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<List<TweetDto>>(t);
        }

        [HttpGet("{username}")]
        public async Task<List<TweetDto>> All(string username, CancellationToken cancellationToken)
        {
            // todo: check the user
            return _mapper.Map<List<TweetDto>>(await _messageRepository.GetByUsernameAsync(username, cancellationToken));
        }

        [HttpPost("{username}/add")]
        public async Task<ActionResult<TweetDto>> Add(
            [FromBody] TweetEditDto tweet,
            string username,
            CancellationToken cancellationToken)
        {
            // todo: check the user

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tweetDb = new Tweet
            {
                UserName = username,
                Text = tweet.Text,
            };

            await _messageRepository.CreateAsync(tweetDb, cancellationToken);

            return CreatedAtAction(nameof(Add), _mapper.Map<TweetDto>(tweetDb));
        }

        [HttpPut("{username}/update/{id}")]
        public async Task<ActionResult<TweetDto>> Update(
            [FromBody] TweetEditDto tweet,
            string username,
            string id,
            CancellationToken cancellationToken)
        {
            // todo: check he user
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tweetDb = await _messageRepository.GetOneAsync(username, id, cancellationToken);

            if (tweetDb == null)
            {
                return NotFound();
            }

            tweetDb.Text = tweet.Text;

            await _messageRepository.EditAsync(tweetDb, cancellationToken);

            return _mapper.Map<TweetDto>(tweetDb);
        }

        [HttpDelete("{username}/delete/{id}")]
        public async Task<ActionResult> Delete(string username, string id, CancellationToken cancellationToken)
        {
            // todo: check the user
            var tweetDb = await _messageRepository.GetOneAsync(username, id, cancellationToken);

            if (tweetDb == null)
            {
                return NotFound();
            }

            await _messageRepository.DeleteAsync(username, id, cancellationToken);

            return Ok();
        }

        [HttpPost("{username}/reply/{id}")]
        public async Task<ActionResult<TweetDto>> Reply(
            [FromBody] TweetEditDto tweet,
            string username,
            string id,
            CancellationToken cancellationToken)
        {
            // todo: check the user
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tweetDb = await _messageRepository.GetOneAsync(id, cancellationToken);

            if (tweetDb == null)
            {
                return NotFound();
            }

            if (tweetDb.Replies == null)
            {
                tweetDb.Replies = new List<Reply>();
            }

            tweetDb.Replies.Add(new Reply
            {
                Text = tweet.Text,
                UserName = username,
            });

            await _messageRepository.EditAsync(tweetDb, cancellationToken);

            // todo: not sure that its ok to return the whole tweet
            // because it coul be really huge
            return CreatedAtAction(nameof(Reply), _mapper.Map<TweetDto>(tweetDb));
        }
    }
}
