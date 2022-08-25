using Api.Dto;
using AutoMapper;
using Core;
using Core.Commands;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/v1.0/tweets")]
    [ApiController]
    public class TweetsController : ControllerBase
    {
        private readonly ITweetCommandHandler _handler;
        private readonly IMapper _mapper;

        public TweetsController(
            ITweetCommandHandler handler,
            IMapper mapper)
        {
            _handler = handler;
            _mapper = mapper;
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

            var createTweetCommand = new CreateTweetCommand
            {
                UserName = username,
                Text = tweet.Text,
                Created = DateTimeOffset.Now
            };

            await _handler.SendCommandAsync(createTweetCommand, cancellationToken);

            return CreatedAtAction(nameof(Add), _mapper.Map<TweetDto>(createTweetCommand));
        }

        [HttpPut("{username}/update/{id}")]
        public async Task<ActionResult<TweetDto>> Update(
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

            var updateTweetCommand = new UpdateTweetCommand
            {
                UserName = username,
                Text = tweet.Text,
                TweetId = id
            };

            try
            {
                await _handler.SendCommandAsync(updateTweetCommand, cancellationToken);
            }
            catch (TweetNotFoundExeption)
            {
                return NotFound();
            }

            return _mapper.Map<TweetDto>(updateTweetCommand);
        }

        [HttpDelete("{username}/delete/{id}")]
        public async Task<ActionResult> Delete(string username, string id, CancellationToken cancellationToken)
        {
            // todo: check the user
            var deleteTweetCommand = new DeleteTweetCommand
            {
                UserName = username,
                TweetId = id
            };

            try
            {
                await _handler.SendCommandAsync(deleteTweetCommand, cancellationToken);
            }
            catch (TweetNotFoundExeption)
            {
                return NotFound();
            }

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

            var addReplyCommand = new AddReplyCommand
            {
                UserName = username,
                Text = tweet.Text,
                TweetId = id,
                Created = DateTimeOffset.Now
            };

            try
            {
                await _handler.SendCommandAsync(addReplyCommand, cancellationToken);
            }
            catch (TweetNotFoundExeption)
            {
                return NotFound();
            }

            // todo: not sure that its ok to return the whole tweet
            // because it coul be really huge
            return CreatedAtAction(nameof(Reply), _mapper.Map<TweetDto>(addReplyCommand));
        }
    }
}
