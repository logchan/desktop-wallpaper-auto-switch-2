using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace DWAS2.Components
{
    /// <summary>
    /// Interaction logic for PicPicker.xaml
    /// </summary>
    public partial class PicPicker : UserControl
    {
        public PicPicker()
        {
            InitializeComponent();
        }

        /*** Events ***/

        /// <summary>
        /// The event raised when the picture changed
        /// </summary>
        public event EventHandler<PicChangedEventArgs> PicChanged;

        public virtual void PicChangedEvent(PicChangedEventArgs e)
        {
            EventHandler<PicChangedEventArgs> handler = PicChanged;
            if(handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// The event raised when picture position changed
        /// </summary>
        public event EventHandler<PicPosChangedEventArgs> PicPosChanged;

        public virtual void PicPosChangedEvent(PicPosChangedEventArgs e)
        {
            EventHandler<PicPosChangedEventArgs> handler = PicPosChanged;
            if(handler != null)
            {
                handler(this, e);
            }
        }

        /*** Dependency Properties ***/

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(PicPicker), new PropertyMetadata(""));
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register("TitleForeground", typeof(SolidColorBrush), typeof(PicPicker), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        public SolidColorBrush TitleForeground
        {
            get { return (SolidColorBrush)GetValue(TitleForegroundProperty); }
            set { SetValue(TitleForegroundProperty, value); }
        }

        public static readonly DependencyProperty PicPathProperty = DependencyProperty.Register("PicPath", typeof(string), typeof(PicPicker), new PropertyMetadata(null,new PropertyChangedCallback(PicPathChangedCallback)));
        public string PicPath
        {
            get { return (string)GetValue(PicPathProperty); }
            set { SetValue(PicPathProperty, value); }
        }

        public static readonly DependencyProperty PisPosEnabledProperty = DependencyProperty.Register("PicPosEnabled", typeof(bool), typeof(PicPicker), new PropertyMetadata(true));
        public bool PicPosEnabled
        {
            get { return (bool)GetValue(PisPosEnabledProperty); }
            set { SetValue(PisPosEnabledProperty, value); }
        }

        public static readonly DependencyProperty SelectedPicPosProperty = DependencyProperty.Register("SelectedPicPos", typeof(PicPos), typeof(PicPicker), new PropertyMetadata(PicPos.Fill));
        public PicPos SelectedPicPos
        {
            get { return (PicPos)GetValue(SelectedPicPosProperty); }
            set { SetValue(SelectedPicPosProperty, value); }
        }

        /*** Methods ***/
        public void SetPicPosButtonText()
        {
            changePicPosButtonText.Text = SelectedPicPos.ToString();
        }

        public static void PicPathChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ((PicPicker)d).picPathTextBlock.Text = (string)e.NewValue;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /*** Event Handlers ***/

        private void browsePicButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp";
            if(File.Exists(PicPath))
            {
                dialog.InitialDirectory = new FileInfo(PicPath).Directory.FullName;
            }
            else
            {
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }
            if(dialog.ShowDialog() == true)
            {
                PicPath = dialog.FileName;
                PicChangedEvent(new PicChangedEventArgs(PicPath));
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (File.Exists(PicPath))
            {
                System.Diagnostics.Process.Start(PicPath);
            }
        }

        private void changePicPosButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedPicPos = WPFHelper.GetNextEnum<PicPos>(SelectedPicPos);
            SetPicPosButtonText();
            PicPosChangedEvent(new PicPosChangedEventArgs(SelectedPicPos));
        }

        private void DWAS2PicPicker_Loaded(object sender, RoutedEventArgs e)
        {
            SetPicPosButtonText();
        }
    }
}
