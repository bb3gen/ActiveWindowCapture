using System;
using System.Windows.Forms;

namespace ActiveWindowCapture
{
    public partial class Form1 : Form
    {
        private KeyboardHook hook = new KeyboardHook();

        public Form1()
        {
            InitializeComponent();

            hook.KeyPressed += Hook_KeyPressed;
            hook.RegisterHotKey(ActiveWindowCapture.ModifierKeys.Control | ActiveWindowCapture.ModifierKeys.Alt, Keys.F12);
        }

        private void Hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            label1.Text = e.Modifier.ToString() + " + " + e.Key.ToString() + " : " + DateTime.Now.ToLongTimeString();

            //[Alt] + [PrintScreen] キーを送信
            SendKeys.SendWait("%{PRTSC}");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            hook.Dispose();
        }
    }
}
