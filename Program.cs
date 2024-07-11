using GlobalHotKeys;
using GlobalHotKeys.Native.Types;
using Serilog;
using System;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;

[assembly: SupportedOSPlatform("windows")]
namespace GlobalMute
{
    internal partial class Program : ApplicationContext
    {
        private const VirtualKeyCode KeyCode = VirtualKeyCode.VK_PAUSE;
        private const Modifiers Modifier = Modifiers.Shift;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static private partial bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_HIDE = 0;

        private static AwtrixPush ap;
        private static MuteControl mc;
        private static TrayIcon ti;
        private static bool enableAwtrix;

        static void Main(string[] args)
        {
            var temp = Path.GetTempPath();
            Log.Logger = new LoggerConfiguration()
                              .WriteTo.File(path: temp + "mute.log",
              rollOnFileSizeLimit: true,
              retainedFileCountLimit: 1,
              fileSizeLimitBytes: 65535)
                             .MinimumLevel.Debug()
                             .CreateLogger();

            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);

            Log.Information("Starting GlobalMute");

            if (!OperatingSystem.IsWindows())
            {
                Log.Fatal("This must be run on Windows.");
                Environment.Exit(1);
            }

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            ti = new TrayIcon();
            ti.Display();

            enableAwtrix = Boolean.Parse(ConfigurationManager.AppSettings["EnableAwtrix"]);

            if (enableAwtrix)
            {
                ap = new AwtrixPush(ConfigurationManager.AppSettings["AwtrixHost"], 1, ConfigurationManager.AppSettings["MuteColor"]);
            }

            mc = new MuteControl();

            bool status = mc.GetMuteStatus();
            if (enableAwtrix)
            {
                ap.PostMuteStateToDisplay(status);
            }

            ti.SetTrayIconToMuteStatus(status);


            void MuteHotKeyPressed(HotKey hotKey) =>
HandleMuteHotKey(hotKey, ap, mc, ti);

            using var hotKeyManager = new HotKeyManager();
            using var subscription = hotKeyManager.HotKeyPressed.Subscribe(MuteHotKeyPressed);
            using var ctrl1 = hotKeyManager.Register(KeyCode, Modifier);

            Log.Information("Listening for key codes {Modifier} + {HotKey}", Modifier, KeyCode);
            Application.Run();
        }

        private static void HandleMuteHotKey(HotKey hotKey, AwtrixPush ap, MuteControl mc, TrayIcon ti)
        {
            Log.Debug("Hotkey Pressed: {Modifier} + {HotKey}", hotKey.Modifiers, hotKey.Key);
            mc.ToggleMute();
            var status = mc.GetMuteStatus();
            ti.SetTrayIconToMuteStatus(status);
            if (enableAwtrix)
            {
                ap.PostMuteStateToDisplay(status);
            }

        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            ti.Dispose();
            Log.Information("Exiting GlobalMute");
            Log.CloseAndFlush();
        }

    }
}
