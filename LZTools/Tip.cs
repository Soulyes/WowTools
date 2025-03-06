using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LZTools
{
    internal class TipForm : Form
    {
        private Timer timer;
        public static List<TipForm> ActiveTips = new List<TipForm>(); // 存储当前显示的提示框
        public const int MaxTips = 5; // 最多显示 5 个提示框

        public TipForm(string content, int width, int height, int disappearAfterMilliseconds, int offsetX, int offsetY)
        {
            // 设置窗体属性
            this.Size = new Size(width, height);
            this.FormBorderStyle = FormBorderStyle.None; // 无边框
            this.StartPosition = FormStartPosition.Manual; // 手动设置位置
            this.Opacity = 0.7; // 70% 透明度
            this.BackColor = Color.LightYellow; // 背景颜色
            this.BackColor = Color.FromArgb(32, 32, 32); ; // 背景颜色
            this.TopMost = true; // 置顶显示

            // 禁用关闭按钮
            this.ControlBox = false; // 隐藏控制按钮（关闭、最小化等）
            this.ShowInTaskbar = false; // 不在任务栏显示

            // 添加一个标签显示提示内容
            Label label = new Label();
            label.Text = content; // 设置提示内容
            label.AutoSize = false;
            label.Size = new Size(width - 20, height - 20); // 设置标签大小
            label.TextAlign = ContentAlignment.MiddleCenter; // 内容居中
            label.Font = new Font("Arial", 12);
            label.ForeColor = Color.White;
            label.Location = new Point(10, 10);
            this.Controls.Add(label);

            // 设置提示框位置
            SetPosition(offsetX - 100, offsetY - height - 30);

            // 设置定时器，用于自动关闭窗体
            timer = new Timer();
            timer.Interval = disappearAfterMilliseconds; // 设置消失时间
            timer.Tick += Timer_Tick; // 绑定定时器事件
            timer.Start(); // 启动定时器

            // 将当前提示框添加到活动列表中
            ActiveTips.Add(this);
            Console.WriteLine($"提示框已显示，当前活动提示框数量：{ActiveTips.Count}");
        }

        private void SetPosition(int offsetX, int offsetY)
        {
            // 计算新提示框的位置
            int newX = offsetX;
            int newY = offsetY;

            // 检查是否有其他提示框，调整位置
            foreach (var tip in ActiveTips)
            {
                if (tip != this && tip.Visible)
                {
                    newY = tip.Top - this.Height - 10; // 上移
                    // newX = tip.Left + 10; // 右移
                }
            }

            // 确保提示框位置在屏幕范围内
            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
            if (newX < 0) newX = 0;
            if (newY < 0) newY = 0;
            if (newX + this.Width > screenBounds.Width) newX = screenBounds.Width - this.Width;
            if (newY + this.Height > screenBounds.Height) newY = screenBounds.Height - this.Height;

            // 设置提示框位置
            this.Location = new Point(newX, newY);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("定时器触发，准备关闭窗体...");
            timer.Stop();

            // 从活动列表中移除当前提示框
            ActiveTips.Remove(this);

            // 使用 Dispose 关闭窗体
            this.Dispose();
        }

        // 禁用 Alt+F4 关闭窗体
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) // 如果是用户尝试关闭
            {
                e.Cancel = true; // 取消关闭操作
            }
        }
    }

    public static class Tip
    {
        public static void Show(string content, int width, int height, int disappearAfterMilliseconds, int offsetX, int offsetY)
        {
            // 如果当前活动提示框数量超过最大值，关闭最早的提示框
            if (TipForm.ActiveTips.Count >= TipForm.MaxTips)
            {
                var oldestTip = TipForm.ActiveTips[0];
                oldestTip.Dispose();
                TipForm.ActiveTips.Remove(oldestTip);
            }

            // 创建并显示提示窗体
            TipForm tipForm = new TipForm(content, width, height, disappearAfterMilliseconds, offsetX, offsetY);
            tipForm.ShowDialog(); // 使用 Show 显示窗体（非模态）
        }
    }
}