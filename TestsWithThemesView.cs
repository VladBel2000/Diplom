using Ophthalmology.Models;
using Ophthalmology.Views.TemplateCrud;
using System;
using System.Collections.Generic;
using System.ComponentModel;


namespace Ophthalmology.Views.Tests
{
    public partial class TestsWithThemesView :IModelView
    {
        public object GetModel() => _test;
        public void Init(object sourceModel) => _test = (Models.Test)sourceModel;

        private Test _test;

        public TestsWithThemesView()
        {
            this._test = null;
        }

        public TestsWithThemesView(Test test)
        {
            this._test = test;
        }

        [Browsable(false)]
        public long _QuizId
        {
            get
            {
                return _test.TestId;
            }
        }

        [DisplayName("Название")]
        public string _Name { get { return _test.Name; } set { _test.Name = value; } }

        [DisplayName("Тема")]
        public string _ThemeName => _test.Themes != null ? _test.Themes.Name : "";

        [Browsable(false)]
        [DisplayName("Показывать правильные ответы")]
        public bool _ShowRAns { get { return _test.ShowRAns; } set { _test.ShowRAns = value; } }

        [Browsable(false)]
        [DisplayName("Прерывать тест")]
        public long _InterCount { get { return _test.InterCount; } set { _test.InterCount = value; } }

        [Browsable(false)]
        [DisplayName("Количество неправильных ответов")]
        public bool _Interrupt { get { return _test.Interrupt; } set { _test.Interrupt = value; } }
    }
}

