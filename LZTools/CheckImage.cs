using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace LZTools
{
    public class ImageFinder
    {
        private const int CaptureSize = 50; // 截图范围大小

        private readonly int _targetProcessId;
        private readonly string _imagePath;
        private readonly int _scanInterval;

        public ImageFinder(int targetProcessId, string imagePath, int scanInterval = 1000)
        {
            _targetProcessId = targetProcessId;
            _imagePath = imagePath;
            _scanInterval = scanInterval;
        }

        public void Start()
        {
            var timer = new Timer();
            timer.Interval = _scanInterval;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //try
            //{
                // 获取目标进程
                Process targetProcess = Process.GetProcessById(_targetProcessId);
                if (targetProcess == null)
                {
                    Console.WriteLine("Process not found!");
                    return;
                }

                // 获取窗口句柄
                IntPtr hwnd = targetProcess.MainWindowHandle;
                if (hwnd == IntPtr.Zero)
                {
                    Console.WriteLine("Window handle not found!");
                    return;
                }

                // 获取窗口区域
                Rect windowRect = GetWindowRect(hwnd);

                // 捕获窗口截图
                using (Bitmap screenshot = CaptureWindow(hwnd, windowRect))
                {
                    // 将 Bitmap 转换为 Mat
                    Mat sourceImage = BitmapConverter.ToMat(screenshot);

                    // 加载模板图片
                    using (Mat templateImage = Cv2.ImRead(_imagePath, ImreadModes.Color))
                    {
                    // 确保 sourceImage 和 templateImage 都是 3 通道图像
                    //Console.WriteLine($"Source Image Type: {sourceImage.Type()}, Depth: {sourceImage.Depth()}");
                    //Console.WriteLine($"Template Image Type: {templateImage.Type()}, Depth: {templateImage.Depth()}");

                    if (sourceImage.Channels() == 4)
                    {
                        Cv2.CvtColor(sourceImage, sourceImage, ColorConversionCodes.BGRA2BGR);
                    }

                    // 确保图像深度为 CV_8U 或 CV_32F
                    if (sourceImage.Depth() != MatType.CV_8U && sourceImage.Depth() != MatType.CV_32F)
                    {
                        sourceImage.ConvertTo(sourceImage, MatType.CV_8U); // 转换为 CV_8U
                    }
                    if (templateImage.Depth() != MatType.CV_8U && templateImage.Depth() != MatType.CV_32F)
                    {
                        templateImage.ConvertTo(templateImage, MatType.CV_8U); // 转换为 CV_8U
                    }

                    // 确保图像类型一致
                    if (sourceImage.Channels() == 1)
                    {
                        Cv2.CvtColor(sourceImage, sourceImage, ColorConversionCodes.GRAY2BGR); // 灰度转彩色
                    }
                    if (templateImage.Channels() == 1)
                    {
                        Cv2.CvtColor(templateImage, templateImage, ColorConversionCodes.GRAY2BGR); // 灰度转彩色
                    }


                    // 模板匹配
                    Mat result = new Mat();
                        Cv2.MatchTemplate(sourceImage, templateImage, result, TemplateMatchModes.CCoeffNormed);

                        // 获取匹配结果
                        Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out OpenCvSharp.Point maxLoc);

                        // 如果匹配度高于阈值
                        if (maxVal > 0.3) // 阈值可根据实际情况调整
                        {
                            // 计算中心点
                            System.Drawing.Point center = new System.Drawing.Point(
                                maxLoc.X + templateImage.Width / 2,
                                maxLoc.Y + templateImage.Height / 2);

                            // 移动鼠标到中心点
                            //MoveMouse(center.X, center.Y);

                            // 以中心点为中心截图
                            Rectangle captureRect = new Rectangle(
                                center.X - CaptureSize / 2,
                                center.Y - CaptureSize / 2,
                                CaptureSize,
                                CaptureSize);

                            using (Bitmap capturedImage = screenshot.Clone(captureRect, screenshot.PixelFormat))
                            {
                                // 保存截图
                                capturedImage.Save("captured.png", ImageFormat.Png);
                                Console.WriteLine("Image captured and saved as captured.png");
                            }
                        }
                        else 
                        {
                        Console.WriteLine("没找到 !");
                        }

                    }
                }
             // }
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Error: {ex.Message}");
            //}
        }

        private Rect GetWindowRect(IntPtr hwnd)
        {
            RECT rect;
            GetWindowRect(hwnd, out rect);
            return new Rect(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
        }

        private Bitmap CaptureWindow(IntPtr hwnd, Rect rect)
        {
            Bitmap bmp = new Bitmap(rect.Width, rect.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                IntPtr hdc = g.GetHdc();
                PrintWindow(hwnd, hdc, 0);
                g.ReleaseHdc(hdc);
            }
            return bmp;
        }

        private void MoveMouse(int x, int y)
        {
            // 明确使用 System.Drawing.Point
            System.Drawing.Point targetPoint = new System.Drawing.Point(x, y);
            Cursor.Position = targetPoint;
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, int nFlags);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
