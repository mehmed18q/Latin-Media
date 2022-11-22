using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using LatinMedia.Core.Convertors;
using LatinMedia.Core.Genertors;
using LatinMedia.Core.Security;
using LatinMedia.Core.Services.Interfaces;
using LatinMedia.Core.ViewModels;
using LatinMedia.DataLayer.Context;
using LatinMedia.DataLayer.Entities.Company;
using LatinMedia.DataLayer.Entities.Course;
using LatinMedia.DataLayer.Entities.Teacher;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LatinMedia.Core.Services
{
    public class CourseService : ICourseService
    {
        private LatinMediaContext _context;
        private IHostingEnvironment _environment;

        public CourseService(LatinMediaContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public List<CourseGroup> GetAllGroups()
        {
            return _context.CourseGroups.Include(g=> g.CourseGroups).ToList();
        }

        public List<SelectListItem> GetGroupsForManageCourse()
        {
            return _context.CourseGroups.Where(g => g.ParentId == null)
                .Select(g => new SelectListItem()
                {
                    Text = g.GroupTitle,
                    Value = g.GroupId.ToString()
                }).ToList();
        }

        public List<SelectListItem> GetSubGroupsForManageCourse(int groupId)
        {
            return _context.CourseGroups.Where(g => g.ParentId == groupId)
                .Select(g => new SelectListItem()
                {
                    Text = g.GroupTitle,
                    Value = g.GroupId.ToString()
                }).ToList();
        }

        public List<SelectListItem> GetSecondSubGroupsForManageCourse(int subGroupId)
        {
            return _context.CourseGroups.Where(g => g.ParentId == subGroupId)
                .Select(g => new SelectListItem()
                {
                    Text = g.GroupTitle,
                    Value = g.GroupId.ToString()
                }).ToList();
        }

        public void AddCourseGroup(CourseGroup @group)
        {
            _context.CourseGroups.Add(group);
            _context.SaveChanges();
        }

        public void UpdateCourseGroup(CourseGroup @group)
        {
            _context.CourseGroups.Update(group);
            _context.SaveChanges();
        }

        public CourseGroup GetGroupById(int groupId)
        {
            return _context.CourseGroups.Find(groupId);
        }

        public List<Teacher> GetAllTeachers()
        {
            return _context.Teachers.Include(t=> t.Courses).ToList();
        }

        public List<SelectListItem> GetTeachersForManageCourse()
        {
            return _context.Teachers.Select(t => new SelectListItem()
            {
                Value = t.TeacherId.ToString(),
                Text = t.TeacherNameEN + "-" + t.TeacherNameFa,
            }).ToList();
        }

        public List<ShowAllTeachersViewModel> ShowAllTeachers()
        {
            return _context.Teachers.Select(t => new ShowAllTeachersViewModel()
            {
                TeacherId = t.TeacherId,
                TeacherNameFa = t.TeacherNameFa,
                TeacherNameEN = t.TeacherNameEN,
                Description = t.Description,
                TeacherImageName = t.TeacherImageName,
                CourseCount = t.Courses.Count(c => c.TeacherId == t.TeacherId)
            }).ToList();
        }

        public int GetTeachersCount()
        {
            return _context.Teachers.Count();
        }

        public List<SelectListItem> GetLevelsForManageCourse()
        {
            return _context.CourseLevels.Select(l => new SelectListItem()
            {
                Value = l.LevelId.ToString(),
                Text = l.LevelTitle
            }).ToList();
        }

        public List<Company> GetAllCompanies()
        {
            return _context.Companies.Include(c=> c.Courses).ToList();
        }

        public List<SelectListItem> GetCompaniesForManageCourse()
        {
            return _context.Companies.Select(c => new SelectListItem()
            {
                Value = c.CompanyId.ToString(),
                Text = c.CompanyTitle
            }).ToList();
        }

        public List<ShowAllCompaniesViewModel> ShowAllCompanies()
        {
            return _context.Companies.Select(c => new ShowAllCompaniesViewModel()
            {
                CompanyId = c.CompanyId,
                CompanyTitle = c.CompanyTitle,
                CompanyImageName = c.CompanyImageName,
                CourseCount = c.Courses.Count(g => g.CompanyId == c.CompanyId)
            }).ToList();
        }

        public int GetCompaniesCount()
        {
            return _context.Companies.Count();
        }

        public int GetSumTimeCourses()
        {
            return _context.Courses.Sum(c => c.CourseTime);
        }

        public List<SelectListItem> GetCourseTypesForManageCourse()
        {
            return _context.CourseTypes.Select(t => new SelectListItem()
            {
                Value = t.TypeId.ToString(),
                Text = t.TypeTitle,
            }).ToList();
        }

        public List<CourseType> GetAllCourseTypes()
        {
            return _context.CourseTypes.ToList();
        }

        public List<ShowCourseForAdminViewModel> GetCoursesForAdmin()
        {
            return _context.Courses.Select(c => new ShowCourseForAdminViewModel()
            {
                CourseImageName = c.CourseImageName,
                CreateDate = c.CreateDate,
                Company = c.Company.CompanyTitle,
                CourseFaTitle = c.CourseFaTitle,
                CourseType = c.CourseType.TypeTitle,
                CoursePrice = c.CoursePrice,
                CourseTime = c.CourseTime,
                CourseId = c.CourseId,
                IsSpecial = c.IsSpecial,
                CourseLatinTitle = c.CourseLatinTitle,
                GroupId = c.CourseGroup.GroupTitle,
                SubGroupId = c.SubGroup.GroupTitle,
                SecondSubGroupId = c.SecondSubGroup.GroupTitle
            }).ToList();
        }

        public int AddCourse(Course course, IFormFile imgCourseUp, IFormFile courseFile)
        {
            course.CreateDate = DateTime.Now;
            course.CourseImageName = "no-photo.png";


            if (imgCourseUp != null && imgCourseUp.IsImage())
            {
                string imagePath = "";

                course.CourseImageName = GeneratorName.GenrateUniqeCode() + Path.GetExtension(imgCourseUp.FileName);
                imagePath = Path.Combine(_environment.WebRootPath, "course/images", course.CourseImageName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    imgCourseUp.CopyTo(stream);
                }

                #region Save Thumbnail Course Image

                string thumbPath = Path.Combine(_environment.WebRootPath, "course/thumbnail", course.CourseImageName);
                ImageConvertors imgResizer = new ImageConvertors();
                imgResizer.Image_resize(imagePath, thumbPath,200);

                #endregion

            }

            if (courseFile != null)
            {
                string filePath = "";

                course.CourseFileName = GeneratorName.GenrateUniqeCode() + Path.GetExtension(courseFile.FileName);
                filePath = Path.Combine(_environment.WebRootPath, "course/download", course.CourseFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    courseFile.CopyTo(stream);
                }
            }

            _context.Courses.Add(course);
            _context.SaveChanges();
            return course.CourseId;
        }

        public Course GetCourseById(int courseId)
        {
            return _context.Courses.Find(courseId);
        }

        public void UpdateCourse(Course course, IFormFile imgCourse, IFormFile courseFile)
        {
            var currentDate = course.CreateDate;

            if (imgCourse != null && imgCourse.IsImage())
            {
                string imagePath = "";

                #region Remove Old Course Images

                if (course.CourseImageName != "no-photo.png")
                {
                    string deletePath = Path.Combine(_environment.WebRootPath, "course/images", course.CourseImageName);
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                    //------Delete Thumb Course Image ------//
                    string deleteThumbPath = Path.Combine(_environment.WebRootPath, "course/thumbnail", course.CourseImageName);
                    if (File.Exists(deleteThumbPath))
                    {
                        File.Delete(deleteThumbPath);
                    }
                }

                #endregion

                #region Add New Course Image
                course.CourseImageName = GeneratorName.GenrateUniqeCode() + Path.GetExtension(imgCourse.FileName);
                imagePath = Path.Combine(_environment.WebRootPath, "course/images", course.CourseImageName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    imgCourse.CopyTo(stream);
                }



                string thumbPath = Path.Combine(_environment.WebRootPath, "course/thumbnail", course.CourseImageName);
                ImageConvertors imgResizer = new ImageConvertors();
                imgResizer.Image_resize(imagePath, thumbPath, 200);


                #endregion

            }


            if (courseFile != null)
            {
                string filePath = "";

                string deleteFilePath = Path.Combine(_environment.WebRootPath, "course/download", course.CourseFileName);
                if (File.Exists(deleteFilePath))
                {
                    File.Delete(deleteFilePath);
                }

                course.CourseFileName = GeneratorName.GenrateUniqeCode() + Path.GetExtension(courseFile.FileName);
                filePath = Path.Combine(_environment.WebRootPath, "course/download", course.CourseFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    courseFile.CopyTo(stream);
                }
            }

            course.CreateDate = currentDate;
            _context.Courses.Update(course);
            _context.SaveChanges();
        }

        public Tuple<List<ShowCourseListItemViewModel>, int> GetCourses(int pageId = 1, string filter = ""
            , int type = 0, int minPrice = 0, int maxPrice = 0,
            List<int> selectedGroups = null, int take = 0, int companyId = 0, int teacherId = 0)
        {
            IQueryable<Course> result = _context.Courses;

            if (take == 0)
                take = 8;

            if (!string.IsNullOrEmpty(filter))
            {
                result = result.Where(c => c.CourseFaTitle.Contains(filter) || c.CourseLatinTitle.Contains(filter) || c.Tags.Contains(filter));
            }

            if (type > 0)
            {
                result = result.Where(c => c.TypeId == type);
            }

            if (minPrice > 0)
            {
                result = result.Where(c => c.CoursePrice > minPrice);
            }

            if (maxPrice > 0)
            {
                result = result.Where(c => c.CoursePrice < maxPrice);
            }

            if (selectedGroups != null && selectedGroups.Any())
            {
                foreach (var groupId in selectedGroups)
                {
                    result = result.Where(c =>
                        c.GroupId == groupId || c.SubGroupId == groupId || c.SecondSubGroupId == groupId);

                }
            }

            if (companyId > 0)
            {
                result = result.Where(c => c.CompanyId == companyId);
            }

            if (teacherId > 0)
            {
                result = result.Where(c => c.TeacherId == teacherId);
            }

            int skip = (pageId - 1) * take;
            int pageCount = (int)Math.Ceiling(result.Include(c => c.Teacher).Include(c => c.Company).Select(c =>
                               new ShowCourseListItemViewModel()
                               {
                                   CourseId = c.CourseId,
                                   CoursePrice = c.CoursePrice,
                                   CourseTime = c.CourseTime,
                                   Company = c.Company.CompanyTitle,
                                   Teacher = c.Teacher.TeacherNameFa,
                                   CourseImage = c.CourseImageName,
                                   CourseTitle = c.CourseFaTitle,
                                   TeacherImage = c.Teacher.TeacherImageName,

                               }).Count() / (double)take);


            var query = result.Include(c => c.Teacher).Include(c => c.Company).Select(c => new ShowCourseListItemViewModel()
            {
                CourseId = c.CourseId,
                CoursePrice = c.CoursePrice,
                CourseTime = c.CourseTime,
                Company = c.Company.CompanyTitle,
                Teacher = c.Teacher.TeacherNameFa,
                CourseImage = c.CourseImageName,
                CourseTitle = c.CourseFaTitle,
                TeacherImage = c.Teacher.TeacherImageName,

            }).Skip(skip).Take(take).ToList();

            return Tuple.Create(query, pageCount);
        }

        public List<ShowCourseListItemViewModel> GetSpecialCourses()
        {
            return _context.Courses.Include(c => c.Teacher).Include(c => c.Company)
                .Where(c => c.IsSpecial).Select(c => new ShowCourseListItemViewModel()
                {
                    CourseId = c.CourseId,
                    CoursePrice = c.CoursePrice,
                    CourseTime = c.CourseTime,
                    Company = c.Company.CompanyTitle,
                    Teacher = c.Teacher.TeacherNameFa,
                    CourseImage = c.CourseImageName,
                    CourseTitle = c.CourseFaTitle,
                    TeacherImage = c.Teacher.TeacherImageName,

                }).ToList();
        }

        public List<ShowCourseListItemViewModel> GetPopularCourses()
        {
            return _context.Courses.Include(c => c.Teacher).Include(c => c.Company)
                .Include(c => c.OrderDetails)
                .Where(od => od.OrderDetails.Any())
                .OrderByDescending(c => c.OrderDetails.Count)
                .Select(c => new ShowCourseListItemViewModel()
                {
                    CourseId = c.CourseId,
                    CoursePrice = c.CoursePrice,
                    CourseTime = c.CourseTime,
                    Company = c.Company.CompanyTitle,
                    Teacher = c.Teacher.TeacherNameFa,
                    CourseImage = c.CourseImageName,
                    CourseTitle = c.CourseFaTitle,
                    TeacherImage = c.Teacher.TeacherImageName,

                }).Take(8).ToList();

        }

        public Course GetCourseForShow(int id)
        {
            return _context.Courses.Include(c => c.Company)
                .Include(c => c.CourseLevel)
                .Include(c => c.Teacher)
                .Include(c => c.CourseLevel)
                .Include(c => c.CourseGroup)
                .Include(c => c.SubGroup)
                .Include(c => c.SecondSubGroup)
                .Include(c => c.UserCourses)
                .Include(c=> c.CourseComments)
                .FirstOrDefault(c => c.CourseId == id);
        }

        public int CourseCount()
        {
            return _context.Courses.Count();
        }

        public void AddComment(CourseComment comment)
        {
            _context.CourseComments.Add(comment);
            _context.SaveChanges();
        }

        public Tuple<List<CourseComment>, int> GetCourseComments(int courseId, int pageId = 1)
        {
            int take = 6;
            int skip = (pageId - 1) * take;
            int pageCount = (int)Math.Ceiling(_context.CourseComments
                .Include(c => c.User)
                .Count(c => c.CourseId == courseId && !c.IsDelete) / (double)take);

            var result = _context.CourseComments.Include(c => c.User)
                .Where(c => c.CourseId == courseId && !c.IsDelete).OrderByDescending(c => c.CreateDate)
                .Skip(skip).Take(take).ToList();
            return Tuple.Create(result, pageCount);
        }

        public int CourseCommentCount()
        {
            return _context.CourseComments.Count(c => c.IsReadAdmin == false);
        }
    }
}
