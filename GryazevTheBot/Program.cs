using GryazevTheBot.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model.GroupUpdate;
using VkNet.Utils;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddSingleton<IVkApi>(_ => new VkApi())
    .AddSingleton<IVkService, VkService>()
    .AddSingleton<IBotService, BotService>();

var app = builder.Build();

app.MapPost("/callback", ([FromBody]object update, IVkService vkService) =>
{
    return Results.Ok(vkService.HandleGroupUpdate(GroupUpdate.FromJson(new VkResponse(JToken.FromObject(update)))));
});

app.Run();