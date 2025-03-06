using Microsoft.Win32;
using ReaLTaiizor.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using OpenCvSharp;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Schema;
using System.Windows.Forms;
using System.Web.UI.WebControls;

namespace LZTools
{
    public static class LZClass
    {
        public static bool ThreadRun = false;

        public static bool GetRunSt()
        {
            return ThreadRun;
        }

        public static void reftime(string TimeSet,HeaderLabel TimeLable)
        {
            int TimeStep = 420;
            try
            {
                TimeStep = Convert.ToInt16(TimeSet);
            }
            catch
            {
                TimeStep = 420;
            }
            ThreadRun = true;
            while (TimeStep > 0)
            {
                TimeLable.Text = TimeStep.ToString();
                TimeLable.Refresh();
                TimeStep--;
                Thread.Sleep(1000);
            }

            ThreadRun = false;
            SystemSounds.Hand.Play();
        }

        public static int CheckWinVer()
        {
            string ver = GetWindowsProductName();
            int vernum = 0;

            if (ver.Contains("Windows 7")) vernum = 7;
            if (ver.Contains("Windows 10")) vernum = 10;
            if (ver.Contains("Windows 11")) vernum = 11;

            return vernum;
        }

        public static string GetWindowsProductName()
        {
            const string registryKey = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
            const string productNameKey = "ProductName";

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey))
            {
                if (key != null)
                {
                    object productName = key.GetValue(productNameKey);
                    if (productName != null)
                    {
                        return productName.ToString();
                    }
                }
            }
            return null;
        }
        private const int SW_HIDE = 0; // 隐藏窗口
        private const int SW_SHOW = 5; // 显示窗口

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private static void HideShowID(Process process, ForeverButton button)
        {
            //MessageBox.Show(id.ToString());
            //Process process = Process.GetProcessById(id);

            if (process.MainWindowHandle == IntPtr.Zero)
            {
                MessageBox.Show("窗口句柄无效，无法操作。");
                return;
            }

            // 获取当前窗口状态
            bool isHidden = button.Text == "显示窗口";

            if (isHidden)
            {
                // 显示窗口
                ShowWindow(process.MainWindowHandle, SW_SHOW);
                button.Text = "隐藏窗口";
            }
            else
            {
                // 隐藏窗口
                ShowWindow(process.MainWindowHandle, SW_HIDE);
                button.Text = "显示窗口";
            }

            button.Refresh();
        }

        private static void HideShowID(Process process, ForeverButton button,bool aaa)
        {
            //MessageBox.Show(id.ToString());
            //Process process = Process.GetProcessById(id);

            if (process.MainWindowHandle == IntPtr.Zero)
            {
                MessageBox.Show("窗口句柄无效，无法操作。");
                return;
            }

            // 获取当前窗口状态

            if (aaa)
            {
                // 显示窗口
                ShowWindow(process.MainWindowHandle, SW_SHOW);
                button.Text = "隐藏窗口";
            }
            else
            {
                // 隐藏窗口
                ShowWindow(process.MainWindowHandle, SW_HIDE);
                button.Text = "显示窗口";
            }

            button.Refresh();
        }

        private static void HideShowID(Process process, ForeverButton button,ContextMenuStrip aa)
        {
            //MessageBox.Show(id.ToString());
            //Process process = Process.GetProcessById(id);

            if (process.MainWindowHandle == IntPtr.Zero)
            {
                MessageBox.Show("窗口句柄无效，无法操作。");
                return;
            }

            // 获取当前窗口状态
            bool isHidden = button.Text == "显示窗口";

            if (isHidden)
            {
                // 显示窗口
                ShowWindow(process.MainWindowHandle, SW_SHOW);
                button.Text = "隐藏窗口";
                aa.Text = "隐藏 " + aa.Text;
            }
            else
            {
                // 隐藏窗口
                ShowWindow(process.MainWindowHandle, SW_HIDE);
                button.Text = "显示窗口";
                aa.Text = "显示 " + aa.Text;
            }
            
            button.Refresh();
        }

        private static void HideShowID(Process process, ForeverButton button, string titlename,ContextMenuStrip aa)
        {
            //MessageBox.Show(id.ToString());
            //Process process = Process.GetProcessById(id);

            if (process.MainWindowHandle == IntPtr.Zero)
            {
                MessageBox.Show("窗口句柄无效，无法操作。");
                return;
            }

            ToolStripItem[] xx =  aa.Items.Find(titlename,true);


            // 获取当前窗口状态
            bool isHidden = button.Text == "显示窗口";

            if (isHidden)
            {
                // 显示窗口
                ShowWindow(process.MainWindowHandle, SW_SHOW);
                button.Text = "隐藏窗口";
                //xx[0].Text = "隐藏 " + aa.Text;
            }
            else
            {
                // 隐藏窗口
                ShowWindow(process.MainWindowHandle, SW_HIDE);
                button.Text = "显示窗口";
                //MessageBox.Show(Iindex.ToString() + " / " + aa.Items.Count.ToString());
                //xx[0].Text = "显示 " + aa.Text;
                //MessageBox.Show(titlename);

            }

            button.Refresh();
        }

        private static List<Process> WowList = new List<Process>();
        private static List<ForeverButton> WowButton = new List<ForeverButton>();

        public static void ZWsearch(System.Windows.Forms.TabPage zwin,ContextMenuStrip lzMenu)
        {
            Process[] ps = Process.GetProcesses();
            WowList.Clear();
            WowButton.Clear();
            try
            {
                for (int x = zwin.Controls.Count - 1; x >= 0; x--)
                {
                    if (zwin.Controls[x] is System.Windows.Forms.Label)
                    {
                        // 移除 Label 控件
                        zwin.Controls.RemoveAt(x);
                    }

                }

                for (int x = zwin.Controls.Count - 1; x >= 0; x--)
                {
                    if (zwin.Controls[x] is ForeverButton)
                    {
                        // 移除 Label 控件
                        zwin.Controls.RemoveAt(x);
                    }
                }
                lzMenu.Items.Clear();
            } catch { }


            int i = 0;
            foreach (Process p in ps)
            {
                if (p.ProcessName.Contains("Battle.net") && p.MainWindowHandle.ToString() != "0")
                {
                    //MessageBox.Show(p.Id.ToString());
                    //ToTip(p.MainWindowTitle + " - " + p.Id.ToString());

                    ForeverButton hiddenbutton = new ForeverButton()
                    { 
                        Text = "隐藏窗口",
                        Name = "P" + p.MainWindowTitle.ToString().Split('-')[1].Replace(" ",""),
                        Location = new Point(10,  5 + i * 60),
                        Size = new System.Drawing.Size(100,30),
                        Font = new Font("微软雅黑",10)
                    };
                    hiddenbutton.Click += (sender, e) => HideShowID(p, sender as ForeverButton);

                    WowButton.Add(hiddenbutton); 

                    System.Windows.Forms.Label label = new System.Windows.Forms.Label()
                    {
                        Text = p.MainWindowTitle.ToString(),
                        Name = "L" + p.MainWindowTitle.ToString().Split('-')[1].Replace(" ", ""),
                        Location = new Point(130, 10 + i * 60),
                        Size = new System.Drawing.Size(200, 20),
                        ForeColor = Color.White,
                        Margin = new Padding(0, 0, 0, 10) // 下边距 10px
                    };

                    lzMenu.Items.Add(p.MainWindowTitle.ToString(), null, (sender, e) => HideShowID(p,hiddenbutton, p.MainWindowTitle.ToString(), lzMenu));
                    WowList.Add(p);
                    
                    //MessageBox.Show(ItemsIndex.ToString() + " / "+ lzMenu.Items.Count.ToString());
                    lzMenu.Refresh();

                    i++;
                    zwin.Controls.Add(label);
                    zwin.Controls.Add(hiddenbutton);
                }

            }
            lzMenu.Items.Add("全部隐藏", null, (sender, e) => ShowCloseAll(false));
            lzMenu.Items.Add("全部显示", null, (sender, e) => ShowCloseAll(true));
            zwin.Refresh();
        }

        public static void ShowCloseAll(bool aaa)
        {
            for(int i = 0;i<WowList.Count;i++) 
            { 
                    HideShowID(WowList[i], WowButton[i],aaa);
            }
        }

        public static void DrawGroupBox(System.Windows.Forms.TabPage tabin)
        {
            Random random = new Random();
            int labelCount = 4;
            int height = 120;

            for (int i = 0; i < labelCount; i++)
            {
                System.Windows.Forms.GroupBox group = new System.Windows.Forms.GroupBox()
                {
                    Text = "版本更新：" + i.ToString(),
                    Location = new Point(5, i * (height + 20)),
                    ForeColor = Color.White,
                    Size = new System.Drawing.Size(855, height)
                };

                System.Windows.Forms.Label label = new System.Windows.Forms.Label
                {
                    Text = $"Label {i + 1}: {GenerateRandomText(random)}", // 随机内容
                    AutoSize = true, // 自动调整大小
                    ForeColor = Color.White,
                    Location = new Point(5, 20),
                    Margin = new Padding(0, 0, 0, 10) // 下边距 10px

                };
                tabin.Controls.Add(group);
                group.Controls.Add(label);

            }
        }

        private static string GenerateRandomText(Random random)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int length = random.Next(5, 16); // 随机长度 5-15
            char[] text = new char[length];
            for (int i = 0; i < length; i++)
            {
                text[i] = chars[random.Next(chars.Length)];
            }
            return new string(text);
        }
    }
}
