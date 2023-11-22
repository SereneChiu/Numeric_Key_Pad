using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Button clickedButton = (Button)e.OriginalSource;   
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
                return;
            }
            if (code == "+/-")
            {
                ModifyNum();
                return;
            }

            if (code== "Settings")
            {
                SettingDialog dialog = new SettingDialog();
                dialog.Show();
            }
        }


        [Flags]
        internal enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
            SMTO_ERRORONEXIT = 0x20
        }

        // Specific import for WM_GETTEXTLENGTH
        [DllImport("user32.dll", EntryPoint = "SendMessageTimeout", CharSet = CharSet.Auto)]
        internal static extern int SendMessageTimeout(
            IntPtr hwnd,
            uint Msg,              // Use WM_GETTEXTLENGTH
            int wParam,
            int lParam,
            SendMessageTimeoutFlags flags,
            uint uTimeout,
            out int lpdwResult);

        // Specific import for WM_GETTEXT
        [DllImport("user32.dll", EntryPoint = "SendMessageTimeout", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern uint SendMessageTimeoutText(
            IntPtr hWnd,
            uint Msg,              // Use WM_GETTEXT
            int countOfChars,
            StringBuilder text,
            SendMessageTimeoutFlags flags,
            uint uTImeoutj,
            out IntPtr result);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr i);

        // callback to enumerate child windows
        private delegate bool EnumWindowProc(IntPtr hwnd, IntPtr parameter);

        private static bool EnumChildWindowsCallback(IntPtr handle, IntPtr pointer)
        {
            // this method will be called foreach child window
            // create a GCHandle from pointer
            var gcHandle = GCHandle.FromIntPtr(pointer);

            // cast pointer as list
            var list = gcHandle.Target as List<IntPtr>;

            if (list == null)
                throw new InvalidCastException("Invalid cast of GCHandle as List<IntPtr>");

            // Adds the handle to the list.
            list.Add(handle);

            return true;
        }

        private static IEnumerable<IntPtr> GetChildWindows(IntPtr parent)
        {
            // Create list to store child window handles.
            var result = new List<IntPtr>();

            // Allocate list handle to pass to EnumChildWindows.
            var listHandle = GCHandle.Alloc(result);

            try
            {
                // enumerates though the children
                EnumChildWindows(parent, EnumChildWindowsCallback, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                // free unmanaged list handle
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }

            return result;
        }

        internal static string GetText(IntPtr hwnd)
        {
            const uint WM_GETTEXTLENGTH = 0x000E;
            const uint WM_GETTEXT = 0x000D;
            int length;
            IntPtr p;

            var result = SendMessageTimeout(hwnd, WM_GETTEXTLENGTH, 0, 0, SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 5, out length);

            if (result != 1 || length <= 0)
                return string.Empty;

            var sb = new StringBuilder(length + 1);

            return SendMessageTimeoutText(hwnd, WM_GETTEXT, sb.Capacity, sb, SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 5, out p) != 0 ?
                    sb.ToString() :
                    string.Empty;
        }

        public static void ModifyNum()
        {
            Process p = Process.GetCurrentProcess();
            Console.WriteLine(GetText(p.MainWindowHandle));    // main window handle of form, returns "Form1"
            Console.WriteLine(GetText(new IntPtr(0x70BA0)));   // actual textbox handle, used Winspector, returns "quertz"

            // iterate through dynamic handles of children
            foreach (var hwnd in GetChildWindows(p.MainWindowHandle))
            {
                double rtn_double = 0.0;
                if (double.TryParse(GetText(hwnd), out rtn_double) == true)
                {
                    keybd_event(36, 0, 0, 0);
                    if (rtn_double > 0)
                    {
                        keybd_event(109, 0, 0, 0);
                    }
                    else
                    {
                        keybd_event(46, 0, 0, 0);
                    }
                    return;
                }
            }

        }

        private Dictionary<string, byte> keys;
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