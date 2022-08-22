using Confluent.Kafka;
using Core;
using System.Text.Json;

namespace Infrastructure.Producers
{
    public class KafkaProduser : ITweetProducer
    {
        private readonly ProducerConfig _config;

        public KafkaProduser(ProducerConfig producerConfig)
        {
            _config = producerConfig;
        }

        public async Task ProduceAsync<T>(string topicName, T message, CancellationToken cancellationToken)
        {
            using var producer = new ProducerBuilder<string, string>(_config)
               .SetKeySerializer(Serializers.Utf8)
               .SetValueSerializer(Serializers.Utf8)
               .Build();

            var eventMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(message, typeof(T))
            };

            var deliveryResult = await producer.ProduceAsync(topicName, eventMessage, cancellationToken);

            if (deliveryResult.Status == PersistenceStatus.NotPersisted)
            {
                throw new Exception($"Could not produce {typeof(T)} message to topic - {topicName} due to the following reason: {deliveryResult.Message}.");
            }
        }
    }
}