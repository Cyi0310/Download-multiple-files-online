namespace OnlineBundleLocal
{
    public class SuggestLog : Log, ILog
    {
        public void PrintLog(string log)
        {
            base.SpecialPrintLog(nameof(SuggestLog), log);
        }
    }
}