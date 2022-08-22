using Confluent.Kafka;
using Core;
using System.Net;
using System.Text.Json;

namespace Infrastructure
{
    public class KafkaProduser : IProducer
    {
        private readonly ProducerConfig _config;
        private readonly string _kafkaServer = Environment.GetEnvironmentVariable("KAFKA_SERVER") ?? "localhost:9092";

        public KafkaProduser()
        {


            _config = new ProducerConfig
            {
                BootstrapServers = _kafkaServer,
                ClientId = Dns.GetHostName(),
            };
        }

        public async Task ProduceAsync<T>(string topic, T message)
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

            var deliveryResult = await producer.ProduceAsync(topic, eventMessage);

            if (deliveryResult.Status == PersistenceStatus.NotPersisted)
            {
                throw new Exception($"Could not produce {typeof(T)} message to topic - {topic} due to the following reason: {deliveryResult.Message}.");
            }
        }
    }
}