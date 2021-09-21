using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlazorSerialTerminal.Data
{
    public class ServiceB : BackgroundService, IService
    {
        public delegate void ChangeEventHandler(int numberArg);
        public event ChangeEventHandler OnValueChanged;
        public int Counter { get; set; }

        private bool countDirectionDown = true;
        private const int upperCountLimit = 1000;
        private const int lowerCountLimit = 0;

        public ServiceB(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<ServiceB>();
            Counter = upperCountLimit;
        }

        public ILogger Logger { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation("ServiceB is starting.");

            stoppingToken.Register(() => Logger.LogInformation("ServiceB is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                //Logger.LogInformation($"ServiceB is doing background work. Iterations:{Counter}");
                if (Counter == lowerCountLimit)
                {
                    countDirectionDown = false;
                }
                else if (Counter == upperCountLimit)
                {
                    countDirectionDown = true;
                }

                if (countDirectionDown == true)
                    Counter--;
                else
                    Counter++;

                OnValueChanged?.Invoke(Counter);
                await Task.Delay(TimeSpan.FromMilliseconds(250), stoppingToken);
            }

            Logger.LogInformation("ServiceB has stopped.");
        }
    }
}
