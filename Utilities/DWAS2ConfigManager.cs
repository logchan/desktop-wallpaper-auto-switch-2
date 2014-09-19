using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows;

namespace DWAS2.Utilities
{
    public struct DWAS2Config
    {
        public string wpLandscape;
        public string wpPortrait;
        public string lcLandscape;
        public string lcPortrait;
        public string language;

        public bool reverse;

        public PicPos wpPosLandscape;
        public PicPos wpPosPortrait;

        public int lastWidth;
        public int lastHeight;
        public int ver;

        /// <summary>
        /// Read configuration file
        /// </summary>
        public void ReadConfig()
        {
            // get configuration with default values
            // images and language
            wpLandscape = ReadOneSettingSafe("wpLandscape", "");
            wpPortrait = ReadOneSettingSafe("wpPortrait", "");
            lcLandscape = ReadOneSettingSafe("lcLandscape", "");
            lcPortrait = ReadOneSettingSafe("lcPortrait", "");
            language = ReadOneSettingSafe("language", "");

            // reverse
            string revstr = "";
            revstr = ReadOneSettingSafe("reverse", null);
            if (revstr != null && revstr == true.ToString())
            {
                reverse = true;
            }
            else
            {
                reverse = false;
            }

            // picture position
            string poswpl = "";
            string poswpp = "";
            poswpl = ReadOneSettingSafe("posWpLandscape", "Fill");
            poswpp = ReadOneSettingSafe("posWpPortrait", "Fill");
            wpPosLandscape = PicPos.Fill;
            wpPosPortrait = PicPos.Fill;
            foreach (string pos in Enum.GetNames(typeof(PicPos)))
            {
                if (string.Compare(poswpl, pos, ignoreCase: true) == 0) Enum.TryParse<PicPos>(pos, true, out wpPosLandscape);
                if (string.Compare(poswpp, pos, ignoreCase: true) == 0) Enum.TryParse<PicPos>(pos, true, out wpPosPortrait);
            }

            // last size
            string rWidth = ReadOneSettingSafe("lastWidth", "768");
            string rHeight = ReadOneSettingSafe("lastHeight", "535");
            int.TryParse(rWidth, out lastWidth);
            int.TryParse(rHeight, out lastHeight);
            if (lastWidth <= 0) lastWidth = 768;
            if (lastHeight <= 0) lastHeight = 535;

            // last version
            string rver = ReadOneSettingSafe("ver", "100");
            int.TryParse(rver, out ver);

            SaveConfig();
        }

        /// <summary>
        /// Read the config and get one key value in a safe way (prevent null ref)
        /// </summary>
        /// <param name="key">the key of the config</param>
        /// <param name="defaultValue">value when failed to get (e.g. key doesn't exist)</param>
        /// <returns>the value of config if key found, or the default value otherwise</returns>
        private string ReadOneSettingSafe(string key, string defaultValue)
        {
            try
            {
                string[] ret = ConfigurationManager.AppSettings.GetValues(key);
                if (ret != null && ret.Length > 0)
                {
                    return ret[0];
                }
                else
                {
                    return defaultValue;
                }
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Save config to file
        /// </summary>
        public void SaveConfig()
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings.Clear();
                // maybe one day I will use reflection to make this better
                config.AppSettings.Settings.Add("wpLandscape", wpLandscape);
                config.AppSettings.Settings.Add("wpPortrait", wpPortrait);
                config.AppSettings.Settings.Add("lcLandscape", lcLandscape);
                config.AppSettings.Settings.Add("lcPortrait", lcPortrait);
                config.AppSettings.Settings.Add("language", language);
                config.AppSettings.Settings.Add("reverse", reverse.ToString());
                config.AppSettings.Settings.Add("posWpLandscape", wpPosLandscape.ToString());
                config.AppSettings.Settings.Add("posWpPortrait", wpPosPortrait.ToString());
                config.AppSettings.Settings.Add("lastWidth", lastWidth.ToString());
                config.AppSettings.Settings.Add("lastHeight", lastHeight.ToString());
                config.AppSettings.Settings.Add("ver", ver.ToString());
                config.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fatal error: failed writing config file. Permission needed?\n严重错误：写入设置文件失败。请检查权限。", "", MessageBoxButton.OK, MessageBoxImage.Error);
                if (GeneralLogger.IsInitialized()) GeneralLogger.WriteLog("Failed writing config file, error:" + ex.Message, LogType.ERROR);
            }
        }

    }
}
