using ReaLTaiizor.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LZTools
{
    //internal class Download
    public static class LZDownlaod
    {
        public static void DownLoadFile(string weburl, string filename, string downloadpath)
        {
            string url = weburl + filename;

            WebClient webDown = new WebClient();

            try
            {
                // 下载文件
                webDown.DownloadFile(url, downloadpath + filename);
                // 进行读取
            }
            catch (Exception ex)
            {
                MessageBox.Show("文件下载失败：" + ex.Message);
            }
            finally
            {
                // 释放 WebClient 对象
                webDown.Dispose();
            }
        }

        public static void CheckDownload()
        {
            DownLoadFile("http://43.139.159.239:9999/", "update2025.exe", Environment.CurrentDirectory + @"\");
        }


        public static string GetWeb()
        {
            string weburl = "http://www.wowlz.com";
            var PosData = new FormUrlEncodedContent(new[]
{
            new KeyValuePair<string,string>("username","guest"),
            }
);
            using (var hClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = hClient.PostAsync("http://43.139.159.239:9999/getweb.php", PosData).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        weburl = response.Content.ReadAsStringAsync().Result;
                        // if (DateTime.Today >= DateTime.Parse(VerDate)) { Application.Exit(); Environment.Exit(0); }
                    }
                }
                catch { }
            }

            return weburl;
        }

        public static bool CheckData(Form MainFrom,string ver)
        {

            string VerDate ="";
            var PosData = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string,string>("username","guest"),
            }
            ); ;


            using (var hClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = hClient.PostAsync("http://43.139.159.239:9999/checkdate.php", PosData).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        VerDate = response.Content.ReadAsStringAsync().Result;
                        // if (DateTime.Today >= DateTime.Parse(VerDate)) { Application.Exit(); Environment.Exit(0); }
                    }
                }
                catch { Application.Exit(); Environment.Exit(0); }
            }

            //ToTip("服务器版本：" + VerDate + "  本地版本:" + ver, MainFrom);

            if (Convert.ToInt16(VerDate) > Convert.ToInt16(ver))
            {
                //DialogResult result = MessageBox.Show("登录器版本需要更新，是否立刻更新？", "确认", MessageBoxButtons.OKCancel);
                //MessageBox.Show("系统版本不对，请更新专用登录器（下列网页中）");
                /*
                if (result == DialogResult.OK && int.Parse(ver) <= 110)
                {
                    Process.Start(Environment.CurrentDirectory + @"\update.exe");
                    // 在这里执行确定按钮的操作
                    Environment.Exit(1);
                }
                */
                return true;
            }
            return false;
        }

        public static void UpdateMain()
        {
            Process.Start(Environment.CurrentDirectory + @"\update.exe");
            // 在这里执行确定按钮的操作
            Environment.Exit(1);
        }

        private static void ToTip(string a, Form MainFrom, int b = 300, int c = 50, int d = 2000)
        {
            ThreadStart TipNext = () => Tip.Show(a, b, c, d, MainFrom.Location.X + MainFrom.Size.Width / 2, MainFrom.Location.Y + MainFrom.Height);
            Thread DoTip = new Thread(TipNext);
            DoTip.Start();
            //Tip.Show(a, b, c, d, this.Location.X + this.Size.Width / 2, this.Location.Y + this.Height);
        }


        public static void OpenUrlInDefaultBrowser(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

        }
    }
}
