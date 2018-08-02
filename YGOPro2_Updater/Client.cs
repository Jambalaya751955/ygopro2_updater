using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YGOPro2_Updater
{
    internal class Client
    {
        /// <summary>文件列表</summary>
        private static ConcurrentDictionary<string, string> fileList;
        /// <summary>The Files which were Downloaded Previous Time</summary>
        private static ConcurrentDictionary<string, string> oldFileList;
        /// <summary>The Files which Failed to Update</summary>
        private static ConcurrentDictionary<string, string> errorList;
        /// <summary>The Number of the All Downloding Files</summary>
        private static int All_num;
        /// <summary>Progress</summary>
        private static int Num;

        private static void Download()
        {
            Parallel.ForEach(fileList, ff =>
            {
                if (!string.IsNullOrEmpty(value: ff.Key) && !string.IsNullOrEmpty(value: ff.Value))
                {
                    Console.Title = $"PROGRESS : {++Num}/{All_num}";

                    if (Config.ignoreSound && (ff.Key.EndsWith(value: ".mp3", comparisonType: StringComparison.OrdinalIgnoreCase)
                        || ff.Key.EndsWith(value: ".ogg", comparisonType: StringComparison.OrdinalIgnoreCase)
                        || ff.Key.EndsWith(value: ".wav", comparisonType: StringComparison.OrdinalIgnoreCase)))
                        //Ignore Sound
                        Console.WriteLine(value: $"SOUND IGNORED : {ff.Key}");
                    else if (MyUtil.CheckIgnores(fileName: ff.Key))
                        //忽略更新
                        Console.WriteLine(value: $"IGNORED : {ff.Key}");
                    else if (File.Exists(path: Config.SHAFile) && File.Exists(path: Config.GetPath(fileName: ff.Key))
                        && oldFileList.ContainsKey(key: ff.Key) && ff.Value == oldFileList[key: ff.Key])
                    {
                        //一致
                        //Console.WriteLine(value: $"SKIPPED:{fileName}");
                    }
                    else
                    {
                        string fileName = ff.Key;
                        //Check Download Filter
                        if (Config.filter)
                            fileName = MyUtil.DownloadFilter(fileName: ff.Key);
                        //下载文件
                        new MyHttp().Download(url: Config.url + ff.Key, path: Config.GetPath(fileName: fileName));
                        //Show the State whether Download was Success
                        if (File.Exists(path: Config.GetPath(fileName: fileName)))
                            Console.WriteLine(value: $"DOWNLOAD COMPLETE : {ff.Key}");
                        else if (string.IsNullOrEmpty(value: ff.Key) || string.IsNullOrEmpty(value: ff.Value))
                            Console.WriteLine(value: "DOWNLOAD FAILED");
                        else
                        {
                            Console.WriteLine(value: $"DOWNLOAD FAILED : {Config.url + ff.Key}");
                            errorList.TryAdd(key: ff.Key, value: ff.Value);
                        }
                    }
                }
            });
        }

        private static void Update()
        {
            if (File.Exists(path: Config.SHAFile))
            {
                oldFileList = new ConcurrentDictionary<string, string>();
                MyUtil.MakeFileinfo(SHA: Config.oldSHA, fileinfo: oldFileList);
            }
            errorList = new ConcurrentDictionary<string, string>();
            All_num = fileList.Count;
            Num = 0;

            Download(); // 开始下载
            if (errorList.Count > 0)
            {
                Console.WriteLine(value: "Some of files failed to update ... ...");
                Console.WriteLine(value: "Please retry the update later.");
                MyUtil.SaveList(path: Config.errorFile, fileinfo: errorList);
            }
            else
            {
                // Save SHA Value
                MyUtil.CreateDir(path: Config.SHAFile);
                File.WriteAllText(path: Config.SHAFile, contents: Config.newSHA, encoding: Encoding.UTF8);
                Console.WriteLine(value: "Update success! Though keep this windows until [Press Any Key to continue ... ... ].");
            }
        }

        public static void Run()
        {
            Console.WriteLine(value: "");
            Console.WriteLine(value: $"DOWNLOAD NUMBER : {Config.count}");
            Console.WriteLine(value: $"UPDATE FROM : {Config.name}'s {Config.repo}");
            Console.WriteLine(value: $"DOWNLOAD TO : {Config.workPath}");
            Console.WriteLine(value: $"CONFIG FILE : {Assembly.GetExecutingAssembly().Location}.config");
            if (Config.ignoreSound)
                Console.WriteLine(value: "ENABLE OPTION : IGNORE THE SOUND FILES");
            Console.WriteLine(value: "");

            fileList = new ConcurrentDictionary<string, string>();
            if (File.Exists(path: Config.errorFile))
            {
                Console.WriteLine(value: "Failed to update previous time ... ...");
                Console.WriteLine(value: "Restarting Update ... ...");

                Parallel.ForEach(File.ReadAllLines(path: Config.errorFile, encoding: Encoding.UTF8), line =>
                {
                    if (line.Split(separator: '\t').Length >= 2)
                        fileList.TryAdd(key: line.Split(separator: '\t')[0], value: line.Split(separator: '\t')[1]);
                });
                Update(); // 开始更新
            }
            else if (File.Exists(path: Config.SHAFile) && Config.newSHA == Config.oldSHA)
                // Match the SHA Key
                Console.WriteLine(value: $"{Config.name}'s {Config.repo} is already up-to-date!");
            else
            {
                // 上一次下载是否失败
                Console.WriteLine(value: "Discovered New Version ... ...");
                Console.WriteLine(value: "Download Filelist ... ...");
                MyUtil.MakeFileinfo(SHA: Config.newSHA, fileinfo: fileList);
                Console.WriteLine(value: "Start Update and Download ... ...");
                Update(); // 开始更新
            }
        }
    }
}