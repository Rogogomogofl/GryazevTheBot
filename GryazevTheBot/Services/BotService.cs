namespace GryazevTheBot.Services
{
    public interface IBotService
    {
        public string HandleMessage(string message);
    }

    public class BotService : IBotService
    {
        public string HandleMessage(string message)
        {
            return message;
        }
    }
}
