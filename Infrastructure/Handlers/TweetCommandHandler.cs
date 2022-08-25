using Core;
using Core.Commands;
using Infrastructure.Exceptions;

namespace Infrastructure.Handlers
{
    public class TweetCommandHandler : ITweetCommandHandler
    {
        private readonly CommandHandlerConfig _handlerConfig;
        private readonly IMessageRepository _messageRepository;
        private readonly ITweetProducer _producer;

        public TweetCommandHandler(
            CommandHandlerConfig handlerConfig,
            IMessageRepository messageRepository,
            ITweetProducer producer)
        {
            _handlerConfig = handlerConfig;
            _messageRepository = messageRepository;
            _producer = producer;
        }

        public async Task SendCommandAsync(CreateTweetCommand command, CancellationToken cancellationToken)
        {
            await _producer.ProduceAsync(_handlerConfig.CreateTweetTopicName, command, cancellationToken);
        }

        public async Task SendCommandAsync(AddReplyCommand command, CancellationToken cancellationToken)
        {
            var tweetDb = await _messageRepository.TweetExistsAsync(command.TweetId, cancellationToken);

            if (!tweetDb)
            {
                throw new TweetNotFoundExeption("Tweet not found");
            }

            await _producer.ProduceAsync(_handlerConfig.AddReplyTopicName, command, cancellationToken);
        }

        public async Task SendCommandAsync(UpdateTweetCommand command, CancellationToken cancellationToken)
        {
            var tweetDb = await _messageRepository.GetOneAsync(command.UserName, command.TweetId, cancellationToken);

            if (tweetDb == null)
            {
                throw new TweetNotFoundExeption("Tweet not found");
            }

            tweetDb.Text = command.Text;

            await _messageRepository.EditAsync(tweetDb, cancellationToken);
        }

        public async Task SendCommandAsync(DeleteTweetCommand command, CancellationToken cancellationToken)
        {
            var tweetDb = await _messageRepository.TweetExistsAsync(command.UserName, command.TweetId, cancellationToken);

            if (!tweetDb)
            {
                throw new TweetNotFoundExeption("Tweet not found");
            }

            await _messageRepository.DeleteAsync(command.UserName, command.TweetId, cancellationToken);
        }
    }
}
