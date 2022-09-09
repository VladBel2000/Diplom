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
using FilterDataGrid;
using Microsoft.EntityFrameworkCore;
using Ophthalmology.Views.SetQuiz;
using Ophthalmology.Views.TemplateCrud;
using System.ComponentModel;
using System.Collections;
using Ophthalmology.Views.SetQuiz.SelectStudent;

namespace Ophthalmology.Views
{
    /// <summary>
    /// Логика взаимодействия для ParticipantsPage.xaml
    /// </summary>
 
    public partial class SetQuizPage : UserControl
    {
        public SetTestForSetQuiz set_test { get; set; }
        List<StudentForDataGrid> list_student_in_test;
        Test now_test;
        Boolean first_entrance;

        public OphthalmologyContext Context { get; set; }

        public SetQuizPage()
        {
            first_entrance = true;
            Context = new();


            if (Context.Tests.Count() == 0)
            { 
                InitializeComponent();
                DurationMinute.IsReadOnly = true;
                Picker_start.IsReadOnly = true;
                Picker_stop.IsReadOnly = true;
                StartButton.IsEnabled = false;
                StopButton.IsEnabled = false;
                AddButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
                MessageBox.Show("Нет тестов для настроек.", "Ошибка");
            }
            else
            {
                // Если нет настройки на тестирование, то создаём настройку с дефолтными значениями
                if (Context.SettingTests.Count() == 0)
                {
                    SettingTest help_ST = new SettingTest();
                    help_ST.TestId = Context.Tests.First().TestId;
                    help_ST.DurationMinute = 30;
                    help_ST.StartTime = DateTime.Now;
                    help_ST.StopTime = DateTime.Now.AddDays(1);
                    Context.Add(help_ST);
                    Context.SaveChanges();
                }

                // Считываем данные о настройке на тестирование
                var set_test_orig = Context.SettingTests.ToList().Last();
                Context.Entry(set_test_orig).Reference(s => s.Test).Load();
                Context.Entry(set_test_orig).Collection(s => s.StudentSetTestResults).Query().Include(sstr => sstr.Student).Load();
                now_test = set_test_orig.Test;

                set_test = new SetTestForSetQuiz(set_test_orig);
                this.DataContext = set_test;

                InitializeComponent();
                InitializeComponent();

                TestComboBox.ItemsSource = Context.Tests.ToList();
                TestComboBox.SelectedItem = now_test;
                Update_page();
            }
        }
        
        // Обновление страницы
        private void Update_page()
        {
            this.DataContext = Context.SettingTests.ToList().Where(s => s.TestId == now_test.TestId).Last();
            SettingTest now_set_test = Context.SettingTests.ToList().Where(s => s.TestId == now_test.TestId).Last();
            set_test = new SetTestForSetQuiz(now_set_test);
            var list = Context.StudentSetTestResults.Where(t => t.SettingTest == now_set_test).Include(s => s.Student).ToList();
            list_student_in_test = new();
            foreach (var rez in list)
            {
                if(rez.Student.GroupId != null)
                    rez.Student.Group = Context.StGroups.Where(s => s.GroupId == rez.Student.GroupId).First();      // Выбирается не то значение
                list_student_in_test.Add(new StudentForDataGrid(rez.Student, now_test));
            }
            this.ItemsDataGrid.ItemsSource = list_student_in_test;
        }


        // Сохранение тестирования
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверки корректности вводы данных
            try
            {
                var duration = this.DurationMinute.Text.Trim();

                // Проверка длительности
                if (duration.Length == 0)
                {
                    MessageBox.Show("Не задана продолжительность теста", "Ошибка");
                    return;
                }
                if (duration[0] == '0')
                {
                    MessageBox.Show("Продолжительность теста начинается с '0'", "Ошибка");
                    this.DurationMinute.Text = "";
                    return;
                }             
                if (!Int32.TryParse(duration, out int rez))
                {
                    MessageBox.Show("Поле \"Продолжительность\" должно состоять только из цифр", "Ошибка");
                    this.DurationMinute.Text = "";
                    return;
                }
                if (duration.Length > 5)
                {
                    MessageBox.Show("Слишком большая продолжительность теста", "Ошибка");
                    this.DurationMinute.Text = "";
                    return;
                }
                if (Picker_start.Value >= Picker_stop.Value)
                {
                    MessageBox.Show("Время закрытия теста меньше, чем время его открытия", "Ошибка");
                    return;
                }
                Context.SaveChanges();
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось сохранить настройку на тестирование");
                return;
            }
            MessageBox.Show("Настройка на тестирование успешно сохранена.", "Сохранение");
            Update_page();
        }

        // Сбросить настройку
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
           // Если никто не проходил тест, то настройка удаляется
           var set_test_all = Context.StudentSetTestResults.Where(st => st.SettingTest.Test == now_test).ToList();

           Boolean have_rez = false;
           for(int i = 0; i < set_test_all.Count(); i++)
           {
               //Context.SettingTests.ToList().Find(st => st.Test == now_test)
               if (set_test_all[i].SettingTest == Context.SettingTests.ToList().Where(st => st.Test == now_test).Last() && set_test_all[i].BeginTime != null)
                   have_rez = true;
           }
           if(!have_rez)
           {
               var set_test_del = Context.SettingTests.ToList().Find(st => st.Test == now_test);
               Context.SettingTests.Remove(set_test_del);
           }


           // Если студент не проходил тест, то его запись удаляется
           for (int i = 0; i < set_test_all.Count(); i++)
           {
               if(set_test_all[i].BeginTime == null)
               {
                   var set_test_rez_del = set_test_all[i];
                   Context.StudentSetTestResults.Remove(set_test_rez_del);
               }
           }

            // Создаём новую настройку на тестирование
            SettingTest help_ST = new SettingTest();
            help_ST.TestId = now_test.TestId;
            help_ST.DurationMinute = 30;
            help_ST.StartTime = DateTime.Now;
            help_ST.StopTime = DateTime.Now.AddDays(1);
            Context.SettingTests.Add(help_ST);
            Context.SaveChanges();

            var set_test_orig = Context.SettingTests.ToList().Where(st => st.Test == now_test).Last();
            Context.Entry(set_test_orig).Reference(s => s.Test).Load();
            Context.Entry(set_test_orig).Collection(s => s.StudentSetTestResults).Query().Include(sstr => sstr.Student).Load();
            set_test = new SetTestForSetQuiz(set_test_orig);
            this.DataContext = set_test;
            Update_page();
        }

        // Добавление студентов
        private void AddButton_Click(object sender, RoutedEventArgs e)
        { 
            // Открываем окно добавления студентов
            var window = new SelectStudentWindow() { Context = Context, settingTest = set_test.settingTest};
            window.ShowDialog();
            Update_page();
        }

        // Удаление студентов
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = this.ItemsDataGrid.SelectedItems;
            try
            {
                if (selected.Count == 0)
                {
                    MessageBox.Show(" Не удалось удалить участников тестирования\n Участники тестирования для удаления не выбраны", "Ошибка");
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show(" Удалить " + selected.Count.ToString() + " участника(-ов) тестирования?", "Удаление", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        Boolean cant_del = false;
                        foreach (StudentForDataGrid student in selected)
                        {
                            StudentSetTestResult SSTR_Del = Context.StudentSetTestResults.ToList().Find(s => s.StudentId == student.StudentId && s.SettingTest == Context.SettingTests.ToList().Where(st => st.Test == now_test).Last());
                            if(SSTR_Del.Result == null)
                                Context.StudentSetTestResults.Remove(SSTR_Del);
                            else
                            {
                                cant_del = true;
                            }
                        }
                        Context.SaveChanges();
                        Update_page();
                        if (cant_del)
                            MessageBox.Show(" Не все выбранные участники были удалены\n Нельзя удалить студента, который уже прошёл тест", "Ошибка");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show(" Не удалось удалить участников тестирования\n", "Ошибка");
            }
        }

        private void TestComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!first_entrance)
            {
                // Ищем выбранный тест в БД
                Test new_test = (Test)TestComboBox.SelectedItem;              
                SettingTest ST_now = Context.SettingTests.ToList().Find(s => s.TestId == new_test.TestId);

                // Если нет настройки на выбранынй тест, то она создаётся по дефолту
                if (ST_now == null)
                {
                    SettingTest help_ST = new SettingTest();
                    help_ST.TestId = new_test.TestId;
                    help_ST.DurationMinute = 30;
                    help_ST.StartTime = DateTime.Now;
                    help_ST.StopTime = DateTime.Now.AddDays(1);
                    Context.Add(help_ST);
                    Context.SaveChanges();
                }
                now_test = new_test;
                // Обновление страницы
                Update_page();
            }
            first_entrance = false;
        }
    }
}
