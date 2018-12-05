using System;
using System.IO.Ports;

namespace Hdc.Measuring
{
    [Serializable]
    public class E2v8kInitializer: IInitializer
    {
        SerialPort _port;

        public void Initialize()
        {
            _port = new SerialPort(PortName, BaudRate, Parity.None, 8, StopBits.One);
            _port.Open();

            if (!_port.IsOpen)
                _port.Open();

            if (ScanDirection_Enabled)
            {
                _port.Write($"w {ScanDirection_Command} {ScanDirection_Value}");
                _port.Write(new byte[] { 13 }, 0, 1);
            }

            if (Gain_Enabled)
            {
                _port.Write($"w {Gain_Command} {Gain_Value}");
                _port.Write(new byte[] { 13 }, 0, 1);
            }

            if (PreAmpGain_Enabled)
            {
                _port.Write($"w {PreAmpGain_Command} {PreAmpGain_Value}");
                _port.Write(new byte[] { 13 }, 0, 1);
            }

            if (ExposureTime_Enabled)
            {
                _port.Write($"w {ExposureTime_Command} {ExposureTime_Value}");
                _port.Write(new byte[] { 13 }, 0, 1);
            }

            //            _port.Close();
            //            _port.Dispose();
        }

        public void Dispose()
        {
            //            _port.Close();
            //            _port.Dispose();
        }

        public string PortName { get; set; } = "COM10";
        public int BaudRate { get; set; } = 9600;

        public bool ScanDirection_Enabled { get; set; }
        public string ScanDirection_Command { get; set; } = "scdi";
        public string ScanDirection_Value { get; set; } = "0"; // 0: forward, 1: reverse

        public bool Gain_Enabled { get; set; }
        public string Gain_Command { get; set; } = "gain"; // Gain
        public string Gain_Value { get; set; } = "0";  // 2dB => 1061

        public bool PreAmpGain_Enabled { get; set; }
        public string PreAmpGain_Command { get; set; } = "pamp"; // PreampGain
        public string PreAmpGain_Value { get; set; }  // 0 => x1(0dB), 1 => x2(6dB)

        public bool ExposureTime_Enabled { get; set; }
        public string ExposureTime_Command { get; set; } = "tint"; // ExposureTime
        public string ExposureTime_Value { get; set; } = "100"; //  ExposureTime 60us => w tint 600;
    }
}