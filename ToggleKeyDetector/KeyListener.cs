using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;

namespace ToggleKeyDetector
{
    public class KeyPressedEventArgs
    {
        public Key Key { get; set; }
        public bool IsPressed { get; set; }
        public KeyPressedEventArgs(Key key, bool isPressed)
        {
            this.Key = key;
            this.IsPressed = isPressed;
        }
    }

    public delegate void KeyPressedEventHandler(KeyPressedEventArgs e);

    public static class KeyListener
    {
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);

        private const int TimerIntervalMilliseconds = 100;
        private static Timer? listenerTimer;
        public static bool IsListening { get; private set; }
        private static object threadLock = new object();

        private static bool IsCapsLockPressed = false;
        private static bool IsNumLockPressed = false;
        private static bool IsScrollPressed = false;

        public const int CapsLockKey = 0x14;
        public const int NumLockKey = 0x90;
        public const int ScrollKey = 0x91;

        public static event KeyPressedEventHandler KeyPressed;

        public static void StartListener()
        {
            lock (threadLock)
            {
                if (IsListening)
                {
                    return;
                }

                if (listenerTimer is null)
                {
                    listenerTimer = new Timer();
                    listenerTimer.Interval = TimerIntervalMilliseconds;
                    listenerTimer.Elapsed += (sender, args) =>
                    {
                        listenerTimer.Stop();
                        OnListenerElapsed();
                        listenerTimer.Start();
                    };
                }

                listenerTimer.Start();

                IsListening = true;
            }
        }

        public static void StopListener()
        {
            lock (threadLock)
            {
                listenerTimer?.Stop();
                IsListening = false;
            }
        }

        private static void OnListenerElapsed()
        {
            IsCapsLockPressed = CheckKeyPress(Key.CapsLock, CapsLockKey, IsCapsLockPressed);
            IsNumLockPressed = CheckKeyPress(Key.NumLock, NumLockKey, IsNumLockPressed);
            IsScrollPressed = CheckKeyPress(Key.Scroll, ScrollKey, IsScrollPressed);
        }

        private static bool CheckKeyPress(Key key, int keyCode, bool previousPressResult)
        {
            bool isPressed = IsKeyPressed(keyCode);
            if (isPressed != previousPressResult)
            {
                KeyPressed?.Invoke(new KeyPressedEventArgs(key, isPressed));
            }

            return isPressed;
        }

        private static bool IsKeyPressed(int key)
        {
            //var state = GetAsyncKeyState((int)key);
            var state = GetKeyState(key);

            return state != 0;
        }
    }
}
