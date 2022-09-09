using Ophthalmology.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Ophthalmology.Views.TemplateCrud;

namespace Ophthalmology.Views.SetQuiz
{
    class StudentForDataGrid 
    {
        public Student student;
        public Test now_test;

        public StudentForDataGrid(Student p_student)
        {
            this.student = p_student;
        }
        public StudentForDataGrid(Student p_student, Test p_now_test)
        {
            this.student = p_student;
            this.now_test = p_now_test;
        }

        [Browsable(false)]
        public long StudentId
        {
            get { return student.StudentId; }
        }

        [DisplayName("Фамилия")]
        public string Secondname
        {
            get { return student.SecondName; }
            set { student.SecondName = value; }
        }

        [DisplayName("Имя")]
        public string Firstname
        {
            get { return student.FirstName; }
            set { student.FirstName = value; }
        }

        [DisplayName("Отчество")]
        public string Middlename
        {
            get { return student.MiddleName; }
            set { student.MiddleName = value; }
        }

        [DisplayName("Группа")]
        public string Group
        {
            get { return student.Group != null ? student.Group.Name : ""; }
        }

        [DisplayName("Результат")]
        public string Result
        {
            get { return student.StudentSetTestResults.ToList().Find(st => st.SettingTest == this.now_test.SettingTests.Last()) != null ? Convert.ToString(student.StudentSetTestResults.ToList().Find(st => st.SettingTest == this.now_test.SettingTests.Last()).Result) : ""; }
        }
    }
}
