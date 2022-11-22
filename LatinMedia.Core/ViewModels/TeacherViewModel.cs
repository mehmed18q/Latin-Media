using System;
using System.Collections.Generic;
using System.Text;

namespace LatinMedia.Core.ViewModels
{
   public class ShowAllTeachersViewModel
    {
        public int TeacherId { get; set; }
        public string TeacherNameFa { get; set; }
        public string TeacherNameEN { get; set; }
        public string Description { get; set; }
        public string TeacherImageName { get; set; }
        public int CourseCount { get; set; }
    }
}
