using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;
namespace RefeshVisCntProject
{
    class Program
    {
        static string url = ConfigurationManager.AppSettings["url"];
        static string domain = ConfigurationManager.AppSettings["domain"];
        static void Main(string[] args)
        {
            Console.Title = "刷访问量";
            IntPtr ParenthWnd = new IntPtr(0);
            IntPtr et = new IntPtr(0);
            ParenthWnd = FindWindow(null, "刷访问量");
            ShowWindow(ParenthWnd, 0);//隐藏本dos窗体, 0: 后台执行；1:正常启动；2:最小化到任务栏；3:最大化
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(aTimer_Elapsed);
            // 设置引发时间的时间间隔 此处设置为１秒（１０００毫秒）
            aTimer.Interval = 1 * 60 * 1000;  //设置时间间隔
            aTimer.Enabled = true;
            Console.WriteLine("程序开始执行时间为：" + DateTime.Now.AddMinutes(1).ToString("yyyy-MM-dd HH:mm:ss"));
            Console.ReadLine();
        }
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]   //找子窗体   
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]   //用于发送信息给窗体   
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport("User32.dll", EntryPoint = "ShowWindow")]   //
        private static extern bool ShowWindow(IntPtr hWnd, int type);
        static void aTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Start Fetching:" + url);
            VisUrl(e.SignalTime);
        }
        async static void VisUrl(DateTime happenTime)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = await client.GetStringAsync(url);
                    var reg = new System.Text.RegularExpressions.Regex("<li class=\"prev_article\">.*</li>");
                    var match = reg.Match(content);
                    var li = match.Groups[0].Value;
                    reg = new System.Text.RegularExpressions.Regex("<a.*href=.*\"\\s");
                    match = reg.Match(li);
                    var a = match.Groups[0].Value;
                    reg = new System.Text.RegularExpressions.Regex("\".*");
                    match = reg.Match(a);
                    var href = match.Groups[0].Value;
                    href = href.Replace("\"", "");
                    url = domain + href;
                    Console.WriteLine("End Fetching:" + url);
                    Console.WriteLine("下次程序的执行：" + DateTime.Now.AddMinutes(1).ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ErrorMsg:csdn服务器连接不上");
            }
            finally
            {

            }
        }
    }
}
