using System;
using System.Threading;
using EasyNetQ;

namespace Hdc.Toolkit
{
    public static class EasyNetQEx
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="checkPeriod">in miliseconds</param>
        /// <param name="notifyIsConnnected"></param>
        public static void WaitForConnected(this IBus bus, int checkPeriod = 5000,
            Action<bool> notifyIsConnnected = null)
        {
            while (true)
            {
                var isConnected = bus.IsConnected;

                notifyIsConnnected?.Invoke(isConnected);

                if (isConnected)
                    break;

                Thread.Sleep(checkPeriod);
            }
        }

        public static IBus CreateBusAndWaitForConnnected(
            string host,
            string virualHost,
            string username,
            string password,
            int checkPeriod = 5000,
            Action<bool> notifyIsConnnected = null)
        {
            var bus = RabbitHutch.CreateBus($"host={host};" +
                                            $"virtualHost={virualHost};" +
                                            $"username={username};" +
                                            $"password={password};" +
                                            $"persistentMessages=false;" + 
                                            $"prefetchcount=1"
                );

            bus.WaitForConnected(checkPeriod, notifyIsConnnected);
            return bus;
        }
    }
}