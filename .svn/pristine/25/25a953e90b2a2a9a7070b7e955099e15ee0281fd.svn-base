using System;
using Topshelf.Logging;

namespace Hdc.Toolkit
{
    public static class TopShelfLogAndConsole
    {
        public static readonly LogWriter Writer = HostLogger.Current.Get("");

        public static void WriteLineInDebug(string value)
        {
            Console.WriteLine(value);
            Writer.Debug(value);
        }
    }
}