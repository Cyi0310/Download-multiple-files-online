using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace OnlineBundleLocal
{    public class Downloader
    {
        private readonly string errorText = "ErrorLog.txt";
        private readonly DownloaderView downloaderView;

        private long nowMilliseconds = 0;
        private int allSuccessCount = 0;
        private bool isDownloadTooLong = false;
        private Stopwatch totalStopWatch;

        public Downloader(DownloaderView downloaderView)
        {
            this.downloaderView = downloaderView;
        }

        public async Task Download(List<string> lines, DownloadData downloadData, string defaultJsonFile)
        {
            totalStopWatch = Stopwatch.StartNew();

            await MainDownload(lines, downloadData, defaultJsonFile);

            Close(lines);
        }
        private async Task MainDownload(List<string> lines, DownloadData downloadData, string defaultJsonFile)
        {
            string defaultSavePath = downloadData.SavePath;
            string saveName;
            string saveSubPath;

            try
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    Stopwatch perDownloadStopWatch = Stopwatch.StartNew();

                    saveSubPath = lines[i].Replace(downloadData.Replace, string.Empty);
                    saveSubPath = saveSubPath.Replace("/", @"\");

                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(lines[i]);
                        response.EnsureSuccessStatusCode();

                        saveName = Path.GetFileName(lines[i]);

                        saveSubPath = Path.Combine(defaultSavePath, saveSubPath.Replace(saveName, string.Empty));
                        Directory.CreateDirectory(saveSubPath);
                        string savePath = Path.Combine(saveSubPath, saveName);

                        File.WriteAllBytes(savePath, await response.Content.ReadAsByteArrayAsync());
                        CheckDownloadSuccess(response);
                    }

                    perDownloadStopWatch.Stop();

                    long perDownloadTime = perDownloadStopWatch.ElapsedMilliseconds;
                    nowMilliseconds += perDownloadTime;

                    downloaderView.Progress(i + 1, lines.Count, saveName, saveSubPath, perDownloadTime);

                    if (IsCalculatePerDownloadTimeCheckDownloadTooLong(downloadData.MaxMilliSecondsTime))
                    {
                        downloaderView.DownLoadTooLong(nowMilliseconds, downloadData.MaxMilliSecondsTime, defaultJsonFile);
                        break;
                    }
                }
            }
            catch (System.Exception e)
            {
                downloaderView.ExceptionErrorLog(e);
            }
        }
        private bool IsCalculatePerDownloadTimeCheckDownloadTooLong(long maxMilliSecondsTime)
        {
            isDownloadTooLong = nowMilliseconds >= maxMilliSecondsTime;
            return isDownloadTooLong;
        }
        private void CheckDownloadSuccess(HttpResponseMessage response)
        {
            bool isDownloadSuccess = (int)response.StatusCode <= 200;
            if (isDownloadSuccess)
            {
                allSuccessCount++;
            }
            else
            {
                try
                {
                    if (File.Exists(errorText))
                    {
                        using (TextWriter tw = new StreamWriter(errorText, true))
                        {
                            tw.WriteLine(response);
                        }
                    }
                    else
                    {
                        File.Create(errorText).Dispose();
                    }

                    downloaderView.DownLoadIsFail((int)response.StatusCode);
                }                
                catch (System.Exception e)
                {
                    downloaderView.ExceptionErrorLog(e);
                }
            }
        }

        private void Close(List<string> lines)
        {
            totalStopWatch.Stop();

            bool isFail = allSuccessCount < lines.Count || isDownloadTooLong;

            downloaderView.Conclusion(totalStopWatch.ElapsedMilliseconds, isFail);
        }
    }
}