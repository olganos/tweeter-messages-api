using DataLayer;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddSingleton<IMongoDatabase>(options =>
//{
//    var client = new MongoClient(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
//    return client.GetDatabase(Environment.GetEnvironmentVariable("DB_NAME"));
//});

//builder.Services.AddSingleton<IMessageRepository, MessageRepository>();

builder.Services.Configure<TweeterMessageSettings>(
    builder.Configuration.GetSection("TweeterMessageDatabase"));

builder.Services.AddSingleton<TweeterMessageService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
