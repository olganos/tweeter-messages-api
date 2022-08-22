using Core.Commands;

namespace Core
{
    public interface ITweetCommandHandler
    {
        Task SendCommandAsync(CreateTweetCommand command, CancellationToken cancellationToken);
        Task SendCommandAsync(AddReplyCommand command, CancellationToken cancellationToken);
    }
}
