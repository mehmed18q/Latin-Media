using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using LatinMedia.DataLayer.Entities.Order;

namespace LatinMedia.DataLayer.Entities.Course
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        public int GroupId { get; set; }

        public int? SubGroupId { get; set; }

        public int? SecondSubGroupId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        public int LevelId { get; set; }

        [Required]
        public int TypeId { get; set; }

        [Required]
        public int TeacherId { get; set; }

        [Display(Name = "عنوان لاتین آموزش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(400, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string CourseLatinTitle { get; set; }

        [Display(Name = "عنوان فارسی آموزش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(400, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string CourseFaTitle { get; set; }

        [Display(Name = "حجم اموزش(مگابایت)")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int Volume { get; set; }

        [Display(Name = "مدت زمان(دقیقه)")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int CourseTime { get; set; }

        [Display(Name = "زیر نویس")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public bool IsSubTitle { get; set; }

        [Display(Name = "زبان اموزش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string Language { get; set; }

        [Display(Name = "توضیحات آموزش ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string CourseDescription { get; set; }

        [Display(Name = "تاریخ انتشار")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "مبلغ آموزش (تومان)")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int CoursePrice { get; set; }

        [Display(Name = "کلمات کلیدی")]
        [MaxLength(600)]
        public string Tags { get; set; }

        [Display(Name = "فایل آموزش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string CourseFileName { get; set; }

        [Display(Name = "تصویر آموزش")]
        [MaxLength(100)]
        public string CourseImageName { get; set; }

        [Display(Name = "دمو آموزش")]
        [MaxLength(500)]
        public string DemoFileName { get; set; }

        [Display(Name = "تعداد ویدئو")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int CountFiles { get; set; }

        [Display(Name = "محصول ویژه؟")]
        public bool IsSpecial { get; set; }


        [Display(Name = "حذف شده؟")]
        public bool IsDelete { get; set; }


        #region Relations

        [ForeignKey("GroupId")]
        public CourseGroup CourseGroup { get; set; }

        [ForeignKey("SubGroupId")]
        public CourseGroup SubGroup { get; set; }

        [ForeignKey("SecondSubGroupId")]
        public CourseGroup SecondSubGroup { get; set; }

        public Company.Company Company { get; set; }
        public CourseLevel CourseLevel { get; set; }
        public CourseType CourseType { get; set; }
        public Teacher.Teacher Teacher { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }

        public List<UserCourse> UserCourses { get; set; }

        public List<CourseComment> CourseComments { get; set; }


        #endregion

    }
}
