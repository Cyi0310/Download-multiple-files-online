using System;

namespace OnlineBundleLocal
{
    public class DownloaderView
    {
        private readonly ILog normalLog;
        private readonly ILog suggestLog;
        private readonly ILog errorLog;

        public DownloaderView()
        {
            normalLog = new NormalLog();
            suggestLog = new SuggestLog();
            errorLog = new ErrorLog();
        }

        public void ExceptionErrorLog(Exception e)
        {
            errorLog.PrintLog(e.Message);
            Console.ReadKey();
        }

        public void JsonNotExists(string jsonName)
        {
            errorLog.PrintLog($"({jsonName}) is not exists.");
            suggestLog.PrintLog($"you need to check ({jsonName}) is exists, and inside need to have content");
            Console.ReadKey();
        }

        public void HandleError(string text)
        {
            errorLog.PrintLog($"({text}) is not exists.");
            suggestLog.PrintLog($"you need to check ({text}) of path or name ect...");
            Console.ReadKey();
        }

        public void ReadStart()
        {
            normalLog.PrintLog("--Read start--");
        }
        public void ReadProgress(string line)
        {
            normalLog.PrintLog($"Read {line} ");
        }
        public void ReadEnd()
        {
            normalLog.PrintLog("--Read end--");
        }

        public void Progress(int nowProgressCount,int count,string saveName, string saveSubPath, long perDownloadTime)
        {
            normalLog.PrintLog($"\n Now progress : {nowProgressCount} / {count}" +
                $" \n name is : {saveName}" +
                $" \n send to : {saveSubPath}" +
                $" \n spend time is : {perDownloadTime} milliseconds");
        }

        public void DownLoadTooLong(long nowMilliseconds, int maxMilliSecondsTime, string defaultJsonFile)
        {
            errorLog.PrintLog($"Download is too long, already spend {nowMilliseconds} milliseconds, " +
                $"\n it's over MaxMilliSecondsTime : {maxMilliSecondsTime},");
            suggestLog.PrintLog($"you can increase {nameof(maxMilliSecondsTime)}, it exists in {defaultJsonFile}");
        }

        public void DownLoadIsFail(int statusCode)
        {
            errorLog.PrintLog($"Download is fail, and statusCode is : {statusCode}");
        }

        public void Conclusion(long ms, bool isFail)
        {
            normalLog.PrintLog($"\n Total spend time is : {ms} milliseconds " +
                $"/ {ms / 1000} seconds");

            string log = isFail ? "fail" : "success";
            normalLog.PrintLog($"\n Download is ({log})");
            
            Console.ReadKey();
        }
    }
}