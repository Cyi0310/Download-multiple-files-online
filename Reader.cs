using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OnlineBundleLocal
{
    public class Reader
    {
        public List<string> Lines { get; private set; }
        public bool IsCanDownload { get; private set; }
        public DownloadData DownloadData { get; private set; }

        public readonly string defaultJsonFile = "Setting.json";

        private readonly DownloaderView downloaderView;

        public Reader(DownloaderView downloaderView)
        {
            this.downloaderView = downloaderView;
        }

        public void ReadFile()
        {
            string jsonPath = CheckJson();
            if (!IsJsonExists(jsonPath) || jsonPath == string.Empty)
            {
                return;
            }

            DownloadData = ReadJsonSetting(jsonPath);
            if (DownloadData == null)
            {
                return;
            }

            if (!IsFileAndFolderExists())
            {
                return;
            }

            List<string> lines = ReadFile(DownloadData.FileName);

            if (lines == null)
            {
                return;
            }

            if (!IsReplaceExists(lines))
            {
                return;
            }

            Lines = lines;
        }


        private string CheckJson()
        {
            try
            {
                string currentPath = AppDomain.CurrentDomain.BaseDirectory;
                string jsonPath = Path.Combine(Directory.GetParent(currentPath).FullName, defaultJsonFile);
                return jsonPath;
            }
            catch (Exception e)
            {
                downloaderView.ExceptionErrorLog(e);
                return string.Empty;
            }
        }
        private bool IsJsonExists(string jsonPath)
        {
            bool isExists = File.Exists(jsonPath);
            if (!isExists)
            {
                downloaderView.JsonNotExists(jsonPath);
            }

            IsCanDownload = isExists;
            return isExists;
        }
        private DownloadData ReadJsonSetting(string jsonPath)
        {
            try
            {
                string jsonData = File.ReadAllText(jsonPath);
                return JsonSerializer.Deserialize<DownloadData>(jsonData);
            }
            catch (Exception e)
            {
                downloaderView.ExceptionErrorLog(e);

                return null;
            }
        }

        private bool IsFileAndFolderExists()
        {
            bool isExists = true;

            if (!File.Exists(DownloadData.FileName))
            {
                downloaderView.HandleError(DownloadData.FileName);
                isExists = false;
            }

            if (!Directory.Exists(DownloadData.SavePath))
            {
                downloaderView.HandleError(DownloadData.SavePath);
                isExists = false;
            }

            IsCanDownload = isExists;
            return isExists;
        }

        private bool IsReplaceExists(List<string> lines)
        {
            bool isExists = true;
            try
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (!lines[i].Contains(DownloadData.Replace))
                    {
                        downloaderView.HandleError(DownloadData.Replace);
                        isExists = false;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                downloaderView.ExceptionErrorLog(e);
                isExists = false;
            }
            IsCanDownload = isExists;
            return isExists;
        }

        private List<string> ReadFile(string fileName)
        {
            try
            {
                List<string> lineList = new List<string>();
                using (StreamReader streamReader = File.OpenText(fileName))
                {
                    string line;
                    downloaderView.ReadStart();

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        lineList.Add(line);
                        downloaderView.ReadProgress(line);
                    }
                    downloaderView.ReadEnd();
                }
                return lineList;
            }
            catch (Exception e)
            {
                downloaderView.ExceptionErrorLog(e);
                return null;
            }
        }

    }
}
