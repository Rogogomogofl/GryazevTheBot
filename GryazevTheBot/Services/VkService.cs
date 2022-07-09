using VkNet.Abstractions;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.GroupUpdate;
using VkNet.Model.RequestParams;

namespace GryazevTheBot.Services
{
    public interface IVkService
    {
        public string HandleGroupUpdate(GroupUpdate groupUpdate);
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
            _vkApi.Authorize(new ApiAuthParams
            {
                AccessToken = _configuration["Config:AccessToken"]
            });
        }

        private void SendMessage(string message, long? peerId)
        {
            if (string.IsNullOrEmpty(message))
                return;

            if (message.Length > MaxMessageLength)
                message = message.Substring(0, MaxMessageLength);

            _vkApi.Messages.Send(new MessagesSendParams
            {
                Message = message,
                PeerId = peerId,
                RandomId = _random.NextInt64()
            });
        }

        public string HandleGroupUpdate(GroupUpdate groupUpdate)
        {
            if (groupUpdate.Type == GroupUpdateType.Confirmation)
                return _configuration["Config:Confirmation"];

            if (groupUpdate.Type == GroupUpdateType.MessageNew)
            {
                var message = groupUpdate.MessageNew.Message;
                var responce = _botService.HandleMessage(message.Text);

                SendMessage(responce, message.PeerId);
            }

            return string.Empty;
        }
    }
}
