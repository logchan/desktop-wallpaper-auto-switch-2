using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using DWAS2.Components;
using DWAS2.Utilities;
using MultiLang;

namespace DWAS2.Pages
{
    /// <summary>
    /// Interaction logic for Wallpaper.xaml
    /// </summary>
    public partial class Wallpaper : UserControl
    {
        private MainWindow wnd = null;

        public Wallpaper()
        {
            InitializeComponent();
            this.landscapeWallpaperPicker.Loaded += PicPickers_Loaded;
            this.portraitWallpaperPicker.Loaded += PicPickers_Loaded;
            this.landscapeWallpaperPicker.PicChanged += PicChangedHandler;
            this.portraitWallpaperPicker.PicChanged += PicChangedHandler;
            this.landscapeWallpaperPicker.PicPosChanged += PicPosChangedHandler;
            this.portraitWallpaperPicker.PicPosChanged += PicPosChangedHandler;
        }

        private void PicChangedHandler(object sender, PicChangedEventArgs e)
        {
            if (wnd == null) return;
            if(sender == this.landscapeWallpaperPicker)
            {
                DWAS2Helper.SetPicturePathConfig(PicMode.Wallpaper, Orientation.Landscape, wnd, e.NewPicPath);
            }
            else if(sender == this.portraitWallpaperPicker)
            {
                DWAS2Helper.SetPicturePathConfig(PicMode.Wallpaper, Orientation.Portrait, wnd, e.NewPicPath);
            }
            wnd.config.SaveConfig();
            wnd.UpdateWallpaper();
        }

        private void PicPosChangedHandler(object sender, PicPosChangedEventArgs e)
        {
            if (wnd == null) return;
            if(sender == this.landscapeWallpaperPicker)
            {
                this.landscapeWallpaperPicker.changePicPosButtonText.Text = this.landscapeWallpaperPicker.changePicPosButtonText.Text.t(wnd.lang);
                DWAS2Helper.SetPicPosConfig(PicMode.Wallpaper, Orientation.Landscape, wnd, e.NewPicPos);
            }
            else if(sender == this.portraitWallpaperPicker)
            {
                this.portraitWallpaperPicker.changePicPosButtonText.Text = this.portraitWallpaperPicker.changePicPosButtonText.Text.t(wnd.lang);
                DWAS2Helper.SetPicPosConfig(PicMode.Wallpaper, Orientation.Portrait, wnd, e.NewPicPos);
            }
            wnd.config.SaveConfig();
            wnd.UpdateWallpaper();
        }

        private void WallpaperPage_Loaded(object sender, RoutedEventArgs e)
        {
            DWAS2Helper.SetUserControlLanguage(this);
            wnd = DWAS2Helper.GetUserControlMainWindow(this);
        }

        private void PicPickers_Loaded(object sender, RoutedEventArgs e)
        {
            PicPicker o = ((PicPicker)sender);
            if(o == landscapeWallpaperPicker)
            {
                o.SelectedPicPos = DWAS2Helper.GetPicPosConfig(PicMode.Wallpaper, Orientation.Landscape, wnd);
                o.PicPath = DWAS2Helper.GetPicturePathConfig(PicMode.Wallpaper, Orientation.Landscape, wnd);
            }
            else if (o == portraitWallpaperPicker)
            {
                o.SelectedPicPos = DWAS2Helper.GetPicPosConfig(PicMode.Wallpaper, Orientation.Portrait, wnd);
                o.PicPath = DWAS2Helper.GetPicturePathConfig(PicMode.Wallpaper, Orientation.Portrait, wnd);
            }
            o.SetPicPosButtonText();
            o.changePicPosButtonText.Text = o.changePicPosButtonText.Text.t(wnd.lang);
        }
    }
}
