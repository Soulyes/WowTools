using ReaLTaiizor.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LZTools
{
    internal class KeyDo
    {

        public void ReadMKeySetting(Form Tform)//记录
        {
            List<AloneCheckBox> checkboxs = GetAllMButtons(Tform);

            if (checkboxs.Any())
            {
                foreach (AloneCheckBox button in checkboxs)
                {
                    // 在这里可以对每个按钮执行操作
                    string savefile = System.Environment.CurrentDirectory + @"\keyseting.ini";
                    if (GetIni("KeySetting", button.Name.ToString(), "", savefile) == "1") button.Checked = true;
                    else button.Checked = false;
                }
            }
        }

        public void KeyTListin(List<String> ThisList, Form Tform)
        {
            ThisList.Clear();
            List<AloneCheckBox> checkboxs = GetAllMButtons(Tform);

            if (checkboxs.Any())
            {
                foreach (AloneCheckBox button in checkboxs)
                {
                    // 在这里可以对每个按钮执行操作
                    if (button.Checked == true) ThisList.Add(button.Name.Replace("MKEY", ""));
                }
            }
        }

        public void SaveMKeySetting(Form Tform)//记录
        {
            List<AloneCheckBox> checkboxs = GetAllMButtons(Tform);

            if (checkboxs.Any())
            {
                foreach (AloneCheckBox button in checkboxs)
                {
                    // 在这里可以对每个按钮执行操作
                    string savefile = System.Environment.CurrentDirectory + @"\keyseting.ini";
                    if (button.Checked == true) SaveIni("KeySetting", button.Name.ToString(), "1", savefile);
                    else SaveIni("KeySetting", button.Name.ToString(), "0", savefile);
                }
            }
        }

        private List<AloneCheckBox> GetAllMButtons(Control control)
        {
            var buttons = new List<AloneCheckBox>();

            foreach (Control childControl in control.Controls)
            {
                // 如果是按钮控件，则将其添加到列表中
                if (childControl is AloneCheckBox button)
                {
                    buttons.Add(button);
                }

                // 如果控件包含子控件，则递归查找子控件中的按钮
                if (childControl.HasChildren)
                {
                    buttons.AddRange(GetAllMButtons(childControl));
                }
            }

            return buttons;
        }

        public void ReadKeySetting(Form Tform)//记录
        {
            List<System.Windows.Forms.CheckBox> checkboxs = GetAllButtons(Tform);

            if (checkboxs.Any())
            {
                foreach (System.Windows.Forms.CheckBox button in checkboxs)
                {
                    // 在这里可以对每个按钮执行操作
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    string savefile = System.Environment.CurrentDirectory + @"\keyseting.ini";
                    if (GetIni("KeySetting", button.Name.ToString(), "", savefile) == "1") button.Checked = true;
                    else button.Checked = false;
                }
            }
        }

        public void SaveKeySetting(Form Tform)//记录
        {
            List<System.Windows.Forms.CheckBox> checkboxs = GetAllButtons(Tform);

            if (checkboxs.Any())
            {
                foreach (System.Windows.Forms.CheckBox button in checkboxs)
                {
                    // 在这里可以对每个按钮执行操作
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    string savefile = System.Environment.CurrentDirectory + @"\keyseting.ini";
                    if (button.Checked == true) SaveIni("KeySetting", button.Name.ToString(), "1", savefile);
                    else SaveIni("KeySetting", button.Name.ToString(), "0", savefile);
                }
            }
        }

        public void DoKeyNext(string orderlist, RichTextBox OutBox, DungeonListBox GuajiList)
        {
            string[] ListStr = orderlist.Split(',');
            bool ordertype = true;
            if (ListStr.Length > 1)
            {
                OutBox.Text +=(DateTime.Now.ToString() + ": ");
                for (int i = 0; i < ListStr.Length; i++)
                {
                    if (ordertype == true)
                    {
                        OutBox.Text += ("按下按键：" + ListStr[i] + " ");
                        SendKey(GuajiList, ListStr[i]);
                        ordertype = false;
                    }
                    else
                    {
                        // 
                        string[] time = ListStr[i].Split('-');
                        if (time.Count() > 1)
                        {
                            Random rnd = new Random();
                            int rndtime = rnd.Next(int.Parse(time[0]), int.Parse(time[1]));
                            OutBox.Text +=("随机时间为：" + rndtime.ToString() + " ");
                            Thread.Sleep(rndtime);

                        }
                        else
                        {
                            Thread.Sleep(int.Parse(time[0]));
                        }

                        ordertype = true;
                    }
                }
                OutBox.Text +=("\r\n");
                
            }

        }

        [DllImport("User32.dll", EntryPoint = "SendMessage")]　//给控件发送指令
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        const int WM_KEYDOWN = 0x0100; //按下按键
        const int WM_KEYUP = 0x101; //弹起按键 
        public void SendKey(DungeonListBox GuajiList, String Key)
        {
            //循环所有
            for (int i = 0; i < GuajiList.Items.Count; i++)
            {
                string PID = GuajiList.Items[i].ToString();
                Process a = Process.GetProcessById(int.Parse(PID));
                SendMessage(a.MainWindowHandle, WM_KEYDOWN, Convert.ToInt32(Key), 0);
                SendMessage(a.MainWindowHandle, WM_KEYUP, Convert.ToInt32(Key), 0);
            }
        }

        public void FindALLCheckBox(Form Tform)
        {
            List<System.Windows.Forms.CheckBox> checkboxs = GetAllButtons(Tform);

            if (checkboxs.Any())
            {
                foreach (System.Windows.Forms.CheckBox button in checkboxs)
                {
                    // 在这里可以对每个按钮执行操作
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    Console.WriteLine($"\n找到  '{button.Name.ToString()}' ");
                    button.Checked = true;
                }
            }
        }

        private List<System.Windows.Forms.CheckBox> GetAllButtons(Control control)
        {
            var buttons = new List<System.Windows.Forms.CheckBox>();

            foreach (Control childControl in control.Controls)
            {
                // 如果是按钮控件，则将其添加到列表中
                if (childControl is System.Windows.Forms.CheckBox button)
                {
                    buttons.Add(button);
                }

                // 如果控件包含子控件，则递归查找子控件中的按钮
                if (childControl.HasChildren)
                {
                    buttons.AddRange(GetAllButtons(childControl));
                }
            }

            return buttons;
        }

        public string GetSay(string filePath)
        {
            if (File.Exists(filePath))
                return File.ReadAllText(filePath, Encoding.UTF8);
            else return "";
        }

        public void SaveSay(string filePath, RichTextBox textbox)
        {
            File.WriteAllText(filePath, textbox.Text);
        }

        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// 读取ini文件
        /// </summary>
        /// <param name="Section">名称</param>
        /// <param name="Key">关键字</param>
        /// <param name="defaultText">默认值</param>
        /// <param name="iniFilePath">ini文件地址</param>
        /// <returns></returns>
        //读取ini文件
        public string GetIni(string Section, string Key, string defaultText, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                StringBuilder temp = new StringBuilder(1024);
                GetPrivateProfileString(Section, Key, defaultText, temp, 1024, iniFilePath);
                return temp.ToString();
            }
            else
            {
                //MessageBox.Show("没有找到文件");
                //SetValue("服务器地址", "huiji", "47.108.88.175", iniFilePath);
                //SetValue("服务器地址", "keliu", @".\IPVA", iniFilePath);
                return defaultText;
            }
        }

        public bool SaveIni(string Section, string Key, string Value, string iniFilePath)
        {
            var pat = Path.GetDirectoryName(iniFilePath);
            if (Directory.Exists(pat) == false)
            {
                Directory.CreateDirectory(pat);
            }
            if (File.Exists(iniFilePath) == false)
            {
                File.Create(iniFilePath).Close();
            }
            long OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
            if (OpStation == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
