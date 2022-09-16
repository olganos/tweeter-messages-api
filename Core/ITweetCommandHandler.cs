using Core.Commands;

namespace Core
{
    public interface ITweetCommandHandler
    {
        Task SendCommandAsync(CreateTweetCommand command, CancellationToken cancellationToken);
        Task SendCommandAsync(AddReplyCommand command, CancellationToken cancellationToken);
        Task SendCommandAsync(UpdateTweetCommand command, CancellationToken cancellationToken);
        Task SendCommandAsync(DeleteTweetCommand command, CancellationToken cancellationToken);
        Task SendCommandAsync(LikeCommand command, CancellationToken cancellationToken);
    }
}
