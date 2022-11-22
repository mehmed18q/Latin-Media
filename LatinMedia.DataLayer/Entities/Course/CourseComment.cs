using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LatinMedia.DataLayer.Entities.Course
{
   public class CourseComment
    {
        [Key]
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Comment { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsDelete { get; set; }
        public bool IsReadAdmin { get; set; }


        #region Relations

        public Course Course { get; set; }
        public User.User User { get; set; }


        #endregion
    }
}
