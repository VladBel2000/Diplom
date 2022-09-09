using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ophthalmology.Models;
using Ophthalmology.Views.Tests.EditTest;
using Ophthalmology.Views.TemplateCrud;

namespace Ophthalmology.Views.Tests.EditTest
{
    /// <summary>
    /// Логика взаимодействия для EditTasksPage.xaml
    /// </summary>
    public partial class EditTestWindow : CreateUpdateWindow<Models.Test>
    {
        private Dictionary<Button, UserControl> pages;
        

        public EditTestWindow()
        {
            InitializeComponent();
            this.Context = new();

        }

        protected override void Init()
        {
            LabelWindow.Content = IsCreate ? "Создание теста" : "Изменение теста";

            pages = new Dictionary<Button, UserControl>
            {
                {TestIdentificationButton, new IdentTestPage{Entity = Entity, Context = Context}},
                {SituationTasksButton, new SituationTaskPage(Context, Entity)},
                {TestSettingsButton, new SettingsPage()} 
            };
            SelectPage(TestIdentificationButton);

            AddEntityValidator(t =>
            {
                if (t.Themes != null)
                    return true;
                else
                {
                    if (((IdentTestPage)this.pages[TestIdentificationButton]).ThemeComboBox.SelectedItem == null)
                    {
                        // Пишем свою тему
                        if(((IdentTestPage)this.pages[TestIdentificationButton]).ThemeComboBox.Text.Trim().Length > 0)
                        {
                            t.Themes = new Theme { Name = ((IdentTestPage)this.pages[TestIdentificationButton]).ThemeComboBox.Text.Trim() };
                            return true;
                        }
                        else
                        {
                            // Если тема из пробелов
                            if (((IdentTestPage)this.pages[TestIdentificationButton]).ThemeComboBox.Text.Length > 0)
                                return false;
                            else
                                return true;

                        }
                    }

                }
                return false;
            }, "Не выбрана тема");

            AddEntityValidator(t => t.Name != null && t.Name.Trim().Length > 0, "Не введено название теста");
        }

        private void SelectPage(Button selectedBtn)
        {
            selectedBtn.Style = FindResource("NavigationButtonsActive") as Style;
            foreach (var (btn, page) in pages)
            {
                if (btn == selectedBtn)
                    PageContent.Content = page;
                else
                    btn.Style = FindResource("NavigationButtons") as Style;
            }
        }

        private void NavigationButtonClick(object sender, RoutedEventArgs e)
        {
            SelectPage((Button)sender);
        }
        
    }
}
