using Confluent.Kafka;
using Core;
using Infrastructure.Handlers;
using Infrastructure.Producers;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .WriteTo.Http(
        Environment.GetEnvironmentVariable("LOGSTASH_URI")
            ?? throw new ArgumentNullException("Logstash URI is needed"),
        null)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddSingleton<IMessageRepository>(sp => new MessageRepository(
        Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
            ?? builder.Configuration.GetValue<string>("MongoDbSettings:ConnectionString"),
        Environment.GetEnvironmentVariable("DB_NAME")
            ?? builder.Configuration.GetValue<string>("MongoDbSettings:DbName"),
        Environment.GetEnvironmentVariable("DB_TWEET_COLLECTION")
            ?? builder.Configuration.GetValue<string>("MongoDbSettings:DbTweetCollectionName"),
        Environment.GetEnvironmentVariable("DB_REPLY_COLLECTION")
            ?? builder.Configuration.GetValue<string>("MongoDbSettings:DbReplyCollectionName")
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

    app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            context.Response.ContentType = Application.Json;

            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

            Log.Error(exceptionHandlerPathFeature?.Error, "Error happened");

            if (exceptionHandlerPathFeature == null)
            {
                return;
            }

            await context.Response.WriteAsync(JsonSerializer.Serialize(
                new
                {
                    Message = app.Environment.IsDevelopment()
                        ? JsonSerializer.Serialize(exceptionHandlerPathFeature.Error)
                        : "Something went wrong"
                })
            );
        });
    });

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
