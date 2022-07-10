using Markov;

namespace GryazevTheBot.Services
{
    public interface IBotService
    {
        public string HandleMessage(string message);
    }

    public class BotService : IBotService
    {
        private readonly MarkovChain<string> _markovChain;
        private readonly IConfiguration _configuration;

        public BotService(IConfiguration configuration)
        {
            _configuration = configuration;

            if (int.TryParse(_configuration["Config:ChainOrder"], out var chainOrder))
                _markovChain = new MarkovChain<string>(chainOrder);
            else
                _markovChain = new MarkovChain<string>(1);

            using var reader = new StreamReader(new FileStream(@"Resources/data.txt", FileMode.Open));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (!string.IsNullOrWhiteSpace(line))
                    _markovChain.Add(line.Split(' '));
            }
        }

        public string HandleMessage(string message)
        {
            if (message.StartsWith('!'))
            {
                if (message.Equals("!говори голова", StringComparison.InvariantCultureIgnoreCase))
                    return string.Join(" ", _markovChain.Chain());
                else
                {
                    var words = string.Join("", message.Where(c => char.IsLetter(c) || c == 32))
                                      .ToLower()
                                      .Split(' ')
                                      .Where(w => !string.IsNullOrWhiteSpace(w));

                    return string.Join(" ", _markovChain.Chain(words));
                }
            }

            return string.Empty;
        }
    }
}
