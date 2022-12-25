namespace OnlineBundleLocal
{
    public class ErrorLog : Log, ILog
    { 
        public void PrintLog(string log)
        {
            base.SpecialPrintLog(nameof(ErrorLog), log);
        }
    }
}