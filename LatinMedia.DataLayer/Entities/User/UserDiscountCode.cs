using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using LatinMedia.DataLayer.Entities.Order;

namespace LatinMedia.DataLayer.Entities.User
{
   public class UserDiscountCode
    {
        [Key]
        public int UD_Id { get; set; }
        public int UserId { get; set; }
        public int DiscountId { get; set; }

        #region Relations

        public Discount Discount { get; set; }
        public User User { get; set; }


        #endregion
    }
}
