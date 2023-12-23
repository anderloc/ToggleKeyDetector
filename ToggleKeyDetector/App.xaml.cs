using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ToggleKeyDetector
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;

        private Icon lowercaseIcon = new Icon("./Icons/Lowercase.ico");
        private Icon uppercaseIcon = new Icon("./Icons/Uppercase.ico");
        private Icon numLockIcon = new Icon("./Icons/NumLock.ico");
        private Icon scrollLockIcon = new Icon("./Icons/ScrollLock.ico");

        private const string CapsLockKey = "CapsLock";
        private const string NumLockKey = "NumLock";
        private const string ScrollLockKey = "ScrollLock";
        private static readonly string[] keys = new string[]
        {
            CapsLockKey, NumLockKey, ScrollLockKey
        };

        private Dictionary<string, bool> states = new()
        {
            { CapsLockKey, false },
            { NumLockKey, false },
            { ScrollLockKey, false },
        };

        private object notifyIconLock = new object();

        private const string MutexName = "ToggleKeyDetectorMutex";

        private System.Timers.Timer iconTimer;
        private int iconIndex = 0;

        protected override void OnStartup(StartupEventArgs e)
        {
            var singleAppMutex = new Mutex(false, MutexName, out var createdNew);
            bool isAppRunning = !createdNew;

            if (isAppRunning)
            {
                singleAppMutex = null;
                Application.Current.Shutdown();
                return;
            }

            base.OnStartup(e);
            this.notifyIcon = (TaskbarIcon)this.FindResource("NotifyIcon");

            var executablePath = Assembly.GetExecutingAssembly()?.Location;
            var executableDirectory = executablePath is null ? Directory.GetCurrentDirectory() : Path.GetDirectoryName(executablePath);

            this.lowercaseIcon = new Icon(Path.Join(executableDirectory, "Icons", "Lowercase.ico"));
            this.uppercaseIcon = new Icon(Path.Join(executableDirectory, "Icons", "Uppercase.ico"));
            this.numLockIcon = new Icon(Path.Join(executableDirectory, "Icons", "NumLock.ico"));
            this.scrollLockIcon = new Icon(Path.Join(executableDirectory, "Icons", "ScrollLock.ico"));

            KeyListener.KeyPressed += this.KeyPressedHandler;
            KeyListener.StartListener();

            iconTimer = new System.Timers.Timer();
            iconTimer.Interval = 1_000; // 1 second
            iconTimer.Elapsed += (sender, args) =>
            {
                iconTimer.Stop();
                UpdateIcon();
                iconTimer.Start();
            };
            iconTimer.Start();
        }

        private void KeyPressedHandler(KeyPressedEventArgs e)
        {
            lock (this.notifyIconLock)
            {
                var key = e.Key;
                var isPressed = e.IsPressed;

                string dictKey = string.Empty;
                switch (key)
                {
                    case Key.CapsLock:
                        dictKey = CapsLockKey;
                        break;
                    case Key.NumLock:
                        dictKey = NumLockKey;
                        break;
                    case Key.Scroll:
                        dictKey= ScrollLockKey;
                        break;
                }

                this.states[dictKey] = isPressed;
            }
        }
        
        private void UpdateIcon()
        {
            var startIndex = this.iconIndex;
            Func<int, int> updateIndex = index => (index + 1) % keys.Length;
            do
            {
                var dictKey = keys[this.iconIndex];
                this.iconIndex = updateIndex(this.iconIndex);

                if (!this.states[dictKey] && dictKey != CapsLockKey)
                {
                    continue;
                }

                if (dictKey == ScrollLockKey)
                {
                    this.notifyIcon.Icon = this.scrollLockIcon;
                }
                else if (dictKey == NumLockKey)
                {
                    this.notifyIcon.Icon = this.numLockIcon;
                }
                else
                {
                    this.notifyIcon.Icon = this.states[dictKey] ? this.uppercaseIcon : this.lowercaseIcon;
                }
                break;
            }
            while (this.iconIndex != startIndex);
        }

        
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            KeyListener.StopListener();
        }
    }
}
