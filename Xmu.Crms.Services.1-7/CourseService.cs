using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Services.Group1_7
{
    public class CourseService : ICourseService
    {
        private CrmsContext _db;
        private readonly IUserService _userService;
        private readonly IClassService _classService;
        private readonly ISeminarService _seminarService;

        // 在构造函数里添加依赖的Service（参考模块标准组的类图）
        public CourseService(CrmsContext db, IUserService userService, IClassService classService, ISeminarService seminarService)
        {
            _db = db;
            _userService = userService;
            _classService = classService;
            _seminarService = seminarService;
        }

        /// <summary>
        /// 按courseId删除课程.
        /// @author ZhouZhongjun
        /// </summary>
        /// <param name="courseId">课程Id</param>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISeminarService.DeleteSeminarByCourseId(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IClassService.DeleteClassByCourseId(System.Int64)"/>
        /// <exception cref="T:System.ArgumentException">courseId格式错误时抛出</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.CourseNotFoundException">未找到课程</exception>
        public void DeleteCourseByCourseId(long courseId)
        {
            if (courseId <= 0)
                throw new ArgumentException("格式错误！");

            var course = _db.Course.Where(c => c.Id == courseId).SingleOrDefault();

            if (course == null)
            {
                throw new CourseNotFoundException();
            }

            //删除course下的所有班级
            _classService.DeleteClassByCourseId(courseId);

            //删除course下的所有Seminar
            _seminarService.DeleteSeminarByCourseId(courseId);

            //删除course下的所有课程
            _db.Course.Remove(course);

            _db.SaveChanges();

        }

        /// <summary>
        /// 按courseId获取课程 .
        /// @author ZhouZhongjun
        /// </summary>
        /// <param name="courseId">课程Id</param>
        /// <returns>course</returns>
        /// <exception cref="T:System.ArgumentException">userId格式错误时抛出</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.CourseNotFoundException">未找到课程</exception>
        public Course GetCourseByCourseId(long courseId)
        {
            if (courseId <= 0)
                throw new ArgumentException("格式错误！");

            var course = _db.Course.Include(c => c.Teacher).SingleOrDefault(c => c.Id == courseId);

            if (course == null)
            {
                throw new CourseNotFoundException();
            }
            return course;
        }

        public long InsertClassById(long courseId, ClassInfo classInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 按userId创建课程.
        /// @author ZhouZhongjun
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="course">课程信息</param>
        /// <returns>courseId 新建课程的id</returns>
        /// <exception cref="T:System.ArgumentException">userId格式错误时抛出</exception>
        public long InsertCourseByUserId(long userId, Course course)
        {
            if (userId <= 0)
                throw new ArgumentException("格式错误！");

            UserInfo teacher = _db.UserInfo.Find(userId);
            course.Teacher = teacher;
            _db.Course.Add(course);
            _db.SaveChanges();
            return course.Id;
        }

        /// <summary>
        /// 按课程名称获取班级列表.
        /// @author YeXiaona
        /// </summary>
        /// <param name="courseName">课程名称</param>
        /// <returns>list 班级列表</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ICourseService.ListCourseByCourseName(System.String)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IClassService.ListClassByCourseId(System.Int64)"/>
        public IList<ClassInfo> ListClassByCourseName(string courseName)
        {
            IList<ClassInfo> classes = new List<ClassInfo>();
            IList<ClassInfo> classInfos;
            IList<Course> courses;
            //根据课程名查找课程
            courses = ListCourseByCourseName(courseName);
            //查找每个课程下的班级并加入班级列表不
            foreach (Course course in courses)
            {
                classInfos = _classService.ListClassByCourseId(course.Id);
                foreach (ClassInfo classInfo in classInfos)
                    classes.Add(classInfo);
            }
            return classes;
        }

        public IList<ClassInfo> ListClassByName(string courseName, string teacherName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 按教师名称获取班级列表.
        /// @author YeXiaona
        /// </summary>
        /// <param name="teacherName">教师名称</param>
        /// <returns>list 班级列表</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IUserService.ListUserIdByUserName(System.String)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ICourseService.ListClassByUserId(System.Int64)"/>
        public IList<ClassInfo> ListClassByTeacherName(string teacherName)
        {
            IList<Course> courses;
            IList<ClassInfo> classInfos;
            IList<ClassInfo> classes = new List<ClassInfo>();
            //根据教师名查找课程
            courses = _db.Course.Include(c => c.Teacher).Where(c => c.Teacher.Name == teacherName).ToList();
            //对某一个课程找到其班级列表
            foreach (Course course in courses)
            {
                classInfos = _classService.ListClassByCourseId(course.Id);
                //将该课程下的每一个班级加到班级列表中
                foreach (ClassInfo classinfo in classInfos)
                {
                    classes.Add(classinfo);
                }
            }
            return classes;
        }

        /// <summary>
        /// 根据课程名称获取课程列表.
        /// @author YeXiaona
        /// </summary>
        /// <param name="courseName">课程名称</param>
        /// <returns>list 课程列表</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ICourseService.GetCourseByCourseId(System.Int64)"/>
        public IList<Course> ListCourseByCourseName(string courseName)
        {
            IList<Course> courses;
            courses = _db.Course.Where(c => c.Name == courseName).ToList();
            return courses;
        }

        public IList<Course> ListCourseByTeacherName(string teacherName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 按userId获取与当前用户相关联的课程列表.
        /// @author ZhouZhongjun
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>null 课程列表</returns>
        /// <exception cref="T:System.ArgumentException">userId格式错误时抛出</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.CourseNotFoundException">未找到课程</exception>
        public IList<Course> ListCourseByUserId(long userId)
        {
            if (userId <= 0)
                throw new ArgumentException("格式错误！");

            UserInfo user = _db.UserInfo.Find(userId);
            IList<Course> courses = new List<Course>();

            //用户为老师
            if (user.Type == Shared.Models.Type.Teacher)
            {
                courses = _db.Course.Where(c => c.Teacher.Id == userId).ToList();
                if (courses == null)
                    throw new CourseNotFoundException();

            }
            //用户为学生
            else if (user.Type == Shared.Models.Type.Student)
            {
                //通过学生找到班级列表
                IList<CourseSelection> selections = _db.CourseSelection.Where(c => c.Student.Id == userId).ToList();
                IList<ClassInfo> classes = new List<ClassInfo>();
                foreach (CourseSelection selection in selections)
                {
                    ClassInfo cou = _db.ClassInfo.Find(selection.ClassId);
                    classes.Add(selection.ClassInfo);
                }

                //通过班级列表找到课程列表
                foreach (ClassInfo info in classes)
                {
                    Course cc = _db.Course.Find(info.CourseId);
                    courses.Add(cc);
                }
            }

            else
                throw new Exception("用户还未绑定！");

            return courses;

        }

        /// <summary>
        /// 传入courseId和course信息修改course信息.
        /// @author ZhouZhongjun
        /// </summary>
        /// <param name="courseId">课程Id</param>
        /// <param name="course">课程信息</param>
        public void UpdateCourseByCourseId(long courseId, Course course)
        {
            if (courseId <= 0)
                throw new ArgumentException("格式错误！");

            Course _course = _db.Course.Find(courseId);
            _course.Name = course.Name;
            _course.StartDate = course.StartDate;
            _course.EndDate = course.EndDate;
            _course.Teacher = course.Teacher;
            _course.Description = course.Description;
            _course.ReportPercentage = course.ReportPercentage;
            _course.PresentationPercentage = course.PresentationPercentage;
            _course.FivePointPercentage = course.FivePointPercentage;
            _course.FourPointPercentage = course.FourPointPercentage;
            _course.ThreePointPercentage = course.ThreePointPercentage;
            _db.SaveChanges();
        }
    }
}
