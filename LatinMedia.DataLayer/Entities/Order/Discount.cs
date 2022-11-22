using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using LatinMedia.DataLayer.Entities.User;

namespace LatinMedia.DataLayer.Entities.Order
{
    public class Discount
    {
        [Key]
        public int DiscountId { get; set; }

        [Display(Name = "کد تخفیف")]
        [Required(ErrorMessage = "لطفا کد تخفیف را وارد نمایید")]
        [MaxLength(150)]
        public string DiscountCode { get; set; }

        [Display(Name = "میزان درصد تخفیف")]
        [Required]
        [Range(typeof(int),"5","70",ErrorMessage = "محدوده درصد تخفیف از {1} تا {2} درصد می باشد")]
        public int DiscountPercent { get; set; }

        public int? UsableCount { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        #region Relations

        public List<UserDiscountCode> UserDiscountCodes { get; set; }

        #endregion

    }
}
