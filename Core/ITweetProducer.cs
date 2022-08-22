namespace Core
{
    public interface ITweetProducer
    {
        Task ProduceAsync<T>(string topicName, T message, CancellationToken cancellationToken);
    }
}