using Confluent.Kafka;
using Newtonsoft.Json;
using VFX.Application.Interface;
using Microsoft.Extensions.Logging;

namespace VFX.Infrastructure.Messaging;

// KafkaMessageSender class implements IMessageSender and IAsyncDisposable
// This class is responsible for sending messages to a Kafka topic asynchronously.
public class KafkaMessageSender : IMessageSender, IAsyncDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly ILogger<KafkaMessageSender> _logger;

    public KafkaMessageSender(string bootstrapServers, ILogger<KafkaMessageSender> logger)
    {
        var config = new ProducerConfig { BootstrapServers = bootstrapServers };
        _producer = new ProducerBuilder<Null, string>(config).Build();
        _logger = logger;
    }

    // SendMessageAsync method to send messages to a specified Kafka topic
    public async Task SendMessageAsync<T>(string topic, T message)
    {
        var jsonMessage = JsonConvert.SerializeObject(message);
        var msg = new Message<Null, string> { Value = jsonMessage };

        try
        {
            var deliveryResult = await _producer.ProduceAsync(topic, msg);
            _logger.LogInformation($"Message delivered to {deliveryResult.TopicPartitionOffset}");
        }
        catch (ProduceException<Null, string> e)
        {
            _logger.LogError($"Failed to deliver message to topic {topic}: {e.Error.Reason}");
        }
    }

    // DisposeAsync method for cleaning up resources asynchronously
    public async ValueTask DisposeAsync()
    {
        await Task.Run(() => _producer.Flush(TimeSpan.FromSeconds(10)));
        _producer.Dispose();
    }
}
