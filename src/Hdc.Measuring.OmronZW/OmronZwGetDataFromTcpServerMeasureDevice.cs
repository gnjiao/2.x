using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using Hdc.Diagnostics;
using Hdc.Reactive.Linq;
using Hdc.Sockets;

namespace Hdc.Measuring.OmronZW
{
    [Serializable]
    [ContentProperty(nameof(EventController))]
    public class OmronZwGetDataFromTcpServerMeasureDevice : IMeasureDevice
    {
        private int _tagCounter;
        private static bool _isInitialized;
        public IEventController EventController { get; set; }
        private AsyncTcpClient _client;
        private Queue<string> _receivedStrings;
        private Queue<double> _receivedValues;
        private Queue<MeasureEvent> _measureEvents;
        public bool IsInitialized { get; }
        public int PointCount { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

        public void Initialize()
        {
            if (_isInitialized)
                return;

            _receivedStrings = new Queue<string>(PointCount);
            _receivedValues = new Queue<double>(PointCount);
            _measureEvents = new Queue<MeasureEvent>(PointCount);

            var sw = new NotifyStopwatch($"{nameof(OmronZwGetDataFromTcpServerMeasureDevice)}.Initialize()");


            var autoResetEvent = new AutoResetEvent(false);

            Task.Run(async () =>
            {
            _client = new AsyncTcpClient();
            _client.OnDataReceived += Client_OnDataReceived;
            _client.OnDisconnected += Client_OnDisconnected;

                try
                {
                    var sw2 = new NotifyStopwatch("_client.ConnectAsync");
                    await _client.ConnectAsync(Host, Port);
                    sw2.Dispose();
                    autoResetEvent.Set();
                }
                catch (Exception e)
                {
                    Console.WriteLine("_client.ConnectAsync Exception!!!, at " + DateTime.Now);
                    throw;
                }

                Console.WriteLine("_client.Receivee begin, at " + DateTime.Now);
                await _client.Receive();
                Console.WriteLine("_client.Receivee OK, at " + DateTime.Now);
            });

            Console.WriteLine("Main thread wait for ConnectAsync() begin, at " + DateTime.Now);
            autoResetEvent.WaitOne();
            Console.WriteLine("Main thread wait for ConnectAsync() OK, at " + DateTime.Now);

            EventController.Initialize();
            EventController.Event.ObserveOnTaskPool()
                .Subscribe(measureEvent => { _measureEvents.Enqueue(measureEvent); });

            _isInitialized = true;
            sw.Dispose();
        }


        private async void Client_OnDisconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Client_OnDisconnected");
            throw new InvalidOperationException("Client_OnDisconnected, at " + DateTime.Now);
        }

        private void Client_OnDataReceived(object sender, byte[] e)
        {
            Char[] c = Encoding.ASCII.GetChars(e);
            var s = new string(c);

            Console.WriteLine("Client_OnDataReceived: Str:" + s);

            _receivedStrings.Enqueue(s);

            var value = GetValueFromTcpString(s);
            _receivedValues.Enqueue(value);

            Console.WriteLine("Client_OnDataReceived: Value:" + value);
        }

        public MeasureResult Measure(MeasureEvent measureEvent)
        {
            //            var zValues = GetZValues(PointCount);
            //            var xValues = _measureEvents.Select(x => x.X).ToArray();
            //            var yValues = _measureEvents.Select(x => x.Y).ToArray();

            var receivedValues2 = _receivedValues.ToList();
            var receivedStrings2 = _receivedStrings.ToList();
            var measureEvents2 = _measureEvents.ToList();

            if (_receivedStrings.Count < PointCount)
            {
                throw new InvalidOperationException("ERROR: _receivedStrings.Count < nCount");
            }

            var measureResult = new MeasureResult()
            {
                Tag = _tagCounter,
                MeasureDateTime = DateTime.Now,
                MeasureEvent = measureEvent,
            };

            for (int i = 0; i < PointCount; i++)
            {
                var me = measureEvents2[i];
                var value = receivedValues2[i];

                Console.WriteLine(
                    $"Out{i:D2}={value},Robot.P:{me.PointIndex},X:{me.X},Y:{me.Y}, TcpStr: {receivedStrings2}");

                measureResult.Outputs.Add(new MeasureOutput()
                {
                    Index = i,
                    Validity = MeasureValidity.Valid,
                    Judge = MeasureOutputJudge.Go,
                    Value = value,
                    MeasureEvent = me,
                });
            }

            _receivedValues.Clear();
            _receivedStrings.Clear();
            _measureEvents.Clear();
            return measureResult;
        }

        public static double GetValueFromTcpString(string s)
        {
            var sps = s.Split('.');
            var first = double.Parse(sps[0]);
            var second = double.Parse(sps[1]);
            var full = first + "." + second;
            var value = double.Parse(full);
            return value;
        }
    }
}