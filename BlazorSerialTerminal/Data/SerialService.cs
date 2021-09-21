
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using BlazorSerialTerminal.Business;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO.Ports;
using System.Linq;

namespace BlazorSerialTerminal.Data
{
    public class SerialService : BackgroundService, IService
    {
        public delegate void ChangeEventHandler(int numberArg);
        public event ChangeEventHandler OnValueChanged;

        public int Counter { get; set; }
        SerialPort serialPort = null;

        private ObservableCollection<SerialDataItem> _asciiDataCollection;
        public object CollectionLock;

        public ObservableCollection<SerialDataItem> AsciiDataCollection
        {
            get
            {
                lock (CollectionLock)
                {
                    return _asciiDataCollection;
                }
            }
        }

        private DateTime startTime;
        private Random random = new Random();

        public SerialService(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<SerialService>();
            CreateDataCollection();
            Counter = 0;

            PortName = "COM101";
            serialPort = new SerialPort(PortName);

            serialPort.BaudRate = 19200;
            serialPort.Parity = System.IO.Ports.Parity.None;
            serialPort.StopBits = System.IO.Ports.StopBits.One;
            serialPort.DataBits = 8;
            serialPort.Handshake = System.IO.Ports.Handshake.None;
            serialPort.ReadTimeout = 200;

            serialPort.NewLine = "\r";

            SerialPortError = "";

        }

        public string PortName { get; set; }
        public string SerialPortError { get; set; }

        private void CreateDataCollection()
        {
            CollectionLock = new object();
            _asciiDataCollection = new ObservableCollection<SerialDataItem>();

        }

        public List<string> GetPortsList()
        {
            return SerialPort.GetPortNames().ToList();
        }

        public void OpenSerialPort()
        {
            if (!serialPort.IsOpen)
            {
                try
                {
                    serialPort.PortName = PortName;
                    //serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                    serialPort.Open();
                    SerialPortError = $"{DateTime.Now:HH:mm:ss}: Port Open";
                }
                catch(Exception ex)
                {
                    SerialPortError = $"{DateTime.Now:HH:mm:ss}: ERROR-{ex.Message}";
                }
            }
        }

        public void CloseSerialPort()
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    //serialPort.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
                    serialPort.Close();
                    SerialPortError = $"{DateTime.Now:HH:mm:ss}: Port Closed";
                }
                catch (Exception ex)
                {
                    SerialPortError = $"{DateTime.Now:HH:mm:ss}: ERROR-{ex.Message}";
                }
            }
        }

        public bool SerialPortIsOpen()
        {
            return serialPort.IsOpen;
        }

        public void SendData(string transmitMessage)
        {
            serialPort.WriteLine(transmitMessage);
        }

        //private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        //{
        //    try
        //    {
        //        var serialData = serialPort.ReadLine().ToString();

        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = ex.Message;
        //    }
        //}

        public ILogger Logger { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int sampleTime = 100;
            Logger.LogInformation("SerialService is starting.");

            stoppingToken.Register(() => Logger.LogInformation("SerialService is stopping."));

            startTime = DateTime.Now;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //Logger.LogInformation("ServiceA is doing background work. Iterations:{counter}", Counter);

                    if (serialPort.IsOpen)
                    {
                        var serialData = serialPort.ReadLine();  //blocks until cr received, really need a time out on this

                        var readData = new SerialDataItem(DateTime.Now, serialData);
                        Counter++;

                        lock (CollectionLock)
                        {
                            if (_asciiDataCollection.Count >= 15)
                            {
                                //remove first record
                                _asciiDataCollection.RemoveAt(0);
                            }

                            _asciiDataCollection.Add(readData);
                        }

                        //send ui event
                        OnValueChanged?.Invoke(Counter);
                    }
                    else
                    {
                        serialPort.ReadTimeout = 100;
                        sampleTime = 5;
                    }
                }
                catch (System.TimeoutException timeoutEx)
                {
                    int fred = 2;
                    //a normal serial port read timeout exception, just continue and dont log
                }
                catch (Exception ex)
                {
                    Logger.LogInformation($"SerialService exception: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMilliseconds(sampleTime), stoppingToken);

            }

            Logger.LogInformation("SerialService has stopped.");
        }
    }

}
