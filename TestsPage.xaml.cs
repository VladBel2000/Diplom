using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Ophthalmology.Models;
using Ophthalmology.Views.TemplateCrud;
using Ophthalmology.Views.Tests.EditTest;

namespace Ophthalmology.Views.Tests
{
    /// <summary>
    /// Interaction logic for TestsPage.xaml
    /// </summary>
    public partial class TestsPage : UserControl
    {
        public TestsPage()
        {
            InitializeComponent();
            TestCrudPage.CrudLogic = new CrudPageLogic<Models.Test, EditTestWindow, TestsWithThemesView>
            {
                HasCloneLogic = true,
                QueryForModels = db => db.Tests.Include(t => t.Themes).Include(t => t.Tasks)
            };
        }
    }
}
