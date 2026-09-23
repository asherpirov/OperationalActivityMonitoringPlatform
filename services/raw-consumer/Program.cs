using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RawConsumer.Models;
using RawConsumer.Services;

var builder = Host.CreateApplicationBuilder(args);

// Kafka Settings
builder.Services.Configure<KafkaSettings>(
    builder.Configuration.GetSection("Kafka"));

// Mongo Settings
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Mongo Client
builder.Services.AddSingleton<MongoClient>(sp =>
{
    var mongoSettings =
        sp.GetRequiredService<IOptions<MongoSettings>>().Value;

    return new MongoClient(mongoSettings.ConnectionString);
});

// Validation Service
builder.Services.AddScoped<ActivityValidationService>();

// Kafka Consumer
builder.Services.AddHostedService<KafkaConsumerService>();

var host = builder.Build();

await host.RunAsync();