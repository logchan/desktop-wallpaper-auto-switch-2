using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DWAS2.Utilities
{
    class DWAS2Helper
    {
        public static string GetPicturePathConfig(PicMode picMode, Orientation orientation, MainWindow wnd)
        {
            if (wnd == null) return null;
            switch (picMode)
            {
                case PicMode.Wallpaper:
                    switch(orientation)
                    {
                        case Orientation.Landscape:
                            return wnd.config.wpLandscape;
                        case Orientation.Portrait:
                            return wnd.config.wpPortrait;
                    }
                    break;
                case PicMode.Lockscreen:
                    switch(orientation)
                    {
                        case Orientation.Landscape:
                            return wnd.config.lcLandscape;
                        case Orientation.Portrait:
                            return wnd.config.lcPortrait;
                    }
                    break;
            }
            return null;
        }

        public static void SetPicturePathConfig(PicMode picMode, Orientation orientation, MainWindow wnd, string value)
        {
            switch (picMode)
            {
                case PicMode.Wallpaper:
                    switch (orientation)
                    {
                        case Orientation.Landscape:
                            wnd.config.wpLandscape = value;
                            break;
                        case Orientation.Portrait:
                            wnd.config.wpPortrait = value;
                            break;
                    }
                    break;
                case PicMode.Lockscreen:
                    switch (orientation)
                    {
                        case Orientation.Landscape:
                            wnd.config.lcLandscape = value;
                            break;
                        case Orientation.Portrait:
                            wnd.config.lcPortrait = value;
                            break;
                    }
                    break;
            }
        }

        public static PicPos GetPicPosConfig(PicMode picMode, Orientation orientation, MainWindow wnd)
        {
            switch (picMode)
            {
                case PicMode.Wallpaper:
                    switch (orientation)
                    {
                        case Orientation.Landscape:
                            return wnd.config.wpPosLandscape;
                        case Orientation.Portrait:
                            return wnd.config.wpPosPortrait;
                    }
                    break;
            }
            throw new InvalidOperationException(String.Format("No config for {0} @ {1} available.", picMode.ToString(), orientation.ToString()));
        }

        public static void SetPicPosConfig(PicMode picMode, Orientation orientation, MainWindow wnd, PicPos value)
        {
            switch (picMode)
            {
                case PicMode.Wallpaper:
                    switch (orientation)
                    {
                        case Orientation.Landscape:
                            wnd.config.wpPosLandscape = value;
                            break;
                        case Orientation.Portrait:
                            wnd.config.wpPosPortrait = value;
                            break;
                    }
                    break;
            }
        }

        public static MainWindow GetUserControlMainWindow(UserControl uc)
        {
            if (Window.GetWindow(uc) is MainWindow)
                return (MainWindow)Window.GetWindow(uc);
            else
            {
                return null;
            }
        }

        public static void SetUserControlLanguage(UserControl uc)
        {
            MainWindow theWnd = GetUserControlMainWindow(uc);
            if (theWnd != null)
            {
                WPFHelper.ApplyUILanguage(theWnd.lang, uc);
            }
        }
    }
}
