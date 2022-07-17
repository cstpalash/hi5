using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace hi5.bot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly IConfiguration _config;

        public WebhookController(ILogger<WebhookController> logger, IConfiguration config)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (config == null) throw new ArgumentNullException("config");

            _logger = logger;
            _config = config;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<string> Get()
        {
            var mode = Request.Query["hub.mode"];
            var token = Request.Query["hub.verify_token"];
            var challenge = Request.Query["hub.challenge"];
            _logger.LogInformation("mode:{0};verify_token:{1};challenge:{2}", mode, token, challenge);

            if (token.Count == 0 || challenge.Count == 0) return BadRequest();

            var validateToken = _config["Token"];

            if (string.CompareOrdinal(token[0], validateToken) != 0) return Forbid();

            return Ok(challenge[0]);
        }
    }
}