using System;
using Ophthalmology.Views.TemplateCrud;
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
using System.Windows.Shapes;
using Ophthalmology.Models;
using Ophthalmology.Views.Tasks;
using System.ComponentModel;

namespace Ophthalmology.Views.Tests.EditTest.SelectTasks
{
    /// <summary>
    /// Логика взаимодействия для SelectTasksWindow.xaml
    /// </summary>
    public partial class SelectTasksWindow : Window
    {
        public OphthalmologyContext Context;
        public Func<OphthalmologyContext, IQueryable<Models.Task>> QueryForModels;
        public Test test;
        public List<Models.Task> selectedTasks = new();
        public SelectTasksWindow()
        {
            InitializeComponent();
        }

        public void OnLoad(object sender, RoutedEventArgs e)
        {
            List<TaskForDataGrid> tasksForDataGrid = new();
            foreach (var task in QueryForModels(Context).ToList())
            {
                tasksForDataGrid.Add(new TaskForDataGrid());
                tasksForDataGrid.Last().Init(task);
            };
            this.DataContext = tasksForDataGrid;
        }

        public void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public void SelectButtonClick(object sender, RoutedEventArgs e)
        {
            foreach(TaskForDataGrid tfdg in this.ItemsDataGrid.SelectedItems)
            {
                selectedTasks.Add((Models.Task) tfdg.GetModel());
            }
            this.DialogResult = true;
        }

        private void OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyDescriptor is PropertyDescriptor descriptor)
            {
                e.Cancel = !descriptor.IsBrowsable;
                e.Column.Header = descriptor.DisplayName ?? descriptor.Name;
                if (descriptor.Attributes[typeof(WidthAttribute)] is WidthAttribute widthAttr)
                    e.Column.Width = new DataGridLength(widthAttr.Width, DataGridLengthUnitType.Star);
                else
                    e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            }
        }
    }
}
