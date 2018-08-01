using System;
using System.Configuration;
using System.Net;

namespace YGOPro2_Updater
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "YGOPro2_Updater";
            // 线程数
            ServicePointManager.DefaultConnectionLimit = Int32.Parse(ConfigurationManager.AppSettings[name: "ConnectionLimit"]);
            // 忽略列表
            for (int i = 1; !string.IsNullOrEmpty(value: ConfigurationManager.AppSettings[name: $"ignore{i}"]); i++)
                Config.ignores.Add(item: ConfigurationManager.AppSettings[name: $"ignore{i}"].Replace(oldValue: "*", newValue: "[^/]*?"));
            int arguments = args.Length;
            // whether the Arguments are Correct
            bool isOK = false;

            // Check the Ignore Sound Option
            if (arguments > 0 && args[arguments - 1] == "--ignore-sound")
            {
                Config.ignoreSound = true;
                arguments--;
            }

            switch (arguments)
            {
                case 0: // Download and Update the All Files
                    for (Config.count = 1; !string.IsNullOrEmpty(value: ConfigurationManager.AppSettings[name: $"name{Config.count}"]); Config.count++)
                    {
                        Config.SetWorkPath(workPath: null, name: null, repo: null);
                        Client.Run(); // 开始実行
                    }
                    break;
                case 1: // Workpath Setting Option
                    for (Config.count = 1; !string.IsNullOrEmpty(value: ConfigurationManager.AppSettings[name: $"name{Config.count}"]); Config.count++)
                    {
                        Config.SetWorkPath(workPath: args[0], name: null, repo: null);
                        Client.Run(); // 开始実行
                    }
                    break;
                case 2: // Download and Update the Files from the Specific Repository
                    for (int i = 1; !string.IsNullOrEmpty(value: ConfigurationManager.AppSettings[name: $"name{i}"]); i++)
                    {
                        if (args[0] == ConfigurationManager.AppSettings[name: $"name{i}"]
                            && args[1] == ConfigurationManager.AppSettings[name: $"repo{i}"])
                        {
                            Config.count = i;
                            isOK = Config.SetWorkPath(workPath: null, name: args[0], repo: args[1]);
                            Client.Run(); // 开始実行
                        }
                    }
                    if (!isOK)
                        Console.WriteLine(value: "You typed the wrong username or incorerct repository.");
                    break;
                case 3: // Download and Update the Files from the Specific Repository with Workpath Setting Option
                    for (int i = 1; !string.IsNullOrEmpty(value: ConfigurationManager.AppSettings[name: $"user{i}"]); i++)
                    {
                        if (args[1] == ConfigurationManager.AppSettings[name: $"name{i}"]
                            && args[2] == ConfigurationManager.AppSettings[name: $"repo{i}"])
                        {
                            Config.count = i;
                            isOK = Config.SetWorkPath(workPath: args[0], name: args[1], repo: args[2]);
                            Client.Run(); // 开始実行
                        }
                    }
                    if (!isOK)
                        Console.WriteLine(value: "You mistyped the workpath, username or repository.");
                    break;
                default:
                    Console.WriteLine(value: "The arguments which you typed are unavailble.");
                    break;
            }
            Console.WriteLine(value: "Press Any Key to continue ... ... ");
            Console.ReadKey(intercept: true);
        }
    }
}