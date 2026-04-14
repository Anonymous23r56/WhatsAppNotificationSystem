using System;
using System.Collections.Generic;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using CronJob.Services.Interfaces;

namespace CronJob.Services
{
    public class TwilioWhatsAppService : ITwilioWhatsAppService
    {
        private readonly string _fromNumber;
        private readonly ILogger<TwilioWhatsAppService> _logger;

        public TwilioWhatsAppService(IConfiguration config,
            ILogger<TwilioWhatsAppService> logger)
        {
            _logger = logger;
            var accountSid = config["Twilio:AccountSid"]!;
            var authToken = config["Twilio:AuthToken"]!;
            _fromNumber = config["Twilio:WhatsAppFrom"]!;
            TwilioClient.Init(accountSid, authToken);
        }

        public async Task<bool> SendAsync(string toNumber, string message)
        {
            try
            {
                var result = await MessageResource.CreateAsync(
                    from: new PhoneNumber(_fromNumber),
                    to: new PhoneNumber(toNumber),
                    body: message
                );
                _logger.LogInformation(
                    "WhatsApp message sent. SID: {Sid}", result.Sid);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Failed to send WhatsApp to {To}. Error: {Error}",
                    toNumber, ex.Message);
                return false;
            }
        }
    }
}
