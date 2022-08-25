using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Adapters.Twilio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace AppointmentBot.Controllers
{
    [Route("api/twilio")]
    [ApiController]
    public class TwilioController : ControllerBase
    {
        private readonly TwilioAdapter _adapter;
        private readonly IBot _bot;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotController"/> class.
        /// </summary>
        /// <param name="adapter">adapter for the BotController.</param>
        /// <param name="bot">bot for the BotController.</param>
        public TwilioController(TwilioAdapter adapter, IBot bot)
        {
            _adapter = adapter;
            _bot = bot;
        }

        /// <summary>
        /// PostAsync method that returns an async Task.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [HttpGet]
        public async Task PostAsync()
        {
            // Delegate the processing of the HTTP POST to the adapter.
            // The adapter will invoke the bot.
            String savedLogData = printDetailedLog(Request);
            try
            {
                await _adapter.ProcessAsync(Request, Response, _bot, default(CancellationToken));
            }
            catch (Exception e)
            {
                Response.StatusCode = StatusCodes.Status403Forbidden;
                savedLogData += "-------El error recibido es: " + e.Message;
                await Response.WriteAsync(savedLogData);
            }
        }

        private string printDetailedLog(HttpRequest request)
        {
            string requestUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
            string cookies = string.Join("-", request.Cookies.Keys.Select(key => key + ":" + request.Cookies[key]).ToArray());
            string headers = string.Join("-", request.Headers.Keys.Select(key => key + ":" + request.Headers[key]).ToArray());
            string parameters = string.Join("-", request.Form.Keys.Select(key => key + ":" + request.Form[key]).ToArray());

            string signature = request.Headers["X-Twilio-Signature"];
            return String.Format("Los datos del response recibidos son: requestUrl={0}, ContentType={3}, parameters={1}, Cookies={4}, headers={5}, signature={2}", requestUrl, parameters, signature, request.ContentType, cookies, headers);
        }
    }
}
