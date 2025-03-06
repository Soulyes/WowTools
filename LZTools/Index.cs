using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReaLTaiizor.Colors;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Util;
using ReaLTaiizor.Manager;
using System.Threading;
using System.Media;
using Newtonsoft.Json;
using System.Net.Http;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.IO;
using ReaLTaiizor.Controls;
using Microsoft.Win32;
using System.Web.UI.WebControls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Security;
using System.Security.Policy;
//using OpenCvSharp;

namespace LZTools
{
    public partial class Index : PoisonForm
    {
        private readonly MaterialSkinManager MM;
        emunClass LzEmun = new emunClass();
        KeyDo LzKeysDo = new KeyDo();
        Thread CoreTaskOnly;
        Thread GuajiTaskOnly;
        List<String> KeyTList = new List<string>();
        string ver = "110";

        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        const int WM_KEYDOWN = 0x0100; //按下按键
        const int WM_KEYUP = 0x101; //弹起按键 

        public Index()
        {
            InitializeComponent();
            MM = MaterialSkinManager.Instance;
            MM.EnforceBackcolorOnAllComponents = true;
            MM.Theme = MaterialSkinManager.Themes.DARK;
            MM.ColorScheme = new(MaterialPrimary.Grey900, MaterialPrimary.Grey700, MaterialPrimary.Grey500, MaterialAccent.Orange400, MaterialTextShade.WHITE);

            Control.CheckForIllegalCrossThreadCalls = false;
            this.Shown += StartTip;
            this.Resize += SizeChange;
        }

        

        private void StartTip(object sender, EventArgs e)
        {
            // 模拟异步加载
            //await Task.Delay(2000); // 假设加载需要 2 秒

            // 在异步加载完成后调用 ToTip
            //ToTip("欢迎使用老钟的魔兽小工具\r\n当前版本:" + ver,300,75,5000);
            ToTip("欢迎使用老钟的魔兽小工具\r\n当前版本: " + ver,300,70,5000);
            //ToTip("当前版本:" + ver);

            DLLLinkPath.Text = LzKeysDo.GetIni("DLL", "path", DLLLinkPath.Text, System.Environment.CurrentDirectory + @"\Runlist.ini"); 

            //LZDownlaod.CheckDownload();
            //开始检查更新
            toolTip1.SetToolTip(ChooseWOW, "点击对搜索所有魔兽世界进程\r\n再次点击即可清空之前的内容\r\n搜索到的进程会在左上角加上进程ID！\n鼠标点击下框的进程，再点击具体的操作框按钮即可加入。");
            toolTip1.SetToolTip(ProcessList, "鼠标点击框内的进程，再点击具体的操作框按钮即可加入。\r\n双击即可注入dll进程，用于IM等dll注入魔兽进程！\r\n本功能慎用，属于违背魔兽世界条款！");
            toolTip1.SetToolTip(GuajiIn,"点击会将进程框选择的ID加入到挂机进程内！\r\n记得点击配置，制作挂机命令顺序\r\n双击则是删除");
            toolTip1.SetToolTip(GuaList, "双击进程ID删除");
            toolTip1.SetToolTip(TongBuin, "点击会将进程框选择的ID加入到同步按键进程内！\r\n记得点击配置勾选要同步按键\r\n双击则是删除");
            toolTip1.SetToolTip(TongBuList, "双击进程ID删除");
            toolTip1.SetToolTip(StartWorking, "点击开始执行挂机命令\r\n所有挂机框内进程ID都会执行！");
            toolTip1.SetToolTip(StratTB, "点击开始执行同步命令\r\n所有同步框内ID都会执行！");
            toolTip1.SetToolTip(BattonChange, "输入名字，同时选择战网进程ID\r\n再点击修改战网备注即可修改！");

            CheckAll();

            //new ToastNotification("老钟小工具", "欢迎使用老钟魔兽小工具\r\n如果有任何疑问请点击kook或者QQ联系老钟", 3000);
        }


        private async void CheckAll()
        {

            StartCheck.Text = "初始化ing";
            StartCheck.ForeColor = Color.Red;
            try
            {
                StartUpdate.Visible = false;
                if (LZDownlaod.CheckData(this, "105"))
                {
                    StartUpdate.Visible = true;
                }

                string uri = await Task.Run( () => LZDownlaod.GetWeb());
                if (LZClass.CheckWinVer() >= 10) guanggao.Source = new Uri(uri);
                else LZDownlaod.OpenUrlInDefaultBrowser(uri);

                uri = "http://wd.wowlz.com/top.php";
                if (LZClass.CheckWinVer() >= 10) topWeb.Source = new Uri(uri);
                else LZDownlaod.OpenUrlInDefaultBrowser(uri);
                LZClass.DrawGroupBox(tabHelp);

            }
            catch { }


            StartCheck.Text = "";
        }

        int TimeStep = 0;
        bool ThreadRun = false;
        Thread TimeStepThread;

        private void Form1_Load(object sender, EventArgs e)
        {
            //读取同步按键
            LzKeysDo.ReadMKeySetting(this);
            //读取挂机按键顺序
            AutoRunList.Text = LzKeysDo.GetIni("RunList", "AutoRunList", "", System.Environment.CurrentDirectory + @"\Runlist.ini");
            //SayList.Text = LzKeysDo.GetIni("RunList", "AutoSayList", "", System.Environment.CurrentDirectory + @"\Runlist.ini");
            TransToCN();

            //同步按键写入内存列表
            LzKeysDo.KeyTListin(KeyTList, this);
            //SayList.Text = LzKeysDo.GetSay(System.Environment.CurrentDirectory + @"\Saylist.ini");


        }

    

        private void ButtonJiShiQI_Click(object sender, EventArgs e)
        {

            if(LZClass.GetRunSt()) TimeStepThread.Abort();
            //try { TimeStepThread.Abort(); } catch { }

            ThreadStart ThreadCk = () => LZClass.reftime(TimeSet.Text,TimeTag);
            TimeStepThread = new Thread(ThreadCk);
            TimeStepThread.Start();
        }

        // DeepSeek API 的端点
        private static readonly string apiUrl = "https://api.deepseek.com/v1/chat/completions";

        // 你的 DeepSeek API 密钥
        private static readonly string apiKey = "sk-2423d7cd443947a396bae6ff0766fb68";

        private void AiUpdateButton_Click(object sender, EventArgs e)
        {
            new ToastNotification("老钟小工具", "欢迎使用老钟魔兽小工具\r\n如果有任何疑问请点击kook或者QQ联系老钟", 3000);
            //await Task.Run();
            /*
            // 获取用户输入
            string userInput = AiInputBox.Text;

            // 如果输入为空，提示用户
            if (string.IsNullOrEmpty(userInput))
            {
                MessageBox.Show("请输入对话内容！");
                return;
            }

            AiUpdateButton.Text = "查询中";
            AiUpdateButton.Enabled = false;
            AiInputBox.Enabled = false;
            // 设置对话内容
            var messages = new[]
            {
                new { role = "system", content = "You are a helpful assistant." },
                new { role = "user", content = userInput }
            };

            // 调用 DeepSeek API 并获取响应
            try
            {
                string response = await CallDeepSeekApiAsync(messages);

                // 解析响应并显示结果
                var deepSeekResponse = JsonConvert.DeserializeObject<DeepSeekResponse>(response);
                string assistantReply = deepSeekResponse.Choices[0].Message.Content;
                AiOutBox.Text = assistantReply;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生错误：{ex.Message}");
            }

            AiUpdateButton.Text = "点击查询";
            AiUpdateButton.Enabled = true;
            AiInputBox.Enabled = true;
            */
        }

        private async Task<string> CallDeepSeekApiAsync(object[] messages)
        {
            using (HttpClient client = new HttpClient())
            {
                // 设置请求头
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // 构造请求体
                var requestBody = new
                {
                    model = "deepseek-chat", // 使用的模型（根据 DeepSeek 文档填写）
                    messages = messages
                };

                // 将请求体序列化为 JSON
                string jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // 发送 POST 请求
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                // 确保请求成功
                response.EnsureSuccessStatusCode();

                // 读取响应内容
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }

        // 定义类来映射 DeepSeek API 响应
        public class DeepSeekResponse
        {
            public string Id { get; set; }
            public string Object { get; set; }
            public long Created { get; set; }
            public Choice[] Choices { get; set; }
            public Usage Usage { get; set; }
        }

        public class Choice
        {
            public int Index { get; set; }
            public Message Message { get; set; }
            public string Finish_Reason { get; set; }
        }

        public class Message
        {
            public string Role { get; set; }
            public string Content { get; set; }
        }

        public class Usage
        {
            public int Prompt_Tokens { get; set; }
            public int Completion_Tokens { get; set; }
            public int Total_Tokens { get; set; }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {

            ThreadStart ThreadCk = () => CheckProcess();
            Thread DoCheck = new Thread(ThreadCk);
            DoCheck.Start();
        }

        /*
 Windows API 函数调用区
*/
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, string lParam);

        private void ToTip(string a,int b = 300,int c = 50,int d = 2000)
        {
            ThreadStart TipNext = () => Tip.Show(a, b, c, d, this.Location.X + this.Size.Width / 2, this.Location.Y + this.Height);
            Thread DoTip = new Thread(TipNext);
            DoTip.Start();
            //Tip.Show(a, b, c, d, this.Location.X + this.Size.Width / 2, this.Location.Y + this.Height);
        }

        private void CheckProcess()
        {
            ChooseWOW.Enabled = false;
            // 检查所有进城中包含“魔兽世界”的title，并打标
            Process[] ps = Process.GetProcesses();

            ProcessList.Items.Clear();
            AutoFishListBox.Items.Clear();

            foreach (Process p in ps)
            {
                try
                {
                    if (p.MainWindowTitle.Contains("魔兽世界") && Path.GetFileName(p.MainModule.FileName.ToLower()).Contains("wow"))
                    {
                        //&& p.MainModule.FileName.ToLower().Contains("wow")
                        //MessageBox.Show(Path.GetFileName(p.MainModule.FileName.ToLower()));
                        //INFOout.Text = INFOout + p.MainModule.FileName.ToString() + "\n";
                        //ProcessList.Items.Add(p.Id, true);
                        ProcessList.Items.Add(p.Id);
                        SendMessage(p.MainWindowHandle, 0x0c, 0, "魔兽世界 -" + p.Id.ToString());
                    }

                }
                catch (Exception Ex) { ToTip(Ex.ToString(),300,50,2000); }
            }

            ChooseWOW.Enabled = true;
            if (ProcessList.Items.Count == 0) ToTip("没有找到魔兽世界进程！");

        }

        private void GuajiIn_Click(object sender, EventArgs e)
        {
            if (ProcessList.SelectedItem != null)
            {
                bool canin = true;
                foreach (var item in GuaList.Items)
                {
                    if (item.ToString() == ProcessList.SelectedItem.ToString()) canin = false;
                }

                for (int i = TongBuList.Items.Count - 1; i >= 0; i--)
                {
                    if (TongBuList.Items[i].ToString() == ProcessList.SelectedItem.ToString()) TongBuList.Items.RemoveAt(i);
                }
                if (canin == true) GuaList.Items.Add(ProcessList.SelectedItem.ToString());
            }
            else ToTip("请选择要加入挂机的进程！");

        }

        private void GuaList_DoubleClick(object sender, EventArgs e)
        {
            //GuaList.SelectedItem
            GuaList.Items.Remove(GuaList.SelectedItem);
        }

        private void TongBuin_Click(object sender, EventArgs e)
        {
            if (ProcessList.SelectedItem != null)
            {
                bool canin = true;
                foreach (var item in TongBuList.Items)
                {
                    if (item.ToString() == ProcessList.SelectedItem.ToString()) canin = false;
                }

                for (int i = GuaList.Items.Count - 1; i >= 0; i--)
                {
                    if (GuaList.Items[i].ToString() == ProcessList.SelectedItem.ToString()) GuaList.Items.RemoveAt(i);
                }
                if (canin == true) TongBuList.Items.Add(ProcessList.SelectedItem.ToString());
            }
            else ToTip("请选择要加入同步的进程！");
        }

        private void TongBuList_DoubleClick(object sender, EventArgs e)
        {
            TongBuList.Items.Remove(TongBuList.SelectedItem);
        }
        // 开始测试抓取屏幕的text文字
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private static uint targetProcessId = 1203;

        
        private static IntPtr targetWindowHandle = IntPtr.Zero;

        // 枚举窗口的回调函数
        private static bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam)
        {
            // 获取窗口的进程 ID
            uint processId;
            GetWindowThreadProcessId(hWnd, out processId);

            // 如果进程 ID 匹配，则保存窗口句柄
            if (processId == targetProcessId)
            {
                targetWindowHandle = hWnd;
                return false; // 停止枚举
            }

            return true; // 继续枚举
        }

        private void AutoFishJoin_Click(object sender, EventArgs e)
        {
            //tabControl1.SelectedTab = tabPage2;  // 切换到 tabpage2
            //IndexTab.SelectedTab = tabFish;
            // 检查所有进城中包含“魔兽世界”的title，并打标
            Process[] ps = Process.GetProcesses();

            AutoFishListBox.Items.Clear();

            foreach (Process p in ps)
            {
                try
                {
                    //if (p.MainWindowTitle.Contains("战网"))
                    if(p.ProcessName.Contains("Battle.net") && p.MainWindowHandle.ToString() != "0")
                    {
                        //&& p.MainModule.FileName.ToLower().Contains("wow")
                        //MessageBox.Show(Path.GetFileName(p.MainModule.FileName.ToLower()));
                        //INFOout.Text = INFOout + p.MainModule.FileName.ToString() + "\n";
                        //ProcessList.Items.Add(p.Id, true);
                        AutoFishListBox.Items.Add(p.Id);
                        if(p.MainWindowTitle.Contains("战网")) SendMessage(p.MainWindowHandle, 0x0c, 0, "战网 - " + p.Id.ToString());
                    }

                }
                catch (Exception Ex) { ToTip(Ex.ToString(), 300, 50, 2000); }
            }

            if (AutoFishListBox.Items.Count == 0) ToTip("没有战网进程或已经都改名");
        }

        private void AutoFishListBox_DoubleClick(object sender, EventArgs e)
        {
            AutoFishListBox.Items.Remove(AutoFishListBox.SelectedItem);
        }

        private void ToSetting_Click(object sender, EventArgs e)
        {
            IndexTab.SelectedTab = tabPeizhi;
        }

        private void toSetting2_Click(object sender, EventArgs e)
        {
            IndexTab.SelectedTab = tabPeizhi;
        }

        private void InputKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            InputKey.Text = "";
            InputKey.Text = e.KeyChar.ToString();
            e.Handled = true;
        }

        private void lostButton1_Click(object sender, EventArgs e)
        {
            if (InputKey.Text != "" && KeyDelayTime.Text != "")
            {

                if (AutoRunList.Text != "") AutoRunList.Text += (",");
                AutoRunList.Text +=(LzEmun.GetCharValue(InputKey.Text) + "," + KeyDelayTime.Text);

                TransToCN();
            }
            InputKey.Focus();
        }

        private void TransToCN()
        {
            string[] ListStr = AutoRunList.Text.Split(',');
            bool a = true;
            AutoRunListCN.Text = "";
            if (ListStr.Length > 1)
            {
                
                AutoRunListCN.Text = AutoRunListCN.Text + ("顺序： \r\n");
                foreach (string str in ListStr)
                {
                    if (a)
                    {
                        if (str == "32")
                        {
                            AutoRunListCN.Text += (" 键<空格>");
                        }
                        else
                            AutoRunListCN.Text +=(" 键<" + LzEmun.GetCharKey(int.Parse(str)) + ">");

                        a = false;
                    }
                    else
                    {
                        AutoRunListCN.Text += ("<" + str + ">毫秒 →");
                        a = true;
                    }
                }
                AutoRunListCN.Text = AutoRunListCN.Text.TrimEnd('→');
            }
        }

        private void lostButton2_Click(object sender, EventArgs e)
        {
            if (InputKey.Text != "" && KeyDelayTime.Text != "")
            {
                if (AutoRunList.Text != "") AutoRunList.Text +=(",");
                AutoRunList.Text +=(LzEmun.GetCharValue(InputKey.Text) + "," + KeyDelayTime.Text + "-" + KeyDelayTimeMax.Text);

                TransToCN();
            }
            InputKey.Focus();
        }

        private void JoinAutoSay_Click(object sender, EventArgs e)
        {
            string[] ListStr = AutoRunList.Text.Split(',');
            string newList = "";
            for (int i = 0; i < ListStr.Length - 2; i++)
            {
                newList += ListStr[i] + ",";
            }
            newList = newList.TrimEnd(',');
            AutoRunList.Text = newList;
            TransToCN();
            InputKey.Focus();
        }

        private void SaveAutoRunList_Click(object sender, EventArgs e)
        {
            InputKey.Focus();
            LzKeysDo.SaveIni("RunList", "AutoRunList", AutoRunList.Text, System.Environment.CurrentDirectory + @"\Runlist.ini");
        }

        private void SaveMKey_Click(object sender, EventArgs e)
        {
            LzKeysDo.SaveMKeySetting(this);
            LzKeysDo.KeyTListin(KeyTList, this); //从写要同步的按键（实时更新）
        }

        private void StartWorking_Click(object sender, EventArgs e)
        {
            if (AutoRunList.Text.Length > 0)
            {
                if (StartWorking.Text == "启动挂机")
                {
                    ThreadStart CoreTask = () => StartGuaji();
                    CoreTaskOnly = new Thread(CoreTask);
                    //CoreTaskOnly.IsBackground = true;
                    CoreTaskOnly.Start();
                    StartWorking.Text = "停止挂机";
                    GuajiIn.Enabled = false;
                    ToTip("自动挂机已经启动！");
                }
                else
                {
                    StartWorking.Text = "启动挂机";
                    CoreTaskOnly.Abort();
                    INFOout.Text +=("\r\n");

                    GuajiIn.Enabled = true;
                    ToTip("自动挂机已经关闭！");
                }
            }
            else
            {
                ToTip("请录入挂机指令！");
                //MaterialSnackBar SendMessage = new MaterialSnackBar("请录入挂机指令！", 1000);
                //SendMessage.Show(this);
                //MainContol.SelectedIndex = 0;
                InputKey.Focus();
            }
        }

        public void StartGuaji(bool stat = true)
        {
            while (true)
            {
                //if(stat == false) GuajiTaskOnly.Abort();
                try
                {
                    
                    if (INFOout.Text.Length >= 15000) { INFOout.Text = ""; INFOout.Text = "历史记录已清理\r\n"; }
                    LzKeysDo.DoKeyNext(AutoRunList.Text, INFOout, GuaList);
                }
                catch (Exception ex)
                {
                    //INFOout.Text +=(ex.ToString() + "\r\n");
                }
            }
        }

        private void StratTB_Click(object sender, EventArgs e)
        {
            if (StratTB.Text == "启动同步")
            {
                startSync();
                ToTip("按键同步已经启动！");
            }
            else if (StratTB.Text == "关闭同步")
            {
                stopSync();
                ToTip("按键同步已经关闭！");
            }
        }

        private KeyboardHook k_hook = new KeyboardHook();
        private KeyEventHandler myKeyEventHandeler = null;//按键钩子

        public void startSync()
        {
            myKeyEventHandeler = new KeyEventHandler(hook_KeyDown);
            k_hook.KeyDownEvent += myKeyEventHandeler;//钩住键按下
            //myKeyEventHandeler = new KeyEventHandler(hook_KeyUp);
            //k_hook.KeyUpEvent += myKeyEventHandeler;//钩住松按键
            k_hook.Start();//安装键盘钩子
            StratTB.Text = "关闭同步";
        }
        public void stopSync()
        {
            if (myKeyEventHandeler != null)
            {
                //myKeyEventHandeler = new KeyEventHandler(hook_KeyDown);
                k_hook.KeyDownEvent -= myKeyEventHandeler;//钩住键按下
                //myKeyEventHandeler = new KeyEventHandler(hook_KeyUp);
                //k_hook.KeyDownEvent -= myKeyEventHandeler;//钩住松按键
                myKeyEventHandeler = null;
                k_hook.Stop();//安装键盘钩子
            }
            StratTB.Text = "启动同步";
        }

        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            //if (KeyTList.Contains(e.KeyCode.ToString()) == true) INFOout.AppendText(e.KeyCode.ToString() + " 在包含范围内\r\n");

            //if (((e.KeyValue >= 48 && e.KeyValue <= 57) || (e.KeyValue >= 112 && e.KeyValue <= 123) || e.KeyValue == (int)Keys.Tab || e.KeyValue == (int)Keys.Space) ||  e.KeyValue == (int)Keys.C || e.KeyValue == (int)Keys.E)
            
            if (KeyTList.Contains(e.KeyCode.ToString()) == true)
            {
                
                //INFOout.AppendText(e.KeyCode.ToString() + " 在包含范围内\r\n");
                //启动项勾选的进程发送按键的命令
                bool FindProcess = false;
                Process p = null;
                for (int i = 0; i < TongBuList.Items.Count; i++)
                {
                    //读取要同步的id
                        FindProcess = false;
                        try
                        {
                            p = Process.GetProcessById(Convert.ToInt32(TongBuList.Items[i].ToString()));
                            FindProcess = true;
                        }
                        catch
                        {
                            INFOout.Text +=(TongBuList.Items[i].ToString() + " 进程丢失！\r\n");
                            TongBuList.Items.RemoveAt(i);
                            i--;
                        }
                    if (FindProcess == true)
                    {
                        PostMessage(p.MainWindowHandle, WM_KEYDOWN, e.KeyValue, 0);
                        PostMessage(p.MainWindowHandle, WM_KEYUP, e.KeyValue, 0);
                        //INFOout.Text += " 同步按键：" + e.KeyCode.ToString() + " values " + e.KeyValue.ToString() + "\r\n";
                    }

                }

            }
        }

        private void KOOKlink_Click(object sender, EventArgs e)
        {
            LZDownlaod.OpenUrlInDefaultBrowser("https://kook.top/Q45pFx");
        }

        private void QQJoin_Click(object sender, EventArgs e)
        {
            LZDownlaod.OpenUrlInDefaultBrowser("https://qm.qq.com/q/7u80rGb53G");
        }

        private void LzIcon_DoubleClick(object sender, EventArgs e)
        {

            if (this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;
            else if(this.WindowState == FormWindowState.Normal) this.WindowState = FormWindowState.Minimized;
        }


        private void SizeChange(object sender, EventArgs e)
        {
            
            if (this.WindowState == FormWindowState.Minimized)
            {
                //ToTip("最小化");
                ShowInTaskbar = false;
            }
            else ShowInTaskbar = true;
            

        }

        private void StartUpdate_Click(object sender, EventArgs e)
        {
            LZDownlaod.UpdateMain();
        }

        private void StartFish_Click(object sender, EventArgs e)
        {
            ToTip("功能开发中，敬请期待！");
        }


        
        private void ZWSearch_Click(object sender, EventArgs e)
        {
            LZClass.ZWsearch(tabZW,ZwList);
        }

        private void ProcessList_DoubleClick(object sender, EventArgs e)
        {
            //注入 dll
            if(ProcessList.SelectedItem != null && DLLLinkPath.Text != "")
            {
                try { 
                    bool a = LZInject.inDll(Convert.ToInt16(ProcessList.SelectedItem.ToString()), DLLLinkPath.Text);
                    if (a) ToTip(ProcessList.SelectedItem.ToString() + "注入成功");
                    else ToTip(ProcessList.SelectedItem.ToString() + "注入成功");
                }
                catch { ToTip(ProcessList.SelectedItem.ToString() + " 注入失败"); }
            }
            
        }

        private void DLLLinkPath_Click(object sender, EventArgs e)
        {
            OpenDllLink.Filter = "所有文件 (*.*)|*.dll";
            OpenDllLink.RestoreDirectory = true;

                string currentPath = DLLLinkPath.Text;
                if (File.Exists(currentPath))
                {
                    OpenDllLink.InitialDirectory = Path.GetDirectoryName(currentPath);
                }
                // 如果路径是目录，则直接使用
                else if (Directory.Exists(currentPath))
                {
                    OpenDllLink.InitialDirectory = currentPath;
                }
                // 如果路径无效，则使用 C:\
                else
                {
                    OpenDllLink.InitialDirectory = @"C:\";
                }


                if (OpenDllLink.ShowDialog() == DialogResult.OK)
                {
                    // 将选择的文件路径更新到 TextBox 中
                    DLLLinkPath.Text = OpenDllLink.FileName;
                    LzKeysDo.SaveIni("DLL", "path", DLLLinkPath.Text, System.Environment.CurrentDirectory + @"\Runlist.ini");
                 }

            

                
        }


        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void AutoFishListBox_DoubleClick_1(object sender, EventArgs e)
        {
            DoChangeID.Text = AutoFishListBox.SelectedItem.ToString();
        }

        private void BattonChange_Click(object sender, EventArgs e)
        {
            if (DoChangeID.Text != null)
            {
                int a = Convert.ToInt32(DoChangeID.Text);
                Process p = Process.GetProcessById(a);
                SendMessage(p.MainWindowHandle, 0x0c, 0, BattleName.Text + " - " + p.Id.ToString());
            }
            else ToTip("没有选择任何进程！");

        }

        private void foreverButton1_Click(object sender, EventArgs e)
        {
            ImageFinder aaa = new ImageFinder(29304, "d1.jpg", 500);
            ToTip("开始搜索");
            aaa.Start();

        }

        private void ShowAllP_Click(object sender, EventArgs e)
        {
            LZClass.ShowCloseAll(true);
        }

        private void HiddenAllP_Click(object sender, EventArgs e)
        {
            LZClass.ShowCloseAll(false);
        }
    }
}
