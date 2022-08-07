using Api.Dto;
using DataLayer;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/v1.0/tweets")]
    [ApiController]
    public class TweetsController : ControllerBase
    {
        private readonly IMessageRepository messageRepository;

        public TweetsController(IMessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        [HttpGet("all")]
        public async Task<List<Tweet>> All(CancellationToken cancellationToken)
        {
            return await messageRepository.GetAllAsync(cancellationToken);
        }

        [HttpGet("{username}")]
        public async Task<List<Tweet>> All(string username, CancellationToken cancellationToken)
        {
            return await messageRepository.GetByUsernameAsync(username, cancellationToken);
        }

        [HttpPost("{username}/add")]
        public async Task Add([FromBody] Tweet tweet, string username, CancellationToken cancellationToken)
        {
            await messageRepository.CreateAsync(tweet, cancellationToken);
        }

        [HttpPut("{username}/update/{id}")]
        public async Task Update([FromBody] Tweet tweet, string username, string id, CancellationToken cancellationToken)
        {
            await messageRepository.EditAsync(tweet, cancellationToken);
        }

        [HttpDelete("{username}/delete/{id}")]
        public async Task Delete(string username, string id, CancellationToken cancellationToken)
        {
            await messageRepository.DeleteAsync(id, cancellationToken);
        }
    }
}
