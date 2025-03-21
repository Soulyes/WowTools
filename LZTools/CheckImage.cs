using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Tesseract;

namespace LZTools
{
    public class ImageFinder
    {
        private static TesseractEngine engine = new TesseractEngine(Environment.CurrentDirectory + @"\", "chi_sim", EngineMode.Default);

        public static void ToFind()
        {
            while (true)
            {
                // 截取屏幕
                Bitmap screenshot = CaptureScreen();

                // 使用 Tesseract 识别文字
                string recognizedText = RecognizeText(screenshot);

                // 检查是否包含目标文字
                if (recognizedText.Contains("123"))
                {
                    Console.WriteLine("检测到目标文字！");
                }
                else
                {
                    Console.WriteLine("未检测到目标文字。");
                }

                // 暂停一段时间，避免过度占用 CPU
                System.Threading.Thread.Sleep(1000); // 1 秒
            }
        }

        static Bitmap CaptureScreen()
        {
            // 获取屏幕大小
            Rectangle bounds = Screen.PrimaryScreen.Bounds;

            // 创建位图对象
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);

            // 截取屏幕
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }

            return bitmap;
        }

        static string RecognizeText(Bitmap image)
        {
            // 初始化 Tesseract
                // 将图像转换为 Pix
                using (var pix = PixConverter.ToPix(image))
                {
                    // 识别文字
                    using (var page = engine.Process(pix))
                    {
                        return page.GetText();
                    }
                }
            
        }

    }
}
