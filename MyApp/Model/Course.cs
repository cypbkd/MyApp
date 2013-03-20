using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Model
{
    /// <summary>
    /// 课程，存放每一节课的详细数据
    /// </summary>
    public class Course 
    {
        public Course()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid LessonId { get; set; }

        public String Name { get; set; }
        public String Location { get; set; }
        public String TeacherName { get; set; }        
        public String Remark { get; set; }
        public List<DateTime> Dates { get; set; }
    }
}
