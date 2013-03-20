using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Model
{
    /// <summary>
    /// 课程，设置每节课的开始和结束
    /// </summary>
    public class Lesson
    {
        public Lesson()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public int Num { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
