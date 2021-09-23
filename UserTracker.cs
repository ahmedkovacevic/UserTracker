using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32;
public enum PrivateDeviceInUse
{
    MicrophoneInUse,
    WebCamInUse,
    BothPrivateDeviesAreInUse,
    NonePrivateDeviceIsUsed
}

namespace UserTrackerVS
{
    class UserTracker
    {
        public string GetApplicationNameFromString(string arg)
        {
            int first = arg.IndexOf("(");
            int second = arg.IndexOf(")");
            return arg.Substring(first + 1, second - first - 1);
        }

        public PrivateDeviceInUse ArePrivateDevicesInUse()
        {
            List<string> regList = new List<string>{@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam",
                                                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam\NonPackaged",
                                                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\microphone",
                                                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\microphone\NonPackaged"};
            bool isWebCamInUse = false;
            bool isMicrophoneInUse = false;
            foreach (var path in regList)
            {
                using (var key = Registry.CurrentUser.OpenSubKey(path))
                {
                    foreach (var subKeyName in key.GetSubKeyNames())
                    {
                        using (var subKey = key.OpenSubKey(subKeyName))
                        {
                            if (((IList)subKey.GetValueNames()).Contains("LastUsedTimeStop"))
                            {
                                var endTime = subKey.GetValue("LastUsedTimeStop") is long ? (long)subKey.GetValue("LastUsedTimeStop") : -1;
                                if (endTime <= 0)
                                {
                                    if (path.Contains("microphone"))
                                    {
                                        isMicrophoneInUse = true;
                                    }
                                    else if (path.Contains("webcam"))
                                    {
                                        isWebCamInUse = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if(isWebCamInUse && isMicrophoneInUse)
            {
                return PrivateDeviceInUse.BothPrivateDeviesAreInUse;
            }
            else if (isWebCamInUse && !isMicrophoneInUse)
            {
                return PrivateDeviceInUse.WebCamInUse;
            }
            else if(!isWebCamInUse && isMicrophoneInUse)
            {
                return PrivateDeviceInUse.MicrophoneInUse;
            }
            else
            {
                return PrivateDeviceInUse.NonePrivateDeviceIsUsed;
            }
        }
    }
}
