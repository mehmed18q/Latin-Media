using System;
using System.Collections.Generic;
using System.Text;

namespace LatinMedia.Core.ViewModels
{
    public class ShowCourseForAdminViewModel
    {
        public int CourseId { get; set; }
        public string Company { get; set; }
        public string CourseType { get; set; }
        public string CourseFaTitle { get; set; }
        public string CourseLatinTitle { get; set; }
        public int CourseTime { get; set; }
        public int CoursePrice { get; set; }
        public string CourseImageName { get; set; }
        public bool IsSpecial { get; set; }
        public DateTime CreateDate { get; set; }
        public string GroupId { get; set; }
        public string SubGroupId { get; set; }
        public string SecondSubGroupId { get; set; }

    }


    public class ShowCourseListItemViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public int CourseTime { get; set; }
        public int CoursePrice { get; set; }
        public string CourseImage { get; set; }
        public string Teacher { get; set; }
        public string TeacherImage { get; set; }
        public string Company { get; set; }

    }


}
