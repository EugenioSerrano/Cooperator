using System;
using System.Collections.Generic;
using System.Text;

namespace Cooperator.Framework.Library.IO
{
    /// <summary>
    /// 
    /// </summary>
    public static class OperatingSystemInfo
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetWindowsVersion()
        {

            // Get OperatingSystem information from the system namespace.
            System.OperatingSystem osInfo = System.Environment.OSVersion;

            // Determine the platform.
            switch (osInfo.Platform)
            {
                // Platform is Windows 95, Windows 98, 
                // Windows 98 Second Edition, or Windows Me.
                case System.PlatformID.Win32Windows:

                    switch (osInfo.Version.Minor)
                    {
                        case 0:
                            return "Windows 95";
                        case 10:
                            if (osInfo.Version.Revision.ToString() == "2222A")
                                return "Windows 98 Second Edition";
                            else
                                return "Windows 98";
                        case 90:
                            return "Windows Me";
                    }
                    break;

                // Platform is Windows NT 3.51, Windows NT 4.0, Windows 2000,
                // or Windows XP.
                case System.PlatformID.Win32NT:

                    switch (osInfo.Version.Major)
                    {
                        case 3:
                            return "Windows NT 3.51";
                        case 4:
                            return "Windows NT 4.0";
                        case 5:
                            if (osInfo.Version.Minor == 0)
                                return "Windows 2000";
                            else
                                return "Windows XP";
                        case 6:
                            return "Windows Vista";
                    }
                    break;

            }
            return "Windows ??";
        }

    }

}

