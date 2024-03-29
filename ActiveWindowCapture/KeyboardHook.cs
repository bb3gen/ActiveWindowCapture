﻿using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ActiveWindowCapture
{
    // https://stackoverflow.com/questions/2450373/set-global-hotkeys-using-c-sharp

    public sealed class KeyboardHook : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private Window _window = new Window();
        private int _currentID;

        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        public KeyboardHook()
        {
            _window.KeyPressed += (sender, args) =>
            {
                if (KeyPressed != null)
                    KeyPressed(this, args);
            };
        }

        public void RegisterHotKey(ModifierKeys modifier, Keys key)
        {
            _currentID += 1;

            if (!RegisterHotKey(_window.Handle, _currentID, (uint)modifier, (uint)key))
                throw new InvalidOperationException("指定されたホットキーはすでに他のアプリケーションで予約されています。");
        }

        public void Dispose()
        {
            for (int i = _currentID; i > 0; i--)
            {
                UnregisterHotKey(_window.Handle, i);
            }

            _window.Dispose();
        }

        #region Window クラス
        private class Window : NativeWindow, IDisposable
        {
            private const int WM_HOTKEY = 0x0312;

            public event EventHandler<KeyPressedEventArgs> KeyPressed;

            public Window()
            {
                this.CreateHandle(new CreateParams());
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (m.Msg == WM_HOTKEY)
                {
                    var key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    var modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);

                    if (KeyPressed != null)
                        KeyPressed(this, new KeyPressedEventArgs(modifier, key));
                }
            }

            public void Dispose()
            {
                this.DestroyHandle();
            }
        }
        #endregion
    }
}
