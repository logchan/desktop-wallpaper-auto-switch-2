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

using DWAS2.Utilities;
using MultiLang;

namespace DWAS2.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        MainWindow wnd = null;
        bool langloaded = false;

        public Home()
        {
            InitializeComponent();
        }

        private void InitializeLanguageSelectComboBox()
        {
            if (langloaded || wnd == null) return;
            int index = 0;
            for(int i = 0; i < wnd.languages.Count; ++i)
            {
                var mle = wnd.languages[i];
                languageSelectBox.Items.Add(mle.Language);
                if (mle.Language == wnd.lang.Language) index = i;
            }
            languageSelectBox.SelectedIndex = index;
            langloaded = true;
        }

        /*** Event Handlers ***/

        private void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            DWAS2Helper.SetUserControlLanguage(this);
            wnd = DWAS2Helper.GetUserControlMainWindow(this);
            InitializeLanguageSelectComboBox();
            wnd.OrientationChanged += MainWindow_Orientation_Changed;
            // set autorun state
            autorunBox.IsChecked = DWAS2Interop.GetAutorunState();
        }

        private void MainWindow_Orientation_Changed(object sender, OrientationChangedEventArgs e)
        {
            if (wnd != null)
            {
                currentOrientationBlock.Text = wnd.lastOrientation.ToString().t(wnd.lang);
                if (wnd.lastOrientation == Orientation.Landscape)
                    currentOrientationBlock.Foreground = Brushes.Green;
                else
                    currentOrientationBlock.Foreground = Brushes.Red;
            }
        }

        private void languageSelectBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!langloaded || wnd==null) return;
            ComboBox c = (ComboBox)sender;
            string langName = c.SelectedItem.ToString();
            foreach(var mle in wnd.languages)
            {
                if(mle.Language == langName)
                {
                    wnd.config.language = mle.Language;
                    wnd.config.SaveConfig();
                    MessageBox.Show("DWAS2 will now restart to change the language.".t(mle), "DWAS2", MessageBoxButton.OK, MessageBoxImage.Information);
                    wnd.Restart();
                }
            }
        }

        private void hideWindowButton_Click(object sender, RoutedEventArgs e)
        {
            if (wnd != null)
                wnd.HideWindow();
        }

        private void fixDetectionButton_Click(object sender, RoutedEventArgs e)
        {
            if(wnd != null)
            {
                wnd.config.reverse = !wnd.config.reverse;
                wnd.config.SaveConfig();
                wnd.UpdateDetection();
            }
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            if (wnd != null)
                wnd.SafeExit();
        }

        private void autorunBox_Click(object sender, RoutedEventArgs e)
        {
            if (wnd == null) return;
            bool result = false;
            if(autorunBox.IsChecked == true)
            {
                result = DWAS2Interop.SetAutorunState(true);
                if (!result) autorunBox.IsChecked = false;
            }
            else
            {
                result = DWAS2Interop.SetAutorunState(false);
                if (!result) autorunBox.IsChecked = true;
            }
            if(result)
            {
                MessageBox.Show("Operation succeeded.".t(wnd.lang), "DWAS2", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Operation failed.".t(wnd.lang), "DWAS2", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
