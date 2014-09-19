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
    /// Interaction logic for LockScreen.xaml
    /// </summary>
    public partial class Lockscreen : UserControl
    {
        private MainWindow wnd = null;

        public Lockscreen()
        {
            InitializeComponent();
            this.landscapeLockscreenPicker.Loaded += PicPickers_Loaded;
            this.portraitLockscreenPicker.Loaded += PicPickers_Loaded;
            this.landscapeLockscreenPicker.PicChanged += PicChangedHandler;
            this.portraitLockscreenPicker.PicChanged += PicChangedHandler;
            this.landscapeLockscreenPicker.PicPosEnabled = false;
            this.portraitLockscreenPicker.PicPosEnabled = false;
        }

        public void PicChangedHandler(object sender, PicChangedEventArgs e)
        {
            if (wnd == null) return;
            if (sender == this.landscapeLockscreenPicker)
            {
                DWAS2Helper.SetPicturePathConfig(PicMode.Lockscreen, Orientation.Landscape, wnd, e.NewPicPath);
            }
            else if (sender == this.portraitLockscreenPicker)
            {
                DWAS2Helper.SetPicturePathConfig(PicMode.Lockscreen, Orientation.Portrait, wnd, e.NewPicPath);
            }
            wnd.config.SaveConfig();
            wnd.UpdateLockscreen();
        }

        private void LockscreenPage_Loaded(object sender, RoutedEventArgs e)
        {
            DWAS2Helper.SetUserControlLanguage(this);
            wnd = DWAS2Helper.GetUserControlMainWindow(this);
        }

        private void PicPickers_Loaded(object sender, RoutedEventArgs e)
        {
            PicPicker o = ((PicPicker)sender);
            if(o == landscapeLockscreenPicker)
            {
                o.PicPath = DWAS2Helper.GetPicturePathConfig(PicMode.Lockscreen, Orientation.Landscape, wnd);
            }
            else if(o == portraitLockscreenPicker)
            {
                o.PicPath = DWAS2Helper.GetPicturePathConfig(PicMode.Lockscreen, Orientation.Portrait, wnd);
            }
            o.changePicPosButtonText.Text = o.changePicPosButtonText.Text.t(wnd.lang);
        }
    }
}
