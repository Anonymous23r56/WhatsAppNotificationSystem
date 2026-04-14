using System;
using System.Collections.Generic;
using System.Text;

namespace CronJob.Services.Interfaces
{
    public interface ITwilioWhatsAppService
    {
        Task<bool> SendAsync(string toNumber, string message);
    }
}
