using Confluent.Kafka;
using Core;
using DataLayer;
using Infrastructure.Handlers;
using Infrastructure.Producers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IMessageRepository>(sp => new MessageRepository(
    Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
        ?? builder.Configuration.GetValue<string>("MongoDbSettings:ConnectionString"),
    Environment.GetEnvironmentVariable("DB_NAME")
        ?? builder.Configuration.GetValue<string>("MongoDbSettings:DbName"),
    Environment.GetEnvironmentVariable("DB_TWEET_COLLECTION")
        ?? builder.Configuration.GetValue<string>("MongoDbSettings:DbTweetCollectionName")
));

builder.Services.AddScoped<ITweetProducer>(sp => new KafkaProduser(new ProducerConfig
{
    BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_SERVER")
        ?? builder.Configuration.GetValue<string>("KafkaSettings:BootstrapServers"),
}));

builder.Services.AddSingleton(new CommandHandlerConfig(
    Environment.GetEnvironmentVariable("KAFKA_CREATE_TWEET_TOPIC_NAME")
        ?? builder.Configuration.GetValue<string>("KafkaSettings:CreateTweetTopicName"),
    Environment.GetEnvironmentVariable("KAFKA_ADD_REPLY_TOPIC_NAME")
        ?? builder.Configuration.GetValue<string>("KafkaSettings:AddRealyTopicName")
));

builder.Services.AddScoped<ITweetCommandHandler, TweetCommandHandler>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
