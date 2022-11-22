using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LatinMedia.DataLayer.Entities.Company
{
   public class Company
    {
        [Key]
        public int CompanyId { get; set; }

        [Display(Name = "نام شرکت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string CompanyTitle { get; set; }

        [Display(Name = "تصویر شرکت")]
        [MaxLength(100)]
        public string CompanyImageName { get; set; }

        [Display(Name = "حذف شده؟")]
        public bool IsDelete { get; set; }

        #region Relations

        public List<Course.Course> Courses { get; set; }

        #endregion
    }
}
