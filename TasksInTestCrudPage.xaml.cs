using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ophthalmology.Views.TemplateCrud;

namespace Ophthalmology.Views.Tests.EditTest.TemplateCrud
{
    /// <summary>
    /// Interaction logic for TemplateCRUDView.xaml
    /// </summary>
    public partial class TasksInTestCrudPage : UserControl
    {
        public ICrudControlLogic CrudLogic { get; set; }

        public TasksInTestCrudPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            _dataGridItemsBackgroundLoader = new BackgroundWorker();
            _dataGridItemsBackgroundLoader.DoWork += DataGridItemsBackgroundLoaderOnDoWork;
            _dataGridItemsBackgroundLoader.RunWorkerCompleted += DataGridItemsBackgroundLoaderOnRunWorkerCompleted;
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!CrudLogic.HasCloneLogic)
                CloneButton.Visibility = Visibility.Collapsed;
            LoadData();
        }

        #region Async Load DataGridItems

        private void DataGridItemsBackgroundLoaderOnDoWork(object? sender, DoWorkEventArgs e)
        {
            _dataGridItems = CrudLogic.GetDataGridItems();
        }

        private void DataGridItemsBackgroundLoaderOnRunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            ContentControl.Content = _controlContent;
            ItemsDataGrid.ItemsSource = _dataGridItems;
        }

        private void LoadData()
        {
            _controlContent = ContentControl.Content;
            ContentControl.Content = new WaitControl();
            _dataGridItemsBackgroundLoader.RunWorkerAsync();
        }
        private object _controlContent;
        private readonly BackgroundWorker _dataGridItemsBackgroundLoader;
        private IEnumerable _dataGridItems;

        #endregion

        #region DataGrid/Buttons Callbacks

        private void CreateButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                CrudLogic.OnCreateEntity();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось создать элемент.", "Ошибка");
            }
        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckIsSelectedZero() || CheckIsSelectedMoreOne()) return;
                CrudLogic.OnEditEntity(ItemsDataGrid.SelectedItem);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось изменить элемент.", "Ошибка");
            }
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckIsSelectedZero() || !AskForDelete()) return;
                CrudLogic.OnDeleteEntities(ItemsDataGrid.SelectedItems);
                LoadData();
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось удалить элементы.", "Ошибка");
            }
        }

        private void CloneButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckIsSelectedZero() || CheckIsSelectedMoreOne()) return;
                CrudLogic.OnCloneEntity(ItemsDataGrid.SelectedItem);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось дублировать элемент.", "Ошибка");
            }
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                CrudLogic.OnAddEntity();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось изменить элемент.", "Ошибка");
            }
        }

        private bool CheckIsSelectedZero()
        {
            if (ItemsDataGrid.SelectedItems.Count == 0)
                MessageBox.Show("Выберите хотя бы один элемент!", "Ошибка");
            return ItemsDataGrid.SelectedItems.Count == 0;
        }

        private bool CheckIsSelectedMoreOne()
        {
            if (ItemsDataGrid.SelectedItems.Count > 1)
                MessageBox.Show("Выбрано несколько элементов! Выберите единственный элемент.", "Ошибка");
            return ItemsDataGrid.SelectedItems.Count > 1;
        }

        private bool AskForDelete()
        {
            var ans = MessageBox.Show(
                $"Выбрано элементов для удаления: {ItemsDataGrid.SelectedItems.Count}. Продолжить?",
                "Предупреждение",
                MessageBoxButton.OKCancel
            );
            return ans == MessageBoxResult.OK;
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

        private void CellMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EditButtonClick(sender, e);
        }


        #endregion

    }
}
