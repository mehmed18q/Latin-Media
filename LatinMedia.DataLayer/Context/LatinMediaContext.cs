using System;
using System.Collections.Generic;
using System.Text;
using LatinMedia.DataLayer.Entities.Company;
using LatinMedia.DataLayer.Entities.Course;
using LatinMedia.DataLayer.Entities.Order;
using LatinMedia.DataLayer.Entities.Permissions;
using LatinMedia.DataLayer.Entities.Teacher;
using LatinMedia.DataLayer.Entities.User;
using LatinMedia.DataLayer.Entities.Wallet;
using Microsoft.EntityFrameworkCore;

namespace LatinMedia.DataLayer.Context
{
    public class LatinMediaContext : DbContext
    {
        public LatinMediaContext(DbContextOptions<LatinMediaContext> options) : base(options)
        {
            
        }

        #region User

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<UserDiscountCode> UserDiscountCodes { get; set; }
        #endregion

        #region Wallet

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletType> WalletTypes { get; set; }

        #endregion

        #region Permission

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        #endregion

        #region Course

        public DbSet<CourseGroup> CourseGroups { get; set; }
        public DbSet<CourseType> CourseTypes { get; set; }
        public DbSet<CourseLevel> CourseLevels { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        public DbSet<CourseComment> CourseComments { get; set; }

        #endregion

        #region Teachers

        public DbSet<Teacher> Teachers { get; set; }

        #endregion

        #region Company

        public DbSet<Company> Companies { get; set; }


        #endregion

        #region Order

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<Discount> Discounts { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasQueryFilter(u => !u.IsDelete); // IsDelete = false

            modelBuilder.Entity<Role>()
                .HasQueryFilter(r=> !r.IsDelete); // IsDelete = false

            modelBuilder.Entity<CourseGroup>()
                .HasQueryFilter(g => !g.IsDelete);

            modelBuilder.Entity<Course>()
                .HasQueryFilter(c => !c.IsDelete);

            modelBuilder.Entity<CourseComment>()
                .HasQueryFilter(c => !c.IsDelete);

            base.OnModelCreating(modelBuilder);
        }
    }
}
