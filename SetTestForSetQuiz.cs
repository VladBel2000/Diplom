using Ophthalmology.Models;
using Ophthalmology.Views.TemplateCrud;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Ophthalmology.Views.SetQuiz
{
    public class SetTestForSetQuiz
    {

        private SettingTest setting_test;

        public SettingTest settingTest { get { return setting_test; } }

        public SetTestForSetQuiz(SettingTest p_setting_test)
        {
            this.setting_test = p_setting_test;
        }

        [Browsable(false)]
        public long? SettingTestId
        {
            get
            {
                return setting_test.TestId;
            }
        }

        [DisplayName("Время открытия")]
        public DateTime? StartTime 
        { 
            get { if (setting_test.StartTime != null)  return setting_test.StartTime; else return (setting_test.StartTime = DateTime.Now); } 
            set {  setting_test.StartTime = value; } 
        } 

        [DisplayName("Время закрытия")]
        public DateTime? StopTime 
        { 
            get { if (setting_test.StopTime != null) return setting_test.StopTime; else return (setting_test.StopTime = DateTime.Now.AddDays(1)); }
            set { setting_test.StopTime = value; } 
        }

        [DisplayName("Длительность")]
        public long? DurationMinute { 
            get { if (setting_test.DurationMinute != null) return setting_test.DurationMinute; else return 30; } 
            set { setting_test.DurationMinute = value; } 
        }

        [DisplayName("Название тестирования")]
        public Test Test
        {
            get => setting_test.Test;
            set => setting_test.Test = value;
        }

        ////
    }
}