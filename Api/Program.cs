using Confluent.Kafka;
using Core;
using Infrastructure.Handlers;
using Infrastructure.Producers;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prometheus;
using Serilog.Sinks.Elasticsearch;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, logConfiguration) =>
    {
        logConfiguration
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Console()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(Environment.GetEnvironmentVariable("ELASTICSEARCH_URI")
                ?? context.Configuration["ElasticConfiguration:Uri"]))
            {
                IndexFormat = $"tweeter-write-api-logs",
                AutoRegisterTemplate = true,
                NumberOfShards = 2,
                NumberOfReplicas = 1
            }).Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName).ReadFrom.Configuration(context.Configuration);
    });

    // Add services to the container.
    builder.Services.AddSingleton<IMessageRepository>(sp => new MessageRepository(
        Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
            ?? builder.Configuration.GetValue<string>("MongoDbSettings:ConnectionString"),
        Environment.GetEnvironmentVariable("DB_NAME")
            ?? builder.Configuration.GetValue<string>("MongoDbSettings:DbName"),
        Environment.GetEnvironmentVariable("DB_TWEET_COLLECTION")
            ?? builder.Configuration.GetValue<string>("MongoDbSettings:DbTweetCollectionName"),
        Environment.GetEnvironmentVariable("DB_REPLY_COLLECTION")
            ?? builder.Configuration.GetValue<string>("MongoDbSettings:DbReplyCollectionName"),
        Environment.GetEnvironmentVariable("DB_LIKE_COLLECTION")
            ?? builder.Configuration.GetValue<string>("MongoDbSettings:DbLikeCollectionName")
    ));

    bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

    var kafkaProducerConfig = isDevelopment
        ? new ProducerConfig
        {
            BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_SERVER")
                ?? builder.Configuration.GetValue<string>("KafkaSettings:BootstrapServers"),
        }
        : new ProducerConfig
        {
            BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_SERVER")
                ?? builder.Configuration.GetValue<string>("KafkaSettings:BootstrapServers"),
            SaslUsername = builder.Configuration.GetValue<string>("KafkaSettings:Key"),
            SaslPassword = builder.Configuration.GetValue<string>("KafkaSettings:Secret"),
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
        };

    builder.Services.AddScoped<ITweetProducer>(sp => new KafkaProduser(kafkaProducerConfig));

    builder.Services.AddSingleton(new CommandHandlerConfig(
        Environment.GetEnvironmentVariable("KAFKA_CREATE_TWEET_TOPIC_NAME")
            ?? builder.Configuration.GetValue<string>("KafkaSettings:CreateTweetTopicName"),
        Environment.GetEnvironmentVariable("KAFKA_ADD_REPLY_TOPIC_NAME")
            ?? builder.Configuration.GetValue<string>("KafkaSettings:AddRealyTopicName")
    ));

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
            options.Authority = Environment.GetEnvironmentVariable("IDENTITY_SERVER_URI")
                ?? builder.Configuration.GetValue<string>("IdentityServer:Uri");

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
            };
        });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("ApiScope", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("scope", "tweeter-api");
        });
    });

    builder.Services.AddSwaggerGen(c =>
    {
        c.EnableAnnotations();
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = @"Enter 'Bearer [space] and your token",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                },
                Scheme="oauth2",
                Name="Bearer",
                In=ParameterLocation.Header
            },
            new List<string>()
        }
        });
    });

    builder.Services.AddScoped<ITweetCommandHandler, TweetCommandHandler>();

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

    app.UseRouting();
    app.UseHttpMetrics();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers()
        .RequireAuthorization("ApiScope");

    app.MapMetrics();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
