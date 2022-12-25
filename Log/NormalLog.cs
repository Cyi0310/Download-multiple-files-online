namespace OnlineBundleLocal
{
    public class NormalLog : Log, ILog
    {
        public void PrintLog(string log)
        {
            base.BasicPrintLog(log);
        }
    }
}