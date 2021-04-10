using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SevColApp.Helpers
{
    public class TimeHelper
    {
        public async Task WaitUntilNextThreeHours(ILogger logger, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Stock exchange service completed");

            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken); //wait a minute to be sure you won't end up waiting no time

            await WaitForHourDevisibleByThree(logger, cancellationToken);
        }

        public async Task WaitForHourDevisibleByThree(ILogger logger, CancellationToken cancellationToken)
        {
            var now = DateTime.Now;

            var hoursToWait = 3 - now.Hour % 3;

            var minutesToWait = hoursToWait * 60 - now.Minute;

            logger.LogInformation($"Stock exchange service waiting for {minutesToWait} minutes.");

            await Task.Delay(TimeSpan.FromMinutes(minutesToWait), cancellationToken);
        }
    }
}
