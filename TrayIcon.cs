using System;
using System.Drawing;
using System.Windows.Forms;

namespace GlobalMute
{
    internal class TrayIcon : IDisposable
    {
        public NotifyIcon NotifyIcon { get; set; }
        private readonly Icon muteIcon;
        private readonly Icon activeIcon;
        private bool disposed = false;

        public TrayIcon()
        {
            NotifyIcon = new NotifyIcon();
            muteIcon = Properties.Resources.MicOff;
            activeIcon = Properties.Resources.MicOn;
        }

        public void Display()
        {
            NotifyIcon.Text = "GlobalMute";
            NotifyIcon.Icon = activeIcon;
            NotifyIcon.Visible = true;

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Exit", null, MenuExitEvent);
            NotifyIcon.ContextMenuStrip = contextMenu;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                NotifyIcon.Dispose();
                muteIcon?.Dispose();
                activeIcon?.Dispose();
            }

            disposed = true;
        }

        ~TrayIcon()
        {
            Dispose(false);
        }

        public void SetTrayIconToMuteStatus(bool status)
        {
            NotifyIcon.Text = status ? "Muted" : "Active";
            NotifyIcon.Icon = status ? muteIcon : activeIcon;
        }

        private void MenuExitEvent(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
