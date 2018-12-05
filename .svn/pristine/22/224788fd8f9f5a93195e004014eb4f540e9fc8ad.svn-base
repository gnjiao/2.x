using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using EasyNetQ;
using Hdc.Toolkit;

namespace Hdc.Measuring
{
    [Serializable]
    public class MqInitializer: IInitializer
    {
        private Thread _mqThread;
        private IBus _bus;
        private Dispatcher _mqDispatcher;
        private readonly AutoResetEvent InitEasyNetQEvent = new AutoResetEvent(false);

        public string EasyNetQ_Host { get; set; }
        public string EasyNetQ_VirtualHost { get; set; } = "hdc";
        public string EasyNetQ_Username { get; set; }
        public string EasyNetQ_Password { get; set; }
        private static bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized)
                throw new InvalidOperationException();

            this.WriteLineDebugAndConsole($"{nameof(MqInitializer)}.{nameof(MqInitializer.Initialize)} begin, at {DateTime.Now}");

            _mqThread = new Thread((() =>
            {
                _bus = EasyNetQEx.CreateBusAndWaitForConnnected(
                   EasyNetQ_Host,
                   EasyNetQ_VirtualHost,
                   EasyNetQ_Username,
                   EasyNetQ_Password,
                   5000,
                   isConnected => this.WriteLineDebugAndConsole($"{nameof(MqInitializer)}:: IBus.IsConnected: {isConnected}, at {DateTime.Now}"));

                _mqDispatcher = Dispatcher.CurrentDispatcher;
                InitEasyNetQEvent.Set();

                Dispatcher.Run();
            }));

            _mqThread.SetApartmentState(ApartmentState.MTA);
            _mqThread.IsBackground = true;
            _mqThread.Start();

            InitEasyNetQEvent.WaitOne();
            Bus = _bus;

            this.WriteLineDebugAndConsole($"{nameof(MqInitializer)}.{nameof(MqInitializer.Initialize)} end, at {DateTime.Now}");
        }

        public static IBus Bus { get; private set; }
    }
}