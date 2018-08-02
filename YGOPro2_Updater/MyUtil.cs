using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YGOPro2_Updater
{
    internal class MyUtil
    {
        public static void CreateDir(string path)
        {
            if (File.Exists(path: path))
                File.Delete(path: path);
            else if (!Directory.Exists(path: path.Substring(startIndex: 0, length: path.LastIndexOf(value: Path.DirectorySeparatorChar))))
                Directory.CreateDirectory(path: path.Substring(startIndex: 0, length: path.LastIndexOf(value: Path.DirectorySeparatorChar)));
        }

        public static bool CheckIgnores(string fileName)
        {
            foreach (string tmp in Config.ignores)
            {
                if (Regex.IsMatch(input: fileName, pattern: $"^{tmp}$", options: RegexOptions.IgnoreCase))
                    return true;
            }
            return false;
        }

        public static string DownloadFilter(string fileName)
        {
            if (fileName.EndsWith(value: ".cdb", comparisonType: StringComparison.OrdinalIgnoreCase))
                fileName = $"cdb/{Path.GetFileName(path: fileName)}";
            else if (fileName.EndsWith(value: ".conf", comparisonType: StringComparison.OrdinalIgnoreCase))
                fileName = $"config/{Path.GetFileName(path: fileName)}";
            else if (fileName.EndsWith(value: ".lua", comparisonType: StringComparison.OrdinalIgnoreCase))
                fileName = $"script/{Path.GetFileName(path: fileName)}";
            else if (fileName.EndsWith(value: "pack.db", comparisonType: StringComparison.OrdinalIgnoreCase))
                fileName = $"pack/{Path.GetFileName(path: fileName)}";
            return fileName;
        }

        public static void SaveList(string path, ConcurrentDictionary<string, string> fileinfo)
        {
            CreateDir(path: path);
            try
            {
                using (FileStream fs = new FileStream(path: path, mode: FileMode.Create, access: FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(stream: fs, encoding: Encoding.UTF8))
                    {
                        if (fileinfo != null)
                        {
                            foreach (KeyValuePair<string, string> ff in fileinfo)
                            {
                                sw.WriteLine(value: $"{ff.Key}\t{ff.Value}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("MyUtil.SaveList() fail,error:" + ex.Message);
            }
        }

        public static void MakeFileinfo(string SHA, ConcurrentDictionary<string, string> fileinfo)
        {
            if (string.IsNullOrEmpty(value: Config.ID))
            {
                // The URL to Get the Filelist
                string temp = $"https://api.github.com/repos/{Config.name}/{Config.repo}/git/trees/{SHA}?recursive=1";
                List<Tree> tree = JsonConvert.DeserializeObject<RootObject>(value: MyHttp.GetHtmlContent(requestUriString: temp)).tree;

                //所有文件
                Parallel.ForEach(tree, index =>
                {
                    if (index.type == "blob")
                    {
                        string fileName = Path.GetFileName(path: index.path);
                        if (fileName.EndsWith(value: "Thumbs.db", comparisonType: StringComparison.OrdinalIgnoreCase)
                           || fileName.EndsWith(value: ".gitattributes", comparisonType: StringComparison.OrdinalIgnoreCase)
                           || fileName.EndsWith(value: ".gitignore", comparisonType: StringComparison.OrdinalIgnoreCase)
                           || fileName.EndsWith(value: "LICENSE", comparisonType: StringComparison.OrdinalIgnoreCase)
                           || fileName.EndsWith(value: "appveyor.yml", comparisonType: StringComparison.OrdinalIgnoreCase)
                           || fileName.EndsWith(value: ".travis.yml", comparisonType: StringComparison.OrdinalIgnoreCase)
                           || fileName.EndsWith(value: "circle.yml", comparisonType: StringComparison.OrdinalIgnoreCase)
                           || fileName.EndsWith(value: "README.md", comparisonType: StringComparison.OrdinalIgnoreCase)
                           || fileName.EndsWith(value: "web.config", comparisonType: StringComparison.OrdinalIgnoreCase)
                           || fileName.EndsWith(value: "update-push.bat", comparisonType: StringComparison.OrdinalIgnoreCase)
                           || fileName.EndsWith(value: "update-server.bat", comparisonType: StringComparison.OrdinalIgnoreCase)
                           || fileName.EndsWith(value: "desktop.ini", comparisonType: StringComparison.OrdinalIgnoreCase)
                           || fileName.EndsWith(value: "start.htm", comparisonType: StringComparison.OrdinalIgnoreCase)
                           || index.path == Assembly.GetExecutingAssembly().Location
                           || index.path == $"{Assembly.GetExecutingAssembly().Location}.config"
                          )
                            return;
                        fileinfo.TryAdd(key: index.path, value: index.sha);
                    }
                });
            }
            else
            {
                for (int i = 1; true; i++)
                {
                    // The URL to Get the Filelist
                    string temp = $"https://gitlab.com/api/v4/projects/{Config.ID}/repository/tree?ref={SHA}&recursive=1&page={i}&per_page=100";
                    string body = MyHttp.GetHtmlContent(requestUriString: temp);
                    if (body == "[]")
                        break;
                    List<Root> root = JsonConvert.DeserializeObject<List<Root>>(value: body);

                    //所有文件
                    Parallel.ForEach(root, index =>
                    {
                        if (index.type == "blob")
                        {
                            if (index.name.EndsWith(value: "Thumbs.db", comparisonType: StringComparison.OrdinalIgnoreCase)
                               || index.name.EndsWith(value: ".gitattributes", comparisonType: StringComparison.OrdinalIgnoreCase)
                               || index.name.EndsWith(value: ".gitignore", comparisonType: StringComparison.OrdinalIgnoreCase)
                               || index.name.EndsWith(value: "LICENSE", comparisonType: StringComparison.OrdinalIgnoreCase)
                               || index.name.EndsWith(value: "appveyor.yml", comparisonType: StringComparison.OrdinalIgnoreCase)
                               || index.name.EndsWith(value: ".travis.yml", comparisonType: StringComparison.OrdinalIgnoreCase)
                               || index.name.EndsWith(value: "circle.yml", comparisonType: StringComparison.OrdinalIgnoreCase)
                               || index.name.EndsWith(value: "README.md", comparisonType: StringComparison.OrdinalIgnoreCase)
                               || index.name.EndsWith(value: "web.config", comparisonType: StringComparison.OrdinalIgnoreCase)
                               || index.name.EndsWith(value: "update-push.bat", comparisonType: StringComparison.OrdinalIgnoreCase)
                               || index.name.EndsWith(value: "update-server.bat", comparisonType: StringComparison.OrdinalIgnoreCase)
                               || index.name.EndsWith(value: "desktop.ini", comparisonType: StringComparison.OrdinalIgnoreCase)
                               || index.name.EndsWith(value: "start.htm", comparisonType: StringComparison.OrdinalIgnoreCase)
                               || index.path == Assembly.GetExecutingAssembly().Location
                               || index.path == $"{Assembly.GetExecutingAssembly().Location}.config"
                              )
                                return;
                            fileinfo.TryAdd(key: index.path, value: index.id);
                        }
                    });
                }
            }
        }

        #region GitHub API for Filelist
        /// <summary>
        /// The getters and setters for Filelist
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public class Tree
        {
            public string path { get; set; }
            public string mode { get; set; }
            public string type { get; set; }
            public string sha { get; set; }
            public int size { get; set; }
            public string url { get; set; }
        }

        public class RootObject
        {
            public string sha { get; set; }
            public string url { get; set; }
            public List<Tree> tree { get; set; }
            public bool truncated { get; set; }
        }
        #endregion

        #region GitLab API for Filelist
        /// <summary>
        /// The getters and setters for Filelist
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public class Root
        {
            public string id { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string path { get; set; }
            public string mode { get; set; }
        }
        #endregion
    }
}