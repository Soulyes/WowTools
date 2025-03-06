using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Web;
using System.Net.Http;

namespace update
{
    public partial class UpdateAPP : Form
    {
        public UpdateAPP()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void UpdateAPP_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            ThreadStart UpdateStart = () => ToUpdateSystem();
            Thread STUP = new Thread(UpdateStart);
            STUP.IsBackground = true;
            STUP.Start();
        }

        private async void ToUpdateSystem()
        {
            Thread.Sleep(3000);
            //进行文件列表读取

            string url = "http://wd.wowlz.com:9999/linkdata.html";
            //创建目录

            string directoryPath = Environment.CurrentDirectory+ @"\runtimes\win-x64\native";
            //MessageBox.Show(directoryPath);
            try
            {
                // 创建目录
                Directory.CreateDirectory(directoryPath);

                // 检查目录是否创建成功
                if (Directory.Exists(directoryPath))
                {
                    Console.WriteLine($"目录创建成功: {directoryPath}");
                }
                else
                {
                    Console.WriteLine("目录创建失败！");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("创建目录时发生错误: " + ex.Message);
            }

            try
            {
                // 创建 HttpClient 对象
                using (HttpClient client = new HttpClient())
                {
                    // 发送 GET 请求并获取响应
                    HttpResponseMessage response = await client.GetAsync(url);

                    // 确保请求成功
                    response.EnsureSuccessStatusCode();

                    // 读取响应内容
                    string content = await response.Content.ReadAsStringAsync();

                    // 按行拆分内容
                    string[] lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                    int i = 0;
                    // 逐行处理
                    foreach (string line in lines)
                    {
                        // 按 "," 拆分每一行
                        string[] parts = line.Split(',');
                        await DownLoadFilesYun(parts[1], parts[0],Environment.CurrentDirectory + @"\");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("请求失败: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("发生错误: " + ex.Message);
            }

            string mainExe = "LZTools.exe";
            try
            {
                DownLoadFiles("http://43.139.159.239:9999/", mainExe, Environment.CurrentDirectory + @"\");
            }
            catch(Exception Ex)
            {
                MessageBox.Show("下载失败，程序可能正在运行中！");
                MessageBox.Show(Ex.ToString());
                Environment.Exit(1);
            }
            /*
            if(File.Exists(Environment.CurrentDirectory + @"\ReaLTaiizor.dll") == false)
            {
                DownLoadFiles("http://43.139.159.239:9999/", "ReaLTaiizor.dll", Environment.CurrentDirectory + @"\");
            }
            */


            label1.Text = "          更新完毕";
            Thread.Sleep(3000);
            Process.Start(Environment.CurrentDirectory + @"\" + mainExe);
            // 在这里执行确定按钮的操作
            Environment.Exit(1);

        }

        private async Task DownLoadFilesYun(string weburl, string filename, string downloadpath)
        {
            string url = weburl;

            WebClient webDown = new WebClient();

            //MessageBox.Show(url);
            //MessageBox.Show(downloadpath + filename);

            try
            {
                webDown.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
                webDown.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCompletedCallback);

                // 下载文件
                label1.Font = new Font(label1.Font.FontFamily, 12, label1.Font.Style);
                label1.Text = "更新：" + filename;
                await webDown.DownloadFileTaskAsync(new Uri(url), downloadpath + filename);
                // 进行读取
            }
            catch (Exception ex)
            {
                //MessageBox.Show("文件下载失败：请重试！");
                label1.Font = new Font(label1.Font.FontFamily, 12, label1.Font.Style);
                label1.Text = "文件下载" + filename + "失败：请重试";
                await Task.Delay(500);
                Environment.Exit(1);
            }

            finally
            {
                // 释放 WebClient 对象
                webDown.Dispose();
            }
            Thread.Sleep(3000);

        }

        // 下载进度回调
        private void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            // 更新进度条或标签
            int progressPercentage = e.ProgressPercentage;
            long bytesReceived = e.BytesReceived;
            long totalBytesToReceive = e.TotalBytesToReceive;

            // 更新 UI（例如 Label 或 ProgressBar）
            label2.Text = $"下载中：{progressPercentage}% ({bytesReceived}/{totalBytesToReceive} 字节)";
        }

        // 下载完成回调
        private void DownloadCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // 下载失败
                label2.Text = "文件下载失败：请重试";
            }
            else
            {
                // 下载成功
                label2.Text = "-";
            }
        }

        private void DownLoadFiles(string weburl, string filename, string downloadpath)
        {
            string url = weburl + filename;

            WebClient webDown = new WebClient();

            //MessageBox.Show(url);
            //MessageBox.Show(downloadpath + filename);

            try
            {
                // 下载文件
                //label1.Font = new Font(label1.Font.FontFamily, 16, label1.Font.Style);
                label1.Text = "更新：" + filename ;
                webDown.DownloadFile(url, downloadpath + filename);
                // 进行读取
            }
            catch (Exception ex)
            {
                //MessageBox.Show("文件下载失败：请重试！");
                //label1.Font = new Font(label1.Font.FontFamily, 14, label1.Font.Style);
                label1.Text = "文件下载" + filename + "失败：请重试";
                Thread.Sleep(3000);
                Environment.Exit(1);
            }

            finally
            {
                // 释放 WebClient 对象
                webDown.Dispose();
            }
            Thread.Sleep(3000);

        }

    }
}
