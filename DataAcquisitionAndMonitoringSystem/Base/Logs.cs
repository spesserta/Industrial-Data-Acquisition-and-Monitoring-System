using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text;
using System.Threading.Tasks;



namespace CompanyCore.Logger
{
    /// <summary>
    /// 有BUG暂停使用
    /// </summary>
    public class Logs
    {
        public static string path = AppDomain.CurrentDomain.BaseDirectory + "logs"; //日志地址

        public static object LogLocker = new object();

        protected static void WriteLog(string type, string content)
        {
            lock (LogLocker) //lock多线程环境下写入日志时发生冲突
            {
                if (!Directory.Exists(path))  //如果日志目录不存在就创建目录
                {
                    Directory.CreateDirectory(path);
                }
                string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string filename = path + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                StreamWriter sw = File.AppendText(filename);
                string write_content = time + " " + type + ":" + content;
                sw.WriteLine(write_content);
                sw.Close();
            }
        }

        public static void Debug(string content) => WriteLog("debug", content);
        public static void Info(string content) => WriteLog("info", content);
        public static void Error(string content) => WriteLog("error", content);
        public static void Error(Exception ex) => WriteLog("error", ex.Message + ex.StackTrace);

    }
}