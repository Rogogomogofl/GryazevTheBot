using GryazevTheBot.Models;
using GryazevTheBot.Services;
using Microsoft.AspNetCore.Mvc;
using VkNet;
using VkNet.Abstractions;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddSingleton<IVkApi>(_ => new VkApi())
    .AddSingleton<IVkService, VkService>()
    .AddSingleton<IBotService, BotService>();

builder.Configuration["Config:Confirmation"] = Environment.GetEnvironmentVariable("Confirmation");
builder.Configuration["Config:AccessToken"] = Environment.GetEnvironmentVariable("AccessToken");

var app = builder.Build();

app.MapPost("/callback", ([FromBody] Update update, IVkService vkService) =>
{
    var response = vkService.HandleGroupUpdate(update);
    return response;
});

app.Run();