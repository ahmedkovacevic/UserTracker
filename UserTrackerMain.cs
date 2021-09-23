using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Globalization;

namespace UserTrackerVS
{
    class UserTrackerMain
    {
        static void Main(string[] args)
        {
            int lastProcessId = -1;
            bool cameraIsActive = false;
            bool microphoneIsActive = false;
            FileHandler fileHandler = new FileHandler();
            UserTracker userTracker = new UserTracker();
            while (true)
            {
                DateTime localDate = DateTime.Now;
                var culture = new CultureInfo("bs-Latn-BA");

                //----------------------- Check Active Window --------------------------------------
                var activatedHandle = GetForegroundWindow();
                if (activatedHandle == IntPtr.Zero)
                {
                    continue;       // No window is currently activated
                }

                int activeProcId;
                string result = "";
                GetWindowThreadProcessId(activatedHandle, out activeProcId);
                if (lastProcessId == -1 || lastProcessId != activeProcId)
                {
                    lastProcessId = activeProcId;
                    string appName = userTracker.GetApplicationNameFromString(Process.GetProcessById(activeProcId).ToString());
                    result = String.Format("Date: {0}, User: {1}, Active application: {2}", localDate.ToString(culture), Environment.UserName, appName);
                    Console.WriteLine(result);
                    fileHandler.AddText("log-app.txt", result);
                }

                result = "";

                //----------------------- Check If Webcam or Microphone is in use --------------------------------------
                switch (userTracker.ArePrivateDevicesInUse())
                {
                    case PrivateDeviceInUse.MicrophoneInUse:
                        if (!microphoneIsActive)
                        {
                            microphoneIsActive = true;
                            result = String.Format("Date: {0}, User: {1}, Microphone is in use", localDate.ToString(culture), Environment.UserName);
                        }
                        cameraIsActive = false;
                        break;
                    case PrivateDeviceInUse.WebCamInUse:
                        if (!cameraIsActive)
                        {
                            cameraIsActive = true; 
                            result = String.Format("Date: {0}, User: {1}, Webcam is in use", localDate.ToString(culture), Environment.UserName);
                        }
                        microphoneIsActive = false;
                        break;
                    case PrivateDeviceInUse.BothPrivateDeviesAreInUse:
                        if(!cameraIsActive || !microphoneIsActive)
                        {
                            cameraIsActive = true;
                            microphoneIsActive = true;
                            result = String.Format("Date: {0}, User: {1}, Both private devices are in use", localDate.ToString(culture), Environment.UserName);
                        }
                        break;
                    case PrivateDeviceInUse.NonePrivateDeviceIsUsed:
                        microphoneIsActive = false;
                        cameraIsActive = false;
                        break;
                }

                if(result != "")
                {
                    Console.WriteLine(result);
                    fileHandler.AddText("log-camera.txt", result);
                }

                //----------------------- Check If log-app.txt file is opened --------------------------------------
                if (fileHandler.IsFileLocked())
                {
                    Console.WriteLine("File is locked!");
                }

                Thread.Sleep(500);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

    }
}
