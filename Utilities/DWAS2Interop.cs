using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;

namespace DWAS2.Utilities
{
    internal static class DWAS2Interop
    {
        /***** WINDOWS API *****/

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction,
                                                int uParam,
                                                string lpvParam,
                                                int fuWinIni);

        /// <summary>
        /// Set the desktop wallpaper
        /// </summary>
        /// <param name="path">path of wallpaper</param>
        /// <returns>true if succeed, false otherwise</returns>
        public static bool SetWallpaper(string path)
        {
            if (File.Exists(path))
            {
                int result = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
                if (result != 0) return true;
                else return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Set the picture position of Windows (by setting registry)
        /// </summary>
        /// <param name="p">target picture position</param>
        /// <returns>true if succeed, false otherwise</returns>
        public static bool SetDesktopPicPos(PicPos p)
        {
            try
            {
                RegistryKey reg = Registry.CurrentUser;
                reg = reg.OpenSubKey("Control Panel\\desktop", true);
                reg.SetValue("WallpaperStyle", ((int)p).ToString());
                reg.Close();
                return true;
            }
            catch (Exception ex)
            {
                if (GeneralLogger.IsInitialized()) GeneralLogger.WriteLog("Set desktop picpos failed. error:" + ex.Message, LogType.ERROR);
                return false;
            }
        }

        /// <summary>
        /// Get the desktop orientation
        /// </summary>
        /// <returns>current orientation</returns>
        public static Orientation GetDesktopOrientation(bool reverse)
        {
            ScreenOrientation so = SystemInformation.ScreenOrientation;
            if (so == ScreenOrientation.Angle0 || so == ScreenOrientation.Angle180)
            {
                return (!reverse) ? Orientation.Landscape : Orientation.Portrait;
            }
            else
            {
                return (!reverse) ? Orientation.Portrait : Orientation.Landscape;
            }
        }

        /// <summary>
        /// Set the autorun checkbox
        /// </summary>
        public static bool GetAutorunState()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                object v = key.GetValue("co.logu.DWAS2", null);
                if (v != null)
                {
                    string s = (string)v;
                    return (string.Compare(s, Assembly.GetExecutingAssembly().Location) == 0);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (GeneralLogger.IsInitialized()) GeneralLogger.WriteLog("Get autorun state failed. error:" + ex.Message, LogType.ERROR);
                return false;
            }
        }

        public static bool SetAutorunState(bool autorun)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (autorun)
                {
                    key.SetValue("co.logu.DWAS2", Assembly.GetExecutingAssembly().Location);
                }
                else
                {
                    key.DeleteValue("co.logu.DWAS2", false);
                }
                return true;
            }
            catch (Exception ex)
            {
                if (GeneralLogger.IsInitialized()) GeneralLogger.WriteLog("Set autorun state failed. error:" + ex.Message, LogType.ERROR);
                return false;
            }
        }
    }
}
