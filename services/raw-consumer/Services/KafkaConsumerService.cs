using Confluent.Kafka;
using RawConsumer.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace RawConsumer.Services;



public class KafkaConsumerService : BackgroundService
{
    private readonly IConsumer<Ignore, string> _consumer;

    private readonly MongoSettings _mongoSettings;
    private readonly KafkaSettings _kafkaSettings;
    private readonly MongoClient _client;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<KafkaConsumerService> _logger;

    public KafkaConsumerService(
        IOptions<KafkaSettings> kafkaSettings,
        IOptions<MongoSettings> mongoSettings,
        MongoClient client,
        ILogger<KafkaConsumerService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _kafkaSettings = kafkaSettings.Value;
        _mongoSettings = mongoSettings.Value;
        _client = client;
        _serviceProvider = serviceProvider;

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = _kafkaSettings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };
        
        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        
        _consumer.Subscribe(_kafkaSettings.TopicName);
        
        _logger.LogInformation("Kafka Consumer initialized and subscribed to topic.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(TimeSpan.FromSeconds(5));

                if (result == null || string.IsNullOrWhiteSpace(result.Message?.Value))
                {
                    continue;
                }

                string jsonMessage = result.Message.Value;

                using var scope = _serviceProvider.CreateScope();
            
                var validator = scope.ServiceProvider.GetRequiredService<ActivityValidationService>();
                var activityEvent = validator.Validate(jsonMessage);

                if (activityEvent == null)
                {
                    continue;
                }

                var collection = _client.GetDatabase(_mongoSettings.DatabaseName).GetCollection<ActivityEvent>(_mongoSettings.CollectionName);
                await collection.InsertOneAsync(activityEvent);
                _logger.LogInformation(
                    "ActivityEvent {EventId} saved to MongoDB.",
                    activityEvent.EventId);
             
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Kafka message");
            }
        }
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}
