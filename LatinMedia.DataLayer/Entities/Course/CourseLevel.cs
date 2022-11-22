using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LatinMedia.DataLayer.Entities.Course
{
   public class CourseLevel
    {
        [Key]
        public int LevelId { get; set; }

        [Display(Name = "سطح اموزش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string LevelTitle { get; set; }

        #region Relations

        public List<Course> Courses { get; set; }


        #endregion

    }
}
