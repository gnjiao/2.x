using System;
using System.Diagnostics;

namespace Hdc.Toolkit
{
    public static class Ex
    {
        public static void WriteLineDebugAndConsole(this object any, string message)
        {
            Debug.WriteLine(message);
            Console.WriteLine(message);
        }
    }
}