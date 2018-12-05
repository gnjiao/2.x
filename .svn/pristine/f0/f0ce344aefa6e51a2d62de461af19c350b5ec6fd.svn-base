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
using Omron.Cxap.Modules.DisplacementSensorSDK;
using Omron.Cxap.Modules.DisplacementSensorSDK.CommHelper;

namespace Hdc.Measuring.OmronZW
{
    [Serializable]
    public class OmronZw2MeasureDevice : IMeasureDevice
    {
        private int _tagCounter;
        private static bool _isInitialized;
        private static DSComm dsComm = null;

        public static DSComm DsComm
        {
            get { return dsComm; }
        }

        public void Initialize()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            dsComm = new DSComm(DSComm.Version.ZW2);

            // Converts the entered IP address to a byte array
            byte[] ipaddress = {192, 168, 250, 50};

            string[] splitIp = IpAddress.Split('.');

            for (int i = 0; i < splitIp.Length; i++)
            {
                if (string.IsNullOrEmpty(splitIp[i]) == false)
                {
                    // For an integer, converts it to the byte type to set
                    ipaddress[i] = byte.Parse(splitIp[i]);
                }
                else
                {
                    // For other than an integer, sets to 0
                    ipaddress[i] = 0;
                }
            }

            // Makes a connection to the sensor
            var ret = dsComm.Open(ipaddress, DisConnectDelegate);

            // Check of the result of connection processing
            if (ret != CommErr.OK)
            {
                "CommErr ERROR".WriteLineInConsoleAndDebug();
            }
            else
            {
                "CommErr OK".WriteLineInConsoleAndDebug();
            }

            int timeout;
            ret = dsComm.GetTimeOutValue(out timeout);
            // Check of the result of connection processing
            if (ret != CommErr.OK)
            {
                "GetTimeOutValue ERROR".WriteLineInConsoleAndDebug();
            }
            else
            {
                $"GetTimeOutValue OK: {timeout}".WriteLineInConsoleAndDebug();
            }

            ret = dsComm.SetTimeOutValue(Timeout);
            // Check of the result of connection processing
            if (ret != CommErr.OK)
            {
                "SetTimeOutValue ERROR".WriteLineInConsoleAndDebug();
            }
            else
            {
                $"SetTimeOutValue OK: {Timeout}".WriteLineInConsoleAndDebug();
            }

            ret = dsComm.GetTimeOutValue(out timeout);
            // Check of the result of connection processing
            if (ret != CommErr.OK)
            {
                "GetTimeOutValue ERROR".WriteLineInConsoleAndDebug();
            }
            else
            {
                $"GetTimeOutValue OK: {timeout}".WriteLineInConsoleAndDebug();
            }
        }

        /// <summary>
        /// Event upon interruption of the connection to the sensor
        /// </summary>
        private void DisConnectDelegate()
        {
            "DisConnectDelegate raised".WriteLineInConsoleAndDebug();
        }

        public bool IsInitialized { get; }

        public string IpAddress { get; set; }

        public int Timeout { get; set; } = 10000;

        public MeasureResult Measure(MeasureEvent measureEvent)
        {
            // Obtains the measurement results
            var measureResult =  new MeasureResult();

            for (var circle = 0; circle <= 2; circle++)
            {
                int[] measureData;
                var ret = dsComm.GetMeasurementValue(DSComm.Task.ALL, out measureData);
                if (ret != CommErr.OK)
                {
                    throw new InvalidOperationException();
                }

                var now = DateTime.Now;
                measureResult = new MeasureResult()
                {
                    Tag = _tagCounter,
                    MeasureDateTime = now,
                };

                var bflag = false;

                for (int i = 0; i < 4; i++)
                {
                    var measureOutput = new MeasureOutput()
                    {
                        Index = i,
                        Validity = MeasureValidity.Valid,
                        Judge = MeasureOutputJudge.Go,
                        MeasureEvent = measureEvent,
                        Value = 0,
                    };
                    measureResult.Outputs.Add(measureOutput);

                     if (measureData[i] == int.MaxValue)
                    {
                        measureOutput.Value = -999;
                        measureOutput.Validity = MeasureValidity.Error;
                    }
                    else
                    {
                        measureOutput.Value = (measureData[i]/1000000.0);
                        bflag = true;
                    }
                }

                if(bflag)
                    break;

                Thread.Sleep(100);
            }

            return measureResult;
        }
    }
}