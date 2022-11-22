using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LatinMedia.DataLayer.Entities.Course
{
    public class CourseType
    {
        [Key]
        public int TypeId { get; set; }

        [Display(Name = "نوع اموزش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string TypeTitle { get; set; }

        #region Relations

        public List<Course> Courses { get; set; }

        #endregion
    }
}
