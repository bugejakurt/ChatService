using ChatService.Program;
using ChatService.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<ChatServiceOptions>()
    .BindConfiguration("ChatService")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<ChatCoordinatorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/CreateSession", ([FromServices] ChatCoordinatorService chatService) =>
{
    return chatService.CreateSession().ToString();
});
app.MapGet("/Ping", ([FromServices] ChatCoordinatorService chatService, Guid guid) =>
{
    bool pinged = chatService.PingSession(guid);
    return pinged ? Results.Ok() : Results.NotFound();
})
.WithName("ChatService")
.WithOpenApi();

app.Run();