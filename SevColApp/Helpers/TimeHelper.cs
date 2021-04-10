using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SevColApp.Helpers
{
    public class TimeHelper
    {
        public async Task WaitForHourDevisibleByThree(ILogger logger, CancellationToken cancellationToken)
        {
            var now = DateTime.Now;

            var hoursToWait = 3 - now.Hour % 3;

            var minutesToWait = hoursToWait * 60 - now.Minute;

            var secondsToWait = minutesToWait * 60 - now.Second;

            logger.LogInformation($"Stock exchange service waiting for {hoursToWait - 1} hours, {minutesToWait - 1 - (hoursToWait -1) * 60} minutes, and {secondsToWait - (minutesToWait - 1) * 60} seconds.");

            await Task.Delay(TimeSpan.FromSeconds(secondsToWait), cancellationToken);
        }
    }
}
