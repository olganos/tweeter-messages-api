using Core;
using Core.Commands;

namespace Infrastructure.Handlers
{
    public class TweetCommandHandler : ITweetCommandHandler
    {
        private readonly CommandHandlerConfig _handlerConfig;
        private readonly ITweetProducer _producer;

        public TweetCommandHandler(CommandHandlerConfig handlerConfig, ITweetProducer producer)
        {
            _handlerConfig = handlerConfig;
            _producer = producer;
        }

        public async Task SendCommandAsync(CreateTweetCommand command, CancellationToken cancellationToken)
        {
            await _producer.ProduceAsync(_handlerConfig.CreateTweetTopicName, command, cancellationToken);
        }

        public async Task SendCommandAsync(AddReplyCommand command, CancellationToken cancellationToken)
        {
            await _producer.ProduceAsync(_handlerConfig.AddReplyTopicName, command, cancellationToken);
        }
    }
}
