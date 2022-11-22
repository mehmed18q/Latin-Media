using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LatinMedia.DataLayer.Entities.Order
{
    public class OrderDetail
    {
        [Key]
        public int DetailId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int CoursePrice { get; set; }

        [Required]
        public int OrderCount { get; set; }

        #region Relation

        public Course.Course Course { get; set; }
        public Order Order { get; set; }


        #endregion
    }
}
