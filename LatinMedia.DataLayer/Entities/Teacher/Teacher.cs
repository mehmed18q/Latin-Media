using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LatinMedia.DataLayer.Entities.Teacher
{
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }

        [Display(Name = "نام فارسی مدرس")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string TeacherNameFa { get; set; }

        [Display(Name = "نام لاتین مدرس")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string TeacherNameEN { get; set; }

        [Display(Name = "درباره مدرس")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
     
        public string Description { get; set; }

        [Display(Name = "تصویر مدرس")]
        [MaxLength(200)]
        public string TeacherImageName { get; set; }

        [Display(Name = "حذف شده ؟")]
        public bool IsDelete { get; set; }

        #region Relations

        public List<Course.Course> Courses { get; set; }

        #endregion

    }
}
