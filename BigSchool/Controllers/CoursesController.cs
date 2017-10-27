using BigSchool.Models;
using BigSchool.ViewModels;
using System.Web.Mvc;
using System.Data.Entity;
using System;
using Microsoft.AspNet.Identity;
using System.Linq;

namespace BigSchool.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public CoursesController()
        {
            dbContext = new ApplicationDbContext();
        }

        public ActionResult Create()
        {
            var viewModel = new CourseViewModel()
            {
                Categories = dbContext.Categories
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CourseViewModel courseViewModel)
        {
            
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Loi");
                courseViewModel.Categories = dbContext.Categories;
                return View(courseViewModel);
            }
            //Lấy thông tin ID của người đang tạo khóa học(là giảng viên)
            var userID = User.Identity.GetUserId();

            //Dữ liệu hợp lệ thì lưu vào Database
            try
            {
                var course = new Course()
                {
                    Place = courseViewModel.Place,
                    DateTime = courseViewModel.getDateTime(),
                    CategoryId = courseViewModel.CategoryId,
                    LecturerId = userID
                };
                //Lưu khóa học
                dbContext.Courses.Add(course);
                dbContext.SaveChanges();

                //Chuyển về trang quản lý khóa học
                return RedirectToAction("ManageCourses");
            }
            catch(System.Exception ex)
            {
                //Write log
                throw ex;
            }
        }
        
        [Authorize]
        public ActionResult ManageCourses()
        {
            try
            {
                var userID = User.Identity.GetUserId();
                var courses = dbContext.Courses
                    .Include(l=>l.Lecturer)
                    .Include(c=>c.Category)
                    .Where(c => c.LecturerId == userID);
                return View(courses);
            }
            catch (Exception ex)
            {
                //write log
                throw ex;
            }
            
        }
    }
}