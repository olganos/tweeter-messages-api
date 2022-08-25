using Api.Dto;
using Api.Dto.Requests;
using Api.Dto.Responses;
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
        public async Task<ActionResult<TweetCreateResponse>> Add(
            [FromBody] TweetCreateRequest tweet,
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

            return CreatedAtAction(nameof(Add), _mapper.Map<TweetCreateResponse>(createTweetCommand));
        }

        [HttpPut("{username}/update/{id}")]
        public async Task<ActionResult> Update(
            [FromBody] TweetEditRequest tweet,
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

            return Ok();
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
        public async Task<ActionResult<AddReplyResponse>> Reply(
            [FromBody] AddReplyRequest reply,
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
                Text = reply.Text,
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

            return CreatedAtAction(nameof(Reply), _mapper.Map<AddReplyResponse>(addReplyCommand));
        }
    }
}
