using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;

namespace YGOPro2_Updater
{
    internal class MyHttp
    {
        #region Download a File
        /// <summary>
        /// Download a File
        /// <param name="requestUriString"></param>
        /// </summary>
        /// <returns>Downloaded File Name</returns>
        public void Download(string url, string path)
        {
            MyUtil.CreateDir(path: path);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString: url);
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                "(KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/17.17134";
            
            try
            {
                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (Stream st = httpWebResponse.GetResponseStream())
                    {
                        using (Stream so = new FileStream(path: path, mode: FileMode.Create))
                        {
                            byte[] by = new byte[2048];
                            int osize = st.Read(buffer: by, offset: 0, count: by.Length);
                            while (osize > 0)
                            {
                                so.Write(buffer: by, offset: 0, count: osize);
                                osize = st.Read(buffer: by, offset: 0, count: by.Length);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("MyHttp.Download() fail,error:" + ex.Message);
            }
        }
        #endregion

        #region 获取网址内容
        /// <summary>
        /// Get the Response Body
        /// <param name="requestUriString"></param>
        /// </summary>
        /// <returns>The String Body</returns>
        public static string GetHtmlContent(string requestUriString)
        {
            string htmlContent = string.Empty;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString: requestUriString);
            httpWebRequest.Timeout = 30000;
            httpWebRequest.Accept = "application/vnd.github.v3+json";
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                "(KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/17.17134";
            
            try
            {
                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (Stream stream = httpWebResponse.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(stream: stream, encoding: Encoding.UTF8))
                            htmlContent = streamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("MyHttp.GetHtmlContent() fail,error:" + ex.Message);
            }
            return htmlContent;
        }
        #endregion
    }
}