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
using System.Diagnostics;

namespace DWAS2.Pages
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
        }

        private void AboutPage_Loaded(object sender, RoutedEventArgs e)
        {
            DWAS2Helper.SetUserControlLanguage(this);
        }

        private void LinkTextblock_Clicked(object sender, RoutedEventArgs e)
        {
            if(sender is TextBlock)
            {
                Process.Start(((TextBlock)sender).Text);
            }
        }
    }
}
