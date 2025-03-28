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
using OpenCvSharp.XPhoto;
using OpenCvSharp;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace LZTools
{
    public partial class Index : PoisonForm
    {
        private readonly MaterialSkinManager MM;
        emunClass LzEmun = new emunClass();
        KeyDo LzKeysDo = new KeyDo();
        Thread CoreTaskOnly;
        Thread GuajiTaskOnly;
        Thread DiaoFish;
        List<String> KeyTList = new List<string>();

        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        const int WM_KEYDOWN = 0x0100; //按下按键
        const int WM_KEYUP = 0x101; //弹起按键 
        System.Windows.Forms.Timer tenMin = new System.Windows.Forms.Timer();

        public Index()
        {
            InitializeComponent();
            MM = MaterialSkinManager.Instance;
            MM.EnforceBackcolorOnAllComponents = true;
            MM.Theme = MaterialSkinManager.Themes.DARK;
            MM.ColorScheme = new(MaterialPrimary.Grey900, MaterialPrimary.Grey700, MaterialPrimary.Grey500, MaterialAccent.Orange400, MaterialTextShade.WHITE);

            Control.CheckForIllegalCrossThreadCalls = false;
            this.Shown += StartTip;
            this.FormClosing += CloseIndex;
            //this.Resize += SizeChange;

            //SearchIcon
            SearchMenu.Items.Add("加入同步", null, (sender, e) => Tongbuin());
            SearchMenu.Items.Add("加入挂机", null, (sender, e) => Guajiin());
            SearchMenu.Items.Add("----------");
            SearchMenu.Items.Add("重新搜索", null, (sender, e) => CheckProcess());
            ProcessList.MouseClick += OnThisRightClick;

            openweb();

            tenMin = new System.Windows.Forms.Timer();
            tenMin.Interval = 600000; // 10 分钟的毫秒数
            tenMin.Tick += new EventHandler(OnTenMinTimerTick);
            tenMin.Start(); // 启动 Timer

        }

        private void OnTenMinTimerTick(object sender, EventArgs e)
        {
            openweb(); // 每 10 分钟调用一次 opencheck()
        }

        private void OnThisRightClick(object sender, MouseEventArgs e)
        {
            
            if(ProcessList.SelectedItem != null) 
            { 
                if(SearchMenu.Items.Count == 5)
                {
                    SearchMenu.Items.RemoveAt(4);
                }
                SearchMenu.Items.Add(@"DLL注入： " + ProcessList.Text ,null,(sender,e) => Toinj());
                SearchMenu.Show(Cursor.Position); 
            }
            
        }

        

        private async void StartTip(object sender, EventArgs e)
        {
            // 模拟异步加载
            //await Task.Delay(2000); // 假设加载需要 2 秒

            // 在异步加载完成后调用 ToTip
            //ToTip("欢迎使用老钟的魔兽小工具\r\n当前版本:" + ver,300,75,5000);
            ToTip("欢迎使用老钟的魔兽小工具\r\n当前版本: " + VER.FromVer(),300,70,5000);
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
            DoShouNa();
        }

        //注册快捷键

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // 定义常量



        // 重写 WndProc 方法以处理窗口消息
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);
            if (KeySwitch.Checked == false)
            {             // 检查是否是热键消息
                if (m.Msg == WM_HOTKEY)
                {
                    int id = m.WParam.ToInt32();
                    switch (id)
                    {
                        case HOTKEY_ID_ALT_1:
                            SW();
                            break;
                        case HOTKEY_ID_ALT_2:
                            TB();
                            break;
                        case HOTKEY_ID_ALT_3:
                            {
                                int Foucid = LZClass.GetProcess();
                                if (Foucid != 0)
                                {
                                    try
                                    {
                                        bool a = LZInject.inDll(Foucid, DLLLinkPath.Text);
                                        if (a) ToTip(Foucid.ToString() + "注入成功");
                                        else ToTip(Foucid.ToString() + "注入成功");
                                    }
                                    catch { ToTip(Foucid.ToString() + " 注入失败"); }
                                }
                                else { ToTip(Foucid.ToString() + " 注入失败"); }
                                break;
                            }
                        case HOTKEY_ID_ALT_4:
                            {
                                if (this.Visible) this.Hide();
                                else this.Show();
                                break;
                            }
                            
                    }
                }
            }

        }

        private const int WM_HOTKEY = 0x0312; // 热键消息
        private const int HOTKEY_ID_ALT_1 = 1; // Alt+1 的唯一标识符
        private const int HOTKEY_ID_ALT_2 = 2; // Alt+2 的唯一标识符
        private const int HOTKEY_ID_ALT_3 = 3; // ALT+CTRL+Q 的唯一标识符
        private const int HOTKEY_ID_ALT_4 = 4; // ALT+CTRL+Q 的唯一标识符
        private const int HOTKEY_ID_ALT_5 = 5; // ALT+CTRL+Q 的唯一标识符
        private const int MOD_CRTL = 0x0002; // 0001 alt 0002 crtl
        private const int MOD_ALT = 0x0001; // 0001 alt 0002 crtl
        private const int VK_1 = 0x31; // 1 键的虚拟键码
        private const int VK_2 = 0x32; // 2 键的虚拟键码
        private const int VK_S = 0x53; // 2 键的虚拟键码
        private const int VK_X = 0x58; // 2 键的虚拟键码
        private const int VK_Q = 0x51; // 2 键的虚拟键码

        // 在窗体加载时注册热键
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            TBKey.Text = LzKeysDo.GetIni("ModKey", "TB", "", System.Environment.CurrentDirectory + @"\Runlist.ini");
            if (TBKey.Text == "") TBKey.Text = "CTRL+ALT+X";
            GJKey.Text = LzKeysDo.GetIni("ModKey", "GJ", "", System.Environment.CurrentDirectory + @"\Runlist.ini");
            if (GJKey.Text == "") GJKey.Text = "CTRL+ALT+S";
            InjectKey.Text = LzKeysDo.GetIni("ModKey", "Inject", "", System.Environment.CurrentDirectory + @"\Runlist.ini");
            if (InjectKey.Text == "") InjectKey.Text = "CTRL+ALT+Q";
            HiddenKey.Text = LzKeysDo.GetIni("ModKey", "Hidden", "", System.Environment.CurrentDirectory + @"\Runlist.ini");
            if (HiddenKey.Text == "") HiddenKey.Text = "CTRL+ALT+D1";

            RegKey();
            //如果要ALT+CRTL RegisterHotKey(this.Handle, HOTKEY_ID_ALT_2, MOD_CRTL | MOD_ALT, VK_2); // 注册 Alt+2
        }


        private void CheckAll()
        {

            StartCheck.Text = "系统初始化";
            StartCheck.ForeColor = Color.Red;
            try
            {
                StartUpdate.Visible = false;
                if (LZDownlaod.CheckData(this, VER.FromVer()))
                {
                    StartUpdate.Visible = true;
                }
                // 更新说明
                LZClass.DrawGroupBox(tabHelp);

            }
            catch { }


            StartCheck.Text = "";
        }

        private async void openweb()
        {
            string uri = await Task.Run(() => LZDownlaod.GetWeb());
            if (LZClass.CheckWinVer() >= 10) guanggao.Source = new Uri(uri);
            else LZDownlaod.OpenUrlInDefaultBrowser(uri);

            uri = "http://wd.wowlz.com/top.php";
            if (LZClass.CheckWinVer() >= 10) topWeb.Source = new Uri(uri);
            else LZDownlaod.OpenUrlInDefaultBrowser(uri);

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

            BindRightClickEventToControls(this);
            IndexTab.MouseClick += OnFormMouseClick;
        }

        // 递归遍历控件并绑定事件
        private void BindRightClickEventToControls(Control parentControl)
        {
            foreach (Control control in parentControl.Controls)
            {
                // 如果是 TabPage 或 GroupBox，绑定事件
                if (control is System.Windows.Forms.TabPage || control is System.Windows.Forms.GroupBox)
                {
                    control.MouseClick += OnFormMouseClick;
                }

                // 如果控件包含子控件，递归遍历
                if (control.HasChildren)
                {
                    BindRightClickEventToControls(control);
                }
            }
        }

        private void OnFormMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // 显示 NotifyIcon 的右键菜单
                ZwList.Show(Cursor.Position);
            }
        }


        private void ButtonJiShiQI_Click(object sender, EventArgs e)
        {

            if(LZClass.GetRunSt()) TimeStepThread.Abort();
            //try { TimeStepThread.Abort(); } catch { }

            ThreadStart ThreadCk = () => LZClass.reftime(TimeSet.Text,TimeTag, LzIcon);
            TimeStepThread = new Thread(ThreadCk);
            TimeStepThread.Start();
        }

        // DeepSeek API 的端点
        private static readonly string apiUrl = "https://api.deepseek.com/v1/chat/completions";

        // 你的 DeepSeek API 密钥
        private static readonly string apiKey = "";

        private void AiUpdateButton_Click(object sender, EventArgs e)
        {
            new ToastNotification("老钟小工具", "欢迎使用老钟魔兽小工具\r\n如果有任何疑问请点击kook或者QQ联系老钟", 3000);
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
            CheckProcess();
        }

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, string lParam);

        private void ToTip(string a,int b = 300,int c = 50,int d = 2000)
        {
            ThreadStart TipNext = () => Tip.Show(a, b, c, d, this.Location.X + this.Size.Width / 2, this.Location.Y + this.Height);
            Thread DoTip = new Thread(TipNext);
            DoTip.Start();
            //Tip.Show(a, b, c, d, this.Location.X + this.Size.Width / 2, this.Location.Y + this.Height);
        }

        public void CheckProcess()
        {
            //try { ProcessList.Items.Clear(); AutoFishListBox.Items.Clear(); }
            //catch(Exception ex) { MessageBox.Show(ex.ToString()); }
            dungeonLabel1.Focus();
            Thread.Sleep(100);

            ProcessList.Items.Clear();
            

            ChooseWOW.Enabled = false;
            // 检查所有进城中包含“魔兽世界”的title，并打标
            Process[] ps = Process.GetProcesses();
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
            Guajiin();
        }
        private void Guajiin()
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
            Tongbuin();
        }

        private void Tongbuin()
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



        private void SaveMKey_Click(object sender, EventArgs e)
        {
            LzKeysDo.SaveMKeySetting(this);
            LzKeysDo.KeyTListin(KeyTList, this); //从写要同步的按键（实时更新）
        }

        private void StartWorking_Click(object sender, EventArgs e)
        {
            if (GuaList.Items.Count > 0) SW();
            else ToTip("请先添加需要挂机的进程");
        }

        private void SW()
        {
            if (AutoRunList.Text.Length > 0 && GuaList.Items.Count > 0)
            {
                if (StartWorking.Text == "启动挂机")
                {
                    ThreadStart CoreTask = () => StartGuaji();
                    CoreTaskOnly = new Thread(CoreTask);
                    //CoreTaskOnly.IsBackground = true;
                    CoreTaskOnly.Start();
                    StartWorking.Text = "停止挂机";
                    GuajiIn.Enabled = false;
                    INFOout.Text += (DateTime.Now.ToString() + ": 开始挂机按键！\r\n") ;
                    if (this.Visible) ToTip("自动挂机已经启动！");
                    else LZClass.LeftDownMessage(LzIcon, "老钟魔兽", "自动挂机已经启动");
                }
                else
                {
                    StartWorking.Text = "启动挂机";
                    CoreTaskOnly.Abort();
                    //INFOout.Text += ("\r\n");
                    INFOout.Text += (DateTime.Now.ToString() + ": 结束挂机按键！\r\n");
                    GuajiIn.Enabled = true;
                    if (this.Visible) ToTip("自动挂机已经关闭！");
                    else LZClass.LeftDownMessage(LzIcon, "老钟魔兽", "自动挂机已经关闭");
                }
            }
            else
            {
                ToTip("没有挂机指令或者没有同步的进程！");
                //MaterialSnackBar SendMessage = new MaterialSnackBar("请录入挂机指令！", 1000);
                //SendMessage.Show(this);
                //MainContol.SelectedIndex = 0;
                InputKey.Focus();
            }

            StartWorking.Refresh();
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
            if (TongBuList.Items.Count > 0) TB();
            else ToTip("请先添加需要同步的进程!");
        }

        private void TB()
        {
            if (TongBuList.Items.Count > 0)
            {
                if (StratTB.Text == "启动同步")
                {
                    startSync();
                    if (this.Visible) ToTip("按键同步已经启动！");
                    else LZClass.LeftDownMessage(LzIcon, "老钟魔兽", "按键同步已经启动");

                }
                else if (StratTB.Text == "关闭同步")
                {
                    stopSync();
                    if (this.Visible) ToTip("按键同步已经关闭！");
                    else LZClass.LeftDownMessage(LzIcon, "老钟魔兽", "按键同步已经关闭");
                }
                StratTB.Refresh();
            }
            else ToTip("请先添加需要同步的进程!");
        }
        private KeyboardHook k_hook = new KeyboardHook();
        private KeyEventHandler myKeyEventHandeler = null;//按键钩子

        public void startSync()
        {
            myKeyEventHandeler = new KeyEventHandler(hook_KeyDown);
            k_hook.KeyDownEvent += myKeyEventHandeler;//钩住键按下
            myKeyEventHandeler = new KeyEventHandler(hook_KeyUP);
            k_hook.KeyUpEvent += myKeyEventHandeler;//钩住松按键
            k_hook.Start();//安装键盘钩子
            StratTB.Text = "关闭同步";
        }
        public void stopSync()
        {
            if (myKeyEventHandeler != null)
            {
                //myKeyEventHandeler = new KeyEventHandler(hook_KeyDown);
                k_hook.KeyDownEvent -= myKeyEventHandeler;//钩住键按下
                //myKeyEventHandeler = new KeyEventHandler(hook_KeyUP);
                k_hook.KeyUpEvent -= myKeyEventHandeler;//钩住松按键
                myKeyEventHandeler = null;
                k_hook.Stop();//安装键盘钩子
            }
            StratTB.Text = "启动同步";
        }

        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (KeyTList.Contains(e.KeyCode.ToString()) == true)
            {
                //INFOout.AppendText(e.KeyCode.ToString() + " 在包含范围内\r\n");
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
                        //PostMessage(p.MainWindowHandle, WM_KEYUP, e.KeyValue, 0);
                        //INFOout.Text += " 同步按键：" + e.KeyCode.ToString() + " values " + e.KeyValue.ToString() + "\r\n";
                    }

                }

            }
        }

        private void hook_KeyUP(object sender, KeyEventArgs e)
        {

            if (KeyTList.Contains(e.KeyCode.ToString()) == true)
            {
                //INFOout.AppendText(e.KeyCode.ToString() + " 在包含范围内\r\n");
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
                        INFOout.Text += (TongBuList.Items[i].ToString() + " 进程丢失！\r\n");
                        TongBuList.Items.RemoveAt(i);
                        i--;
                    }
                    if (FindProcess == true)
                    {
                        //PostMessage(p.MainWindowHandle, WM_KEYDOWN, e.KeyValue, 0);
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
            if (this.Visible) this.Hide();
            else this.Show();

        }

        private void StartUpdate_Click(object sender, EventArgs e)
        {
            //LZDownlaod.UpdateMain();
            //ToTip("开始下载最新 update.exe");
            //LZDownlaod.DownLoadFile("", "update.exe", Environment.CurrentDirectory + @"\");
            //下载

            //ToTip("下载完毕，关闭主程序！");
            Thread.Sleep(1000);
            Process.Start(Environment.CurrentDirectory + @"\update.exe");
            // 在这里执行确定按钮的操作
            Environment.Exit(1);
        }

        Thread ThFish;

        private void StartFish_Click(object sender, EventArgs e)
        {
            ToTip("功能开发中，敬请期待！");
            /*
            ThreadStart ThreadImage = () => ImageFinder.ToFind();
            Thread ImageCh = new Thread(ThreadImage);
            ImageCh.Start();
            */
            //ImageFinder.ToFind();

            StartFish.Enabled = false;
            StartFish.Refresh();

            try
            {
                string imagePath = "test.jpg";

                // 1. 检查文件是否存在
                if (!File.Exists(imagePath))
                {
                    MessageBox.Show($"找不到图像文件: {imagePath}");
                    return;
                }

                int FishProcessID = Convert.ToInt32(FishText.Text);
                Process FishProcess = Process.GetProcessById(FishProcessID);


                // 3. 执行检测
                ThreadStart DiaoyuFun = () => Diaoyu(imagePath, FishProcess);
                DiaoFish = new Thread(DiaoyuFun);
                DiaoFish.Start();

                ThreadStart mintofish = () => dokeyfish(FishProcess);
                ThFish = new Thread(mintofish);
                ThFish.Start();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }


        private void dokeyfish(Process FishProcess)
        {
            while(true)
            { 

            PostMessage(FishProcess.MainWindowHandle, WM_KEYDOWN, (int)Keys.D9, 0);
            PostMessage(FishProcess.MainWindowHandle, WM_KEYUP, (int)Keys.D9, 0);
            Thread.Sleep(31000);
            }
        }

        private void Diaoyu(string imagePath,Process FishProcess)
        {
            // 2. 创建检测器

            while(true)
            { 
                var detector = new ImageFinder(imagePath);
                detector.SimilarityThreshold = 0.5; // 相似度阈值不要设置过高
                detector.DetectScaledImages = true;  // 启用比例缩放检测
                detector.MinScale = 0.3;            // 最小缩放比例
                detector.MaxScale = 3.0;            // 最大缩放比例
                detector.ScaleStep = 0.05;           // 缩放步长

                var result = detector.DetectOnce();

                if (result.IsDetected)
                {
                    //Console.WriteLine($"找到目标！相似度: {result.Similarity:P0}");
                    //MessageBox.Show($"找到目标！相似度: {result.Similarity:P0}");
                    //做具体动作
                    PostMessage(FishProcess.MainWindowHandle, WM_KEYDOWN, (int)Keys.D0, 0);
                    PostMessage(FishProcess.MainWindowHandle, WM_KEYUP, (int)Keys.D0, 0);

                }
                else
                {
                    //MessageBox.Show("未找到目标");
                }
                Thread.Sleep(100);
            }

        }


        private void ZWSearch_Click(object sender, EventArgs e)
        {
            LZClass.ShowCloseAll(true);
            DoShouNa();
        }

        private void DoShouNa()
        {
            try
            {
                LZClass.ZWsearch(tabZW, ZwList);
                //SerachButtonIn();
                //Thread.Sleep(1000);
                CheckProcess();
                ZwList.Items.Add("------------------");
                //ZwList.Items.Add("wow进程", null, (sender, e) => SerachButtonIn());
                ZwList.Items.Add("搜索并DLL注入进程", null, (sender, e) => SandI());
                ZwList.Items.Add("------------------");
                ZwList.Items.Add("退出程序", null, (sender, e) => CloseIndex());
            }
            catch
            { }

        }

        private void SandI()
        {
            CheckProcess();
            LZClass.InjectAll(ZwList, ProcessList, DLLLinkPath.Text,LzIcon);
            //LZClass.LeftDownMessage(LzIcon, "老钟魔兽提醒", "自动挂机已经关闭");
        }

        private void CloseIndex()
        {
            LZClass.ShowCloseAll(true);
            try
            {
                UnregKey();
            }
            catch { }
            LzIcon.Dispose();
            this.Dispose();
            this.Close();
        }

        private void CloseIndex(object sender, FormClosingEventArgs e)
        {
            LZClass.ShowCloseAll(true);
            LzIcon.Dispose();
            this.Dispose();
            this.Close();
        }

        private void ProcessList_DoubleClick(object sender, EventArgs e)
        {
            Toinj();
        }

        private void Toinj()
        {
            //注入 dll
            try
            {
                if (ProcessList.SelectedItem != null && DLLLinkPath.Text != "")
                {

                    try
                    {
                        bool a = LZInject.inDll(Convert.ToInt32(ProcessList.SelectedItem.ToString()), DLLLinkPath.Text);
                        if (a) ToTip(ProcessList.SelectedItem.ToString() + "注入成功");
                        else ToTip(ProcessList.SelectedItem.ToString() + "注入成功");
                    }
                    catch { ToTip(ProcessList.SelectedItem.ToString() + " 注入失败"); }
                }
            }
            catch { }

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
            //ImageFinder aaa = new ImageFinder(29304, "d1.jpg", 500);
            ToTip("开始搜索");
            //aaa.Start();

        }

        private void ShowAllP_Click(object sender, EventArgs e)
        {
            LZClass.ShowCloseAll(true);
        }

        private void HiddenAllP_Click(object sender, EventArgs e)
        {
            LZClass.ShowCloseAll(false);
        }

        private void KeySwitch_CheckedChanged(object sender, EventArgs e)
        {
            if(KeySwitch.Checked ) { KeySwitch.Text = "已关闭快捷键"; }
            else { KeySwitch.Text = "已启用快捷键"; }
        }

        private void LoadSaveList_Click(object sender, EventArgs e)
        {
            
            AutoRunList.Text = LzKeysDo.GetIni("RunList", KeySaveList.Text, "", System.Environment.CurrentDirectory + @"\Runlist.ini");
            TransToCN();
        }

        private void SaveAutoRunList_Click(object sender, EventArgs e)
        {
            InputKey.Focus();
            LzKeysDo.SaveIni("RunList", "AutoRunList", AutoRunList.Text, System.Environment.CurrentDirectory + @"\Runlist.ini");
            LzKeysDo.SaveIni("RunList", KeySaveList.Text, AutoRunList.Text, System.Environment.CurrentDirectory + @"\Runlist.ini");
            
        }

        private void LoadKeyList_Click(object sender, EventArgs e)
        {
            AutoRunList.Text = LzKeysDo.GetIni("RunList", KeySaveList.Text, "", System.Environment.CurrentDirectory + @"\Runlist.ini");
            TransToCN();
            KeySaveList.Refresh();
        }

        private void HandleKeyInput(ForeverTextBox textBox, KeyEventArgs e)
        {
            // 清空文本框
            textBox.Text = "";

            // 构建按键组合字符串
            var keyParts = new List<string>();

            // 检查修饰键
            if (e.Control) keyParts.Add("CTRL");
            if (e.Alt) keyParts.Add("ALT");
            //if (e.Shift) keyParts.Add("Shift");

            // 添加主键（排除修饰键本身）
            if (!IsModifierKey(e.KeyCode))
            {
                keyParts.Add(e.KeyCode.ToString());
            }

            // 组合成字符串（如 "Ctrl+Alt+S"）
            textBox.Text = string.Join("+", keyParts);

            // 阻止后续事件处理
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        // 辅助函数：判断是否是修饰键
        private bool IsModifierKey(Keys key)
        {
            return key == Keys.ControlKey ||
                   key == Keys.Menu ||      // Alt键
                   key == Keys.ShiftKey ||
                   key == Keys.LWin ||
                   key == Keys.RWin;
        }

        private void TBKey_KeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyInput(TBKey, e); // 调用通用方法
        }

        private void GJKey_KeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyInput(GJKey, e); // 调用通用方法
        }

        private void HiddenKey_KeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyInput(HiddenKey, e); // 调用通用方法
        }

        private void InjectKey_KeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyInput(InjectKey, e); // 调用通用方法
        }

        private void ribbonButtonLeft1_Click(object sender, EventArgs e)
        {

            groupBox6.Focus();
            UnregKey();
            RegKey();

        }

        private void RegKey()
        {
            RegisterHotKey(this.Handle, HOTKEY_ID_ALT_1, GetModifiers(GJKey.Text), GetKeyOnly(GJKey.Text)); // 注册 Alt+CTRL+S
            RegisterHotKey(this.Handle, HOTKEY_ID_ALT_2, GetModifiers(TBKey.Text), GetKeyOnly(TBKey.Text)); // 注册 Alt+CTRL+Q
            RegisterHotKey(this.Handle, HOTKEY_ID_ALT_3, GetModifiers(InjectKey.Text), GetKeyOnly(InjectKey.Text)); // 注册 Alt+CTRL+Q
            RegisterHotKey(this.Handle, HOTKEY_ID_ALT_4, GetModifiers(HiddenKey.Text), GetKeyOnly(HiddenKey.Text)); // 注册 Alt+CTRL+1
            //RegisterHotKey(this.Handle, HOTKEY_ID_ALT_5, MOD_ALT | MOD_CRTL, VK_2); // 注册 Alt+CTRL+2

            LzKeysDo.SaveIni("ModKey", "TB", TBKey.Text, System.Environment.CurrentDirectory + @"\Runlist.ini");
            LzKeysDo.SaveIni("ModKey", "GJ", GJKey.Text, System.Environment.CurrentDirectory + @"\Runlist.ini");
            LzKeysDo.SaveIni("ModKey", "Inject", InjectKey.Text, System.Environment.CurrentDirectory + @"\Runlist.ini");
            LzKeysDo.SaveIni("ModKey", "Hidden", HiddenKey.Text, System.Environment.CurrentDirectory + @"\Runlist.ini");

        }

        private int GetModifiers(string KeyString)
        {
            int mod = 0;

            if (KeyString.Contains("CTRL")) mod |= 0x0002;
            if (KeyString.Contains("ALT")) mod |= 0x0001;

            return mod;
        }

        private int GetKeyOnly(string KeyString) 
        {
            string aa =  KeyString.Replace("CTRL", "").Replace("ALT", "").Replace("+", "").Replace(" ", "");
            int virtualKey = 0;
            if (Enum.TryParse(aa, out Keys key))
            {
                // 2. 转换为虚拟键码 (D0 → 0x30)
                virtualKey = (int)key;
                Console.WriteLine($"0x{virtualKey:X}"); // 输出 "0x30"
            }

            //MessageBox.Show(virtualKey.ToString());
            return virtualKey;
        }

        private void UnregKey() 
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_1); // 注销 Alt+1
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_2); // 注销 Alt+2
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_3); // 注销 Alt+2
            UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_4); // 注销 Alt+2
            //UnregisterHotKey(this.Handle, HOTKEY_ID_ALT_5); // 注销 Alt+2
        }
    }
}
