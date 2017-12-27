using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;



namespace Xmu.Crms.Services.HotPot

{
    /*
    public class UserService : IUserService

    {

        private readonly CrmsContext _db;

        private readonly ISchoolService _schoolService;



        // 在构造函数里添加依赖的Service（参考模块标准组的类图）

        public UserService(CrmsContext db, ISchoolService schoolService)

        {

            _db = db;

            _schoolService = schoolService;

        }



        public void InsertAttendanceById(long classId, long seminarId, long userId, double longitude, double latitude)

        {

            throw new NotImplementedException();

        }



        public List<Attendance> ListAttendanceById(long classId, long seminarId)

        {

            throw new NotImplementedException();

        }



        public User SignUpPhone(User user)

        {

            var us = _db.UserInfo.SingleOrDefault(u => u.Phone == user.Phone);

            if (us == null)

            {

                throw new UserNotFoundException();

            }

            if (user.Password != us.Password) // 千万不要真的用明文存储密码！

            {

                throw new PasswordErrorException();

            }

            return us;

        }



        public bool DeleteTeacherAccount(long userId)
        {
            throw new NotImplementedException();
        }



        public bool DeleteStudentAccount(long userId)
        {
            throw new NotImplementedException();
        }



        public User GetUserByUserId(long id)
        {
            // 调用Entity framework
            var user = _db.UserInfo.Include(u => u.School).SingleOrDefault(u => u.Id == id);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            // 调用依赖的 Serivce
            _schoolService.GetSchoolBySchoolId(user.School.Id);
            return user;

        }



        public List<long> ListUserIdByUserName(string userName)
        {
            throw new NotImplementedException();
        }



        public void UpdateUserByUserId(long userId, User user)
        { 
            throw new NotImplementedException();
        }



        public List<User> ListUserByClassId(long classId, string numBeginWith, string nameBeginWith)
        {
            throw new NotImplementedException();
        }



        public List<User> ListUserByUserName(string userName)
        {
            throw new NotImplementedException();
        }



        public List<User> ListPresentStudent(long seminarId, long classId)

        {

            throw new NotImplementedException();

        }



        public List<User> ListAbsenceStudent(long seminarId, long classId)

        {

            throw new NotImplementedException();

        }



        public List<Course> ListCourseByTeacherName(string teacherName)

        {

            throw new NotImplementedException();

        }

    }
    */

}