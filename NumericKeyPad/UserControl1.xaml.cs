using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace NumericKeyPad
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        /// <summary>
        /// 导入模拟键盘的方法
        /// </summary>
        /// <param name="bVk" >按键的虚拟键值</param>
        /// <param name= "bScan" >扫描码，一般不用设置，用0代替就行</param>
        /// <param name= "dwFlags" >选项标志：0：表示按下，2：表示松开</param>
        /// <param name= "dwExtraInfo">一般设置为0</param>
        [DllImport("User32.dll")]
        public static extern void keybd_event(byte bVK, byte bScan, Int32 dwFlags, int dwExtraInfo);
        public UserControl1()
        {
            InitializeComponent();
            CreateKeys();
        }
        private void ButtonGrid_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)e.OriginalSource;    //获取click事件触发源，即按了的按钮
            string code = (String)clickedButton.Content;
            if (keys.ContainsKey(code))
            {
                byte b = keys[code];
                try
                {
                    keybd_event(b, 0, 0, 0);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        private Dictionary<string, byte> keys;
        /// <summary>
        /// 创建键-值对
        /// </summary>
        private void CreateKeys()
        {
            keys = new Dictionary<string, byte>()
            {
                { "1",97},
                { "2",98},
                { "3",99},
                { "4",100},
                { "5",101},
                { "6",102},
                { "7",103},
                { "8",104},
                { "9",105},
                { "0",96},
                { ".",  110},
                { "+",107},
                { "-",109},
                { "*",106},
                { "/",  111},
                { "Enter",13},
                { "DEL",8}
            };
        }
    }
}