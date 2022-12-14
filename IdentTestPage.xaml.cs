using Ophthalmology.Models;
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
    /// Логика взаимодействия для IdentTestPage.xaml
    /// </summary>
    public partial class IdentTestPage
    {
        public Models.Test Entity;
        public OphthalmologyContext Context;

        public IdentTestPage()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ThemeComboBox.ItemsSource = Context.Themes.ToList();
        }
    }
}
