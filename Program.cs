using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineBundleLocal
{
    class Program
    {
        static async Task Main(string[] args)
        {
            DownloaderView downloaderView = new DownloaderView();
            Downloader bundleTool = new Downloader(downloaderView);
            Reader reader = new Reader(downloaderView);

            reader.ReadFile();

            if (!reader.IsCanDownload)
            {
                return;
            }

            await bundleTool.Download(reader.Lines, reader.DownloadData, reader.defaultJsonFile);
        }
    }
}
