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

namespace Ophthalmology.Views.Tests
{
    /// <summary>
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            this.Loaded += have_interruption_Click;
        }

        private void have_interruption_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)!have_interruption.IsChecked)
                Count_error.Visibility = Visibility.Hidden;
            else
                Count_error.Visibility = Visibility.Visible;
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CheckBox_show_answer.IsChecked = !CheckBox_show_answer.IsChecked;
        }

        private void Label_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            have_interruption.IsChecked = !have_interruption.IsChecked;
            if ((bool)!have_interruption.IsChecked)
                Count_error.Visibility = Visibility.Hidden;
            else
                Count_error.Visibility = Visibility.Visible;
        }
    }
}
