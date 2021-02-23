using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PubSubSample
{
    public class Home : Controller
    {
        private readonly Channel<string> _channel;
        public Home(Channel<string> channel)
        {
            _channel = channel;
        }
        public async Task<bool> SendC()
        {
            await _channel.Writer.WriteAsync("Hello");
            return true;
        }
    }
}