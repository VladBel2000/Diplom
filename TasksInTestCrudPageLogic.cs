using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Ophthalmology.Models;
using Ophthalmology.Views.Tasks.EditTasks;
using Ophthalmology.Views.TemplateCrud;
using Ophthalmology.Views.Tests.EditTest.SelectTasks;

namespace Ophthalmology.Views.Tests.EditTest.TemplateCrud
{
    public class TasksInTestCrudPageLogic<TModel, TCreateUpdateWindow, TModelView> : ICrudControlLogic
        where TModel : Models.Task, new()
        where TCreateUpdateWindow : CreateUpdateWindow<TModel>, new()
        where TModelView : class, IModelView, new()
    {
        #region Public Methods
        public OphthalmologyContext Context { get; set; }
        public Test test { get; set; }

        /// <summary>
        /// Func for getting query for models, that will placed in DataGrid. Accept Context
        /// </summary>
        public Func<OphthalmologyContext, IQueryable<TModel>> QueryForModels { get; set; }
        
        /// <summary>
        /// Indicates that page has Clone object logic
        /// </summary>
        public bool HasCloneLogic { get; set; }

        public TasksInTestCrudPageLogic()
        {
            HasCloneLogic = false;
        }

        public void OnCreateEntity()
        {
            OphthalmologyContext contextForCreate = new();

            Models.Task task = new();
            var window = new EditTasksWindow
            {
                Entity = task,
                Context = contextForCreate,
                IsCreate = true
            };
            window.ShowDialog();
            if (window.DialogResult is null or false)
            {
                return;
            }
            else
            {
                try
                {
                    contextForCreate.SaveChanges();
                    long task_id = task.TaskId;
                    test.Tasks.Add(Context.Tasks.Find(task_id));
                }
                catch
                {
                    MessageBox.Show("Не удалось сохранить новый элемент", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        
        public void OnAddEntity()
        {
            var window = new SelectTasksWindow() { 
                test = test,
                QueryForModels = db => db.Tasks.Where(task => !test.Tasks.Contains(task)).Include(task => task.Diagnosis).Include(task => task.Symptoms),
                Context = Context};

            window.ShowDialog();
            if (window.DialogResult is null or false)
            {
                return;
            }
            else
            {
                foreach (Models.Task task in window.selectedTasks)
                {
                    test.Tasks.Add(Context.Tasks.Find(task.TaskId));
                };
            }
        }

        public void OnEditEntity(object modelView)
        {
            var model = GetModelFromModelView(modelView);
            
            OphthalmologyContext contextForEdit = new();
            var task = (Models.Task)model;

            var window = new EditTasksWindow
            {
                Entity = contextForEdit.Tasks.Include(task => task.Diagnosis).Include(task => task.Symptoms).First(t => t.TaskId == task.TaskId),
                Context = contextForEdit,
                IsCreate = false
            };
            window.ShowDialog();

            if (window.DialogResult is true)
            {
                try
                {
                    contextForEdit.SaveChanges();
                    Context.Entry(model).Reload();
                    Context.SaveChanges();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Не удалось изменить элемент", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        public void OnCloneEntity(object modelView)
        {
            var model = GetModelFromModelView(modelView) as ICloneable;
            Context.Add(model.Clone());
        }

        public void OnDeleteEntities(IList modelViews)
        {
            foreach (var modelView in modelViews)
            {
                var model = GetModelFromModelView(modelView);
                test.Tasks.Remove(model);
            }
        }

        public IEnumerable GetDataGridItems()
        {
            var models = QueryForModels(Context).ToList();
            ObservableCollection<TModelView> modelViews = new();
            foreach (var model in models)
            {
                var modelView = new TModelView();
                modelView.Init(model);
                modelViews.Add(modelView);
            }
            return modelViews;
        }

        #endregion

        #region Private

        private TModel GetModelFromModelView(object modelView) => (TModel) ((TModelView) modelView).GetModel();

        #endregion
    }

}