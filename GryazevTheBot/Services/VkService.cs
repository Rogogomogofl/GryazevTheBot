using VkNet.Abstractions;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.GroupUpdate;

namespace GryazevTheBot.Services
{
    public interface IVkService
    {
        public Task<string> HandleGroupUpdateAsync(GroupUpdate update);
    }

    public class VkService : IVkService
    {
        private readonly IVkApi _vkApi;
        private readonly IConfiguration _configuration;
        private readonly IBotService _botService;

        private readonly Random _random = new Random();

        const int MaxMessageLength = 4096;

        public VkService(
            IVkApi vkApi,
            IConfiguration configuration,
            IBotService botService)
        {
            _vkApi = vkApi;
            _configuration = configuration;
            _botService = botService;

            if (!vkApi.IsAuthorized)
                Auth();
        }

        private void Auth()
        {
            _vkApi.Authorize(new ()
            {
                AccessToken = _configuration["Config:AccessToken"]
            });
        }

        private async Task SendMessageAsync(string message, long? peerId)
        {
            if (string.IsNullOrEmpty(message))
                return;

            if (message.Length > MaxMessageLength)
                message = message.Substring(0, MaxMessageLength);

            await _vkApi.Messages.SendAsync(new ()
            {
                Message = message,
                PeerId = peerId,
                RandomId = _random.NextInt64()
            });
        }

        public async Task<string> HandleGroupUpdateAsync(GroupUpdate update)
        {
            if (update.Type == GroupUpdateType.Confirmation)
                return await _vkApi.Groups.GetCallbackConfirmationCodeAsync((ulong)update.GroupId);

            if (update.Type == GroupUpdateType.MessageNew)
            {
                var message = update.MessageNew.Message;
                var responce = _botService.HandleMessage(message.Text);

                await SendMessageAsync(responce, message.PeerId);
            }

            return "ok";
        }
    }
}
