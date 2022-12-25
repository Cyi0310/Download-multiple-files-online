using System;

namespace OnlineBundleLocal
{
    public abstract class Log
    {
        protected void BasicPrintLog(string log)
        {
            Console.WriteLine(log);
        }

        protected void SpecialPrintLog(string logName, string log)
        {
            BasicPrintLog($"\n --{logName}--" +
               $"\n {log}" +
               $"\n --{logName } end--");
        }

    }
}
