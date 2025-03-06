using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LZTools
{
    internal class emunClass
    {
        public int GetCharValue(string a)
        {
            a = a.ToUpper();
            if (char.IsDigit(a[0])) a = "D" + a;
            Keys key;

            if (a == " ") return (int)Keys.Space;

            if (Enum.TryParse<Keys>(a, out key))
            {
                return (int)key;
            }
            else
            {
                //Console.WriteLine($"\n无法找到与字符 '{a}' 对应的 Keys 枚举值");
                return 0;
            }
        }

        public string GetCharKey(int KeyCode)
        {
            Keys key = (Keys)KeyCode;

            // 获取枚举值对应的名称
            string keyName = Enum.GetName(typeof(Keys), key);

            if (keyName != null)
            {
                Console.WriteLine($"键盘键码 {KeyCode} 对应的 Keys 枚举值为 {keyName}");
            }
            else
            {
                Console.WriteLine($"无法找到键盘键码 {KeyCode} 对应的 Keys 枚举值");
            }
            return keyName;
        }
    }
}
