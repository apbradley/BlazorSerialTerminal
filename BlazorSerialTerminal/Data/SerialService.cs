
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using BlazorSerialTerminal.Business;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlazorSerialTerminal.Data
{
    public class SerialService : BackgroundService, IService
    {
        public delegate void ChangeEventHandler(int numberArg);
        public event ChangeEventHandler OnValueChanged;

        public int Counter { get; set; }

        private ObservableCollection<ChartItem> _dataPointCollection;
        private ObservableCollection<NodeChartData> _devExCollection;
        private object _infragCollectionLock;
        //private object _infragCollectionLock;

        public ObservableCollection<ChartItem> DataPointCollection
        {
            get
            {
                lock (_infragCollectionLock)
                {
                    return _dataPointCollection;
                }
            }
        }
        public ObservableCollection<NodeChartData> DevExCollection
        {
            get
            {
                lock (_infragCollectionLock)
                {
                    return _devExCollection;
                }
            }
        }

        private DateTime startTime;
        private Random random = new Random();

        private bool countDirectionDown = true;
        private const int upperCountLimit = 1000;
        private const int lowerCountLimit = 0;

        public SerialService(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<SerialService>();
            CreateDataCollection();
            Counter = 0;
        }

        private void CreateDataCollection()
        {
            _infragCollectionLock = new object();
            _dataPointCollection = new ObservableCollection<ChartItem>();
            _devExCollection = new ObservableCollection<NodeChartData>();

            //for (int i = 0; i < 10; i++)
            //{
            //    var pointTime = startTime + TimeSpan.FromDays(i);
            //    string label = $"{pointTime:yyMMdd}";
            //    //$"{pointTime.Year}{pointTime.Month:d2}{pointTime.Day:d2}";
            //    DataPointCollection.Add(new ChartItem(label, random.Next(0, 20)));

            //    //new SixDataValueItem(label, random.Next(0, 20), random.Next(10, 30),
            //    //    random.Next(20, 40), random.Next(30, 50), random.Next(40, 60), random.Next(0, 50)));
            //}
        }

        public ILogger Logger { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const int sampleTime = 500;
            int totalCount = 0;
            Logger.LogInformation("ServiceA is starting.");

            stoppingToken.Register(() => Logger.LogInformation("ServiceA is stopping."));

            startTime = DateTime.Now;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //Logger.LogInformation("ServiceA is doing background work. Iterations:{counter}", Counter);

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

                    totalCount++;
                    var pointTime = startTime + TimeSpan.FromMilliseconds(totalCount * sampleTime);
                    string label = $"{pointTime:HH:mm:ss}";

                    lock (_infragCollectionLock)
                    {
                        if (_dataPointCollection.Count >= 60)
                        {
                            //remove first record
                            _dataPointCollection.RemoveAt(0);
                        }

                        var randomValue = random.Next(0, 50);

                        _dataPointCollection.Add(new ChartItem(label, randomValue));

                        if (_devExCollection.Count >= 60)
                        {
                            //remove first record
                            _devExCollection.RemoveAt(0);
                        }

                        var newData = new ChartItem(label, random.Next(0, 50));
                        _devExCollection.Add(new NodeChartData(pointTime, randomValue));
                    }

                    //send ui event
                    OnValueChanged?.Invoke(Counter);
                }
                catch (Exception ex)
                {
                    int fred = 2;
                    Logger.LogInformation($"ServiceA exception: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMilliseconds(sampleTime), stoppingToken);

            }

            Logger.LogInformation("ServiceA has stopped.");
        }
    }
}
