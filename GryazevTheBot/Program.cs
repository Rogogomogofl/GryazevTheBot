using GryazevTheBot.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model.GroupUpdate;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddSingleton<IVkApi>(_ => new VkApi())
    .AddSingleton<IVkService, VkService>()
    .AddSingleton<IBotService, BotService>();

builder.Configuration["Config:Confirmation"] = Environment.GetEnvironmentVariable("Confirmation");
builder.Configuration["Config:AccessToken"] = Environment.GetEnvironmentVariable("AccessToken");
builder.Configuration["Config:ChainOrder"] = Environment.GetEnvironmentVariable("ChainOrder");

var app = builder.Build();

app.MapPost("/callback", async ([FromBody] object body, IVkService vkService) =>
{
    var update = GroupUpdate.FromJson(new VkNet.Utils.VkResponse(JToken.Parse(body?.ToString() ?? "")));
    return await vkService.HandleGroupUpdateAsync(update);
});

app.Run();