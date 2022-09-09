using Ophthalmology.Views.Tests.EditTest;
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
using Ophthalmology.Views.Tests.EditTest.TemplateCrud;

using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Ophthalmology.Views.Tests.EditTest.SelectTasks;
using Ophthalmology.Views.Tasks.EditTasks;
using Ophthalmology.Views.Tasks;
using Ophthalmology.Models;

namespace Ophthalmology.Views.Tests
{
    /// <summary>
    /// Логика взаимодействия для SituationTaskPage.xaml
    /// </summary>
    public partial class SituationTaskPage
    {
        public Test test;
        public OphthalmologyContext Context;

        public SituationTaskPage(OphthalmologyContext Context, Test test)
        {
            InitializeComponent();
            TasksInTestCrudPage.CrudLogic = new TasksInTestCrudPageLogic<Models.Task, EditTasksWindow, TaskForDataGrid>
            {
                HasCloneLogic = false,
                Context = Context,
                test = test,
                QueryForModels = db => db.Tasks.Where(task => test.Tasks.Contains(task)).Include(task => task.Diagnosis).Include(task => task.Symptoms)
            };
        }
    }
}
