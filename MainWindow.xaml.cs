using FirstFloor.ModernUI.Windows.Controls;
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
using System.IO;

using DWAS2.Utilities;
using MultiLang;
using WinForms = System.Windows.Forms;

namespace DWAS2
{

    public class OrientationChangedEventArgs : EventArgs
    {
        public OrientationChangedEventArgs(Orientation orientation)
        {
            newOrientation = orientation;
        }
        private Orientation newOrientation;
        public Orientation NewOrientation { get { return newOrientation; } }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public DWAS2Config config = new DWAS2Config();
        public List<MultiLangEngine> languages;
        public MultiLangEngine lang;
        public Orientation lastOrientation;

        private bool shouldHideAtStartup = false;
        private WinForms.NotifyIcon notifyIcon = new WinForms.NotifyIcon();
        private System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            
            // init logger
            string logpath = System.AppDomain.CurrentDomain.BaseDirectory + @"\DWAS2.log";
            GeneralLogger.Initialize(logpath);

            // read and process configuration
            config.ReadConfig();
            ProcessConfig();

            // check hiding
            if (File.Exists(config.wpLandscape) && File.Exists(config.wpPortrait) && File.Exists(config.lcLandscape) && File.Exists(config.lcPortrait))
                this.shouldHideAtStartup = true;

            // read language
            languages = DWAS2LanguageManager.LoadLanguages();
            lang = DWAS2LanguageManager.SelectLanguage(languages, config.language);
            if(lang.Language.CompareTo(config.language) != 0)
            {
                config.language = lang.Language;
                config.SaveConfig();
            }
            // LANGUAGE IS PROCESSED IN WINDOW_LOADED EVENT HANDLER

            // read orientation
            lastOrientation = DWAS2Interop.GetDesktopOrientation(config.reverse);

            // notify icon
            InitNotifyIcon();

            // initial set
            UpdateDetection();

            // start timer
            InitTimer();
        }

        /*** Core Features ***/

        public event EventHandler<OrientationChangedEventArgs> OrientationChanged;

        public virtual void OrientationChangedEvent(OrientationChangedEventArgs e)
        {
            EventHandler<OrientationChangedEventArgs> handler = OrientationChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void UpdateDetection()
        {
            var newOrientation = DWAS2Interop.GetDesktopOrientation(config.reverse);
            if(newOrientation != lastOrientation)
            {
                lastOrientation = newOrientation;

                UpdateNotifyIcon();
                UpdateWallpaper();
                UpdateLockscreen();

                OrientationChangedEvent(new OrientationChangedEventArgs(newOrientation));
            }
        }

        public void UpdateWallpaper()
        {
            string path = lastOrientation == Orientation.Landscape ? config.wpLandscape : config.wpPortrait;
            PicPos picpos = lastOrientation == Orientation.Landscape ? config.wpPosLandscape : config.wpPosPortrait;
            if(File.Exists(path))
            {
                if (!DWAS2Interop.SetDesktopPicPos(picpos))
                {
                    notifyIcon.ShowBalloonTip(1000, "DWAS2", "DWAS2 failed to set your picture position.".t(lang), WinForms.ToolTipIcon.Error);
                }
                if(!DWAS2Interop.SetWallpaper(path))
                {
                    notifyIcon.ShowBalloonTip(1000, "DWAS2", "DWAS2 failed to set your wallpaper.".t(lang), WinForms.ToolTipIcon.Error);
                    return;
                }
            }
            else
            {
                notifyIcon.ShowBalloonTip(1000, "DWAS2", "DWAS2 wallpaper for current orientation does not exist.".t(lang), WinForms.ToolTipIcon.Error);
            }
        }

        public void UpdateLockscreen()
        {
            string path = lastOrientation == Orientation.Landscape ? config.lcLandscape : config.lcPortrait;
            if(File.Exists(path))
            {
                // check if the file is already in PictureLibrary
                FileInfo fi = new FileInfo(path);
                string myPicturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                if (fi.Directory.FullName == myPicturesPath)
                {
                    // if so, directly set
                    DWAS2WinRT.SetLockscreen(fi.Name);
                }
                else
                {
                    try
                    { 
                        // if not, copy the file with a unique name and set
                        Guid g = Guid.NewGuid();
                        // while (Guid.Empty == g) g = new Guid();
                        string fname = g.ToString();
                        fname += fi.Extension;
                        fi.CopyTo(myPicturesPath + @"\" + fname);
                        DWAS2WinRT.SetLockscreen(fname, deleteAfterwards: true);
                    }
                    catch(Exception ex)
                    {
                        if (GeneralLogger.IsInitialized()) GeneralLogger.WriteLog("Failed to set lock screen. error:" + ex.Message, LogType.ERROR);
                        notifyIcon.ShowBalloonTip(1000, "DWAS2", "DWAS2 failed to set your lockscreen.".t(lang), WinForms.ToolTipIcon.Error);
                    }
                }
            }
            else
            {
                notifyIcon.ShowBalloonTip(1000, "DWAS2", "DWAS2 lockscreen for current orientation does not exist.".t(lang), WinForms.ToolTipIcon.Error);
            }
        }

        /*** Private Methods ***/

        private void InitTimer()
        {
            timer.Tick += new EventHandler(delegate { UpdateDetection(); });
            timer.Interval = new TimeSpan(0, 0, 0, 0, milliseconds: 500);
            timer.Start();
        }

        private void InitNotifyIcon()
        {
            // Text
            notifyIcon.Text = "Desktop Wallpaper Auto Switch 2".t(lang);

            // Context menu
            WinForms.ContextMenu menu = new WinForms.ContextMenu();

            WinForms.MenuItem showItem = new WinForms.MenuItem();
            showItem.Text = "Show DWAS2 Window".t(lang);
            showItem.Click += new EventHandler(delegate { ShowWindow(); });

            WinForms.MenuItem hideItem = new WinForms.MenuItem();
            hideItem.Text = "Hide DWAS2 Window".t(lang);
            hideItem.Click += new EventHandler(delegate { HideWindow(); });

            WinForms.MenuItem closeItem = new WinForms.MenuItem();
            closeItem.Text = "Exit DWAS2".t(lang);
            closeItem.Click += new EventHandler(delegate { this.SafeExit(); });

            menu.MenuItems.Add(showItem);
            menu.MenuItems.Add(hideItem);
            menu.MenuItems.Add(closeItem);

            notifyIcon.ContextMenu = menu;

            // Event handlers
            notifyIcon.DoubleClick += notifyIcon_DoubleClick;

            // Update icon and show
            UpdateNotifyIcon();
            notifyIcon.Visible = true;
        }

        private void UpdateNotifyIcon()
        {
            notifyIcon.Icon = lastOrientation == Orientation.Landscape ? Properties.Resources.landscape_icon : Properties.Resources.portrait_icon;
        }

        private void ProcessConfig()
        {
            // first version, nothing to do
        }

        private void ApplyLanguageSelection()
        {
            this.Title = this.Title.t(lang);
        }

        private void ProcessExit()
        {
            config.lastWidth = (int)this.ActualWidth;
            config.lastHeight = (int)this.ActualHeight;
            config.SaveConfig();
            if(notifyIcon != null)
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
                notifyIcon = null;
            }
        }

        /*** Public Methods ***/

        public void HideWindow()
        {
            this.Hide();
        }

        public void ShowWindow()
        {
            if (!this.IsVisible)
            {
                this.Show();
            }
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
            this.Activate();
        }

        public void SafeExit()
        {
            ProcessExit();
            Environment.Exit(0);
        }

        public void Restart()
        {
            ProcessExit();
            System.Windows.Forms.Application.Restart();
        }

        /*** Event Handlers ***/

        private void ModernWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void ModernWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyLanguageSelection();
            // set to last width/height
            this.Height = config.lastHeight;
            this.Width = config.lastWidth;
            // set auto-hide
            if(shouldHideAtStartup)
            {
                this.Visibility = Visibility.Hidden;
            }
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ProcessExit();
        }

        void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if (!this.IsVisible || this.WindowState == WindowState.Minimized)
                this.ShowWindow();
            else
                this.HideWindow();
        }
    }
}
