using NAudio.CoreAudioApi;
using Serilog;
using System;
using System.Runtime.InteropServices;

namespace GlobalMute
{
    internal class MuteControl
    {

        private readonly MMDevice mmdevice;

        public MuteControl()
        {

            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();

            Log.Information("Getting primary audio sink");
            try
            {
                mmdevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
            }
            catch (COMException ex)
            {
                Log.Fatal(ex, "Error enumerating primary audio sink");
                Environment.Exit(1);
            }
        }

        public void ToggleMute()
        {
            var status = GetMuteStatus();
            Log.Information("Mute status: {Status}", status);

            SetMuteStatus(!status);

            Log.Information("New mute status: {Status}", GetMuteStatus());

        }

        public bool GetMuteStatus()
        {
            return mmdevice.AudioEndpointVolume.Mute;
        }

        public void SetMuteStatus(bool mute)
        {
            mmdevice.AudioEndpointVolume.Mute = mute;
        }
    }
}
