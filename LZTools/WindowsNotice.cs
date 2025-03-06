using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LZTools
{
    public class ToastNotification : Form
    {
        private Timer timer;
        private int fadeStep = 10; // 淡出步长
        private int fadeInterval = 50; // 淡出间隔（毫秒）
        private int displayTime; // 显示时间（毫秒）

        /// <summary>
        /// 创建一个右下角提示框
        /// </summary>
        /// <param name="title">提示标题</param>
        /// <param name="message">提示内容</param>
        /// <param name="displayTime">显示时间（毫秒）</param>
        public ToastNotification(string title, string message, int displayTime = 5000)
        {
            this.displayTime = displayTime;

            // 设置窗体属性
            this.FormBorderStyle = FormBorderStyle.None; // 无边框
            //this.BackColor = Color.LightGray; // 背景颜色
            this.BackColor = Color.Black; // 背景颜色
            this.Size = new Size(300, 100); // 窗体大小
            this.StartPosition = FormStartPosition.Manual; // 手动设置位置
            this.Opacity = 20; // 初始透明度为0（完全透明）
            this.ShowInTaskbar = false;

            // 设置标题和内容
            Label titleLabel = new Label
            {
                Text = title,
                Font = new Font("微软雅黑", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 10),
                AutoSize = true
            };

            Label messageLabel = new Label
            {
                Text = message,
                Font = new Font("微软雅黑", 10),
                ForeColor = Color.White,
                Location = new Point(10, 40),
                AutoSize = true
            };

            this.Controls.Add(titleLabel);
            this.Controls.Add(messageLabel);

            // 初始化 Timer
            timer = new Timer
            {
                Interval = displayTime // 显示时间
            };
            timer.Tick += Timer_Tick;

            // 定位到屏幕右下角
            Rectangle screenBounds = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(
                screenBounds.Right - this.Width - 10,
                screenBounds.Bottom - this.Height - 10
            );

            // 显示窗体并开始淡入
            this.Show();
            FadeIn();
        }

        // 淡入效果
        private void FadeIn()
        {
            Timer fadeInTimer = new Timer
            {
                Interval = fadeInterval
            };
            fadeInTimer.Tick += (s, e) =>
            {
                if (this.Opacity < 1)
                {
                    this.Opacity += 0.1; // 每次增加10%透明度
                }
                else
                {
                    fadeInTimer.Stop();
                    timer.Start(); // 开始计时，准备淡出
                }
            };
            fadeInTimer.Start();
        }

        // 淡出效果
        private void FadeOut()
        {
            Timer fadeOutTimer = new Timer
            {
                Interval = fadeInterval
            };
            fadeOutTimer.Tick += (s, e) =>
            {
                if (this.Opacity > 0)
                {
                    this.Opacity -= 0.1; // 每次减少10%透明度
                }
                else
                {
                    fadeOutTimer.Stop();
                    this.Close(); // 关闭窗体
                }
            };
            fadeOutTimer.Start();
        }

        // Timer 触发事件
        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            FadeOut(); // 开始淡出
        }
    }
}
