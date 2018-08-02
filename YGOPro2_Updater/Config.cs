using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace YGOPro2_Updater
{
    internal class Config
    {
        /// <summary>工作目录</summary>
        public static string workPath = ConfigurationManager.AppSettings[name: "path"];
        /// <summary>信息目录</summary>
        public static string infoPath = Path.Combine(path1: workPath, path2: "YGOPro2_Updater");
        /// <summary>User or Group Name</summary>
        public static string name;
        /// <summary>Repository Name</summary>
        public static string repo;
        /// <summary>Project ID</summary>
        public static string ID;
        /// <summary>Download Filter</summary>
        public static bool filter;
        /// <summary>下载网址</summary>
        public static string url;

        /// <summary>SHA Key</summary>
        public static string SHAFile;
        /// <summary>错误列表</summary>
        public static string errorFile;
        /// <summary>Base SHA</summary>
        public static string oldSHA;
        /// <summary>New SHA</summary>
        public static string newSHA;

        /// <summary>Ignored Files</summary>
        public static List<string> ignores = new List<string>();
        /// <summary>Download without Sound</summary>
        public static bool ignoreSound = false;
        /// <summary>The Number of the Multiple Repositories</summary>
        public static int count;

        public static string GetPath(string fileName) => Path.Combine(path1: workPath, path2: fileName.Replace(oldChar: '/', newChar: Path.DirectorySeparatorChar));

        public static bool SetWorkPath(string workPath, string name, string repo)
        {
            if (!string.IsNullOrEmpty(value: workPath))
                Config.workPath = workPath;
            if (string.IsNullOrEmpty(value: name) || string.IsNullOrEmpty(value: repo))
            {
                Config.name = ConfigurationManager.AppSettings[name: $"name{count}"];
                Config.repo = ConfigurationManager.AppSettings[name: $"repo{count}"];
            }
            else
            {
                Config.name = name;
                Config.repo = repo;
            }
            ID = ConfigurationManager.AppSettings[name: $"ID{count}"];
            filter = Convert.ToBoolean(ConfigurationManager.AppSettings[name: $"filter{count}"]);
            if (string.IsNullOrEmpty(value: ID))
                url = $"https://raw.githubusercontent.com/{Config.name}/{Config.repo}/master/";
            else
                url = $"https://gitlab.com/{Config.name}/{Config.repo}/raw/master/";

            if (!Directory.Exists(path: infoPath))
                Directory.CreateDirectory(path: infoPath);
            SHAFile = Path.Combine(path1: infoPath, path2: $"version{count}.txt");
            errorFile = Path.Combine(path1: infoPath, path2: $"error{count}.txt");

            if (File.Exists(path: SHAFile))
                oldSHA = File.ReadAllText(path: SHAFile, encoding: Encoding.UTF8);
            // The URL to Get the Latest SHA Key of Commit
            string temp;
            if (string.IsNullOrEmpty(value: ID))
            {
                temp = $"https://api.github.com/repos/{Config.name}/{Config.repo}/branches/master";
                newSHA = JsonConvert.DeserializeObject<RootObject>(value: MyHttp.GetHtmlContent(requestUriString: temp)).commit.commit.tree.sha;
            }
            else
            {
                temp = $"https://gitlab.com/api/v4/projects/{ID}/repository/branches/master";
                newSHA = JsonConvert.DeserializeObject<Root>(value: MyHttp.GetHtmlContent(requestUriString: temp)).commit.id;
            }
            return true;
        }

        #region GitHub REST API v3
        /// <summary>
        /// The getters and setters for GitHub API
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public class Author
        {
            public string name { get; set; }
            public string email { get; set; }
            public DateTime date { get; set; }
        }

        public class Committer
        {
            public string name { get; set; }
            public string email { get; set; }
            public DateTime date { get; set; }
        }

        public class Tree
        {
            public string sha { get; set; }
            public string url { get; set; }
        }

        public class Verification
        {
            public bool verified { get; set; }
            public string reason { get; set; }
            public object signature { get; set; }
            public object payload { get; set; }
        }

        public class Commit2
        {
            public Author author { get; set; }
            public Committer committer { get; set; }
            public string message { get; set; }
            public Tree tree { get; set; }
            public string url { get; set; }
            public int comment_count { get; set; }
            public Verification verification { get; set; }
        }

        public class Author2
        {
            public string login { get; set; }
            public int id { get; set; }
            public string node_id { get; set; }
            public string avatar_url { get; set; }
            public string gravatar_id { get; set; }
            public string url { get; set; }
            public string html_url { get; set; }
            public string followers_url { get; set; }
            public string following_url { get; set; }
            public string gists_url { get; set; }
            public string starred_url { get; set; }
            public string subscriptions_url { get; set; }
            public string organizations_url { get; set; }
            public string repos_url { get; set; }
            public string events_url { get; set; }
            public string received_events_url { get; set; }
            public string type { get; set; }
            public bool site_admin { get; set; }
        }

        public class Committer2
        {
            public string login { get; set; }
            public int id { get; set; }
            public string node_id { get; set; }
            public string avatar_url { get; set; }
            public string gravatar_id { get; set; }
            public string url { get; set; }
            public string html_url { get; set; }
            public string followers_url { get; set; }
            public string following_url { get; set; }
            public string gists_url { get; set; }
            public string starred_url { get; set; }
            public string subscriptions_url { get; set; }
            public string organizations_url { get; set; }
            public string repos_url { get; set; }
            public string events_url { get; set; }
            public string received_events_url { get; set; }
            public string type { get; set; }
            public bool site_admin { get; set; }
        }

        public class Parent
        {
            public string sha { get; set; }
            public string url { get; set; }
            public string html_url { get; set; }
        }

        public class Commit
        {
            public string sha { get; set; }
            public string node_id { get; set; }
            public Commit2 commit { get; set; }
            public string url { get; set; }
            public string html_url { get; set; }
            public string comments_url { get; set; }
            public Author2 author { get; set; }
            public Committer2 committer { get; set; }
            public List<Parent> parents { get; set; }
        }

        public class Links
        {
            public string self { get; set; }
            public string html { get; set; }
        }

        public class RequiredStatusChecks
        {
            public string enforcement_level { get; set; }
            public List<object> contexts { get; set; }
        }

        public class Protection
        {
            public bool enabled { get; set; }
            public RequiredStatusChecks required_status_checks { get; set; }
        }

        public class RootObject
        {
            public string name { get; set; }
            public Commit commit { get; set; }
            public Links _links { get; set; }
            public bool @protected { get; set; }
            public Protection protection { get; set; }
            public string protection_url { get; set; }
        }
        #endregion

        #region GitLab REST API v4
        /// <summary>
        /// The getters and setters for GitLab API
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public class Commit3
        {
            public string id { get; set; }
            public string short_id { get; set; }
            public string title { get; set; }
            public DateTime created_at { get; set; }
            public List<string> parent_ids { get; set; }
            public string message { get; set; }
            public string author_name { get; set; }
            public string author_email { get; set; }
            public DateTime authored_date { get; set; }
            public string committer_name { get; set; }
            public string committer_email { get; set; }
            public DateTime committed_date { get; set; }
        }

        public class Root
        {
            public string name { get; set; }
            public Commit3 commit { get; set; }
            public bool merged { get; set; }
            public bool @protected { get; set; }
            public bool developers_can_push { get; set; }
            public bool developers_can_merge { get; set; }
            public bool can_push { get; set; }
        }
        #endregion
    }
}