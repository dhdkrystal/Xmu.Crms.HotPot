using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Services.SmartFive
{
    public class UserService : IUserService
    {
        private readonly CrmsContext _db;

        // 在构造函数里添加依赖的Service（参考模块标准组的类图）
        public UserService(CrmsContext db)
        {
            _db = db;
        }

        public UserInfo GetUserByUserId(long userId)//测试成功
        {
            if (userId <= 0)
            {
                throw new ArgumentException();
            }

           
               var  u = _db.UserInfo.Single(user=>user.Id==userId);
            //var u = (from user in _db.UserInfo
            //         where user.Id == userId
            //         select new UserInfo
            //         {
            //             Id = user.Id,
            //             Phone = user.Phone,
            //             Avatar = user.Avatar,
            //             Password = user.Password,
            //             Name = user.Name,
            //             SchoolId = user.SchoolId,
            //             Gender = user.Gender,
            //             Type = user.Type,
            //             Number = user.Number,
            //             Education = user.Education,
            //             Title = user.Title,
            //             Email = user.Email
            //         }).SingleOrDefault();

            if (u == null)
            {
                throw new UserNotFoundException();
            }

            return u;
        }

        public UserInfo GetUserByUserNumber(string userNumber)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 获取签到状态
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="seminarId"></param>
        /// <param name="userId"></param>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public Attendance GetAttendanceById(long classId, long seminarId, long userId)//测试成功时间
        {
            if (classId <= 0 || seminarId <= 0 || userId <= 0)
            {
                throw new ArgumentException();
            }
            if (_db.Seminar.Find(seminarId) == null)
                throw new SeminarNotFoundException();
            if (_db.ClassInfo.Find(classId) == null)
                throw new ClassNotFoundException();
           var attendance = _db.Attendances
                .Include(l => l.Seminar)
                .Include(l => l.ClassInfo)
                .Include(l=>l.Student)
                .SingleOrDefault(s => (s.Seminar.Id == seminarId && s.ClassInfo.Id == classId&&s.Student.Id==userId));
           return attendance;
        }

        public void InsertClassAttendanceById(long classId, long seminarId)
        {
            if (classId <= 0 || seminarId <= 0)
            {
                throw new ArgumentException();
            }
            if (_db.Seminar.Find(seminarId) == null)
                throw new SeminarNotFoundException();
            if (_db.ClassInfo.Find(classId) == null)
                throw new ClassNotFoundException();
            var students = _db.CourseSelection.Where(c => c.ClassId == classId).ToList();
            foreach (var s in students) {
                Attendance a = new Attendance();
                a.ClassInfo= (from i in _db.ClassInfo
                              where i.Id == classId
                              select i).SingleOrDefault();
                a.Student = (from j in _db.UserInfo
                                      where j.Id ==s.StudentId
                                      select j).SingleOrDefault();
                a.Seminar = (from k in _db.Seminar
                                      where k.Id == seminarId
                                      select k).SingleOrDefault();
                a.AttendanceStatus = AttendanceStatus.Absent;
                _db.Attendances.Add(a);
            }
            _db.SaveChanges();
        }
        public AttendanceStatus InsertAttendanceById(long classId, long seminarId, long userId, decimal longitude, decimal latitude)//测试成功时间
        {
            if (classId <= 0 || seminarId <= 0 || userId <= 0)
            {
                throw new ArgumentException();
            }
            if (_db.Seminar.Find(seminarId) == null)
                throw new SeminarNotFoundException();
            if (_db.ClassInfo.Find(classId) == null)
                throw new ClassNotFoundException();
            //var u = (from class1 in _db.ClassInfo
            //         where class1.Id == classId
            //         select class1).SingleOrDefault();
            //if (u == null)
            //    throw new ClassNotFoundException();
            //var v = (from seminar in _db.Seminar
            //         where seminar.Id == seminarId
            //         select seminar).SingleOrDefault();
            //if (v == null)
            //    throw new SeminarNotFoundException();
            Attendance attendance = new Attendance();
            attendance.ClassInfo = (from i in _db.ClassInfo
                                    where i.Id == classId
                                    select i).SingleOrDefault();
            attendance.Student = (from j in _db.UserInfo
                                  where j.Id == userId
                                  select j).SingleOrDefault();
            attendance.Seminar = (from k in _db.Seminar
                                  where k.Id == seminarId
                                  select k).SingleOrDefault();
            
            var location = _db.Location
                .Include(l => l.Seminar)
                .Include(l => l.ClassInfo)
                .SingleOrDefault(s => (s.Seminar.Id == seminarId && s.ClassInfo.Id == classId));
            double distance=GetDistance((double)latitude, (double)longitude, (double)location.Latitude, (double)location.Longitude);
            if (distance < 20)
                if (location.Status == 1)
                {
                    attendance.AttendanceStatus = AttendanceStatus.Present;
                }
                else
                {
                    attendance.AttendanceStatus = AttendanceStatus.Late;
                }
            else
                attendance.AttendanceStatus = AttendanceStatus.Absent;
            _db.Attendances.Add(attendance);
            _db.SaveChanges();
            return attendance.AttendanceStatus??0;
        }

        /// <summary>
        /// 改变签到状态
        /// </summary>
        /// <param name="seminarId"></param>
        /// <param name="classId"></param>
        /// <returns></returns>
        public AttendanceStatus? UpdateAttendanceById(long classId, long seminarId, long userId, decimal longitude, decimal latitude)//测试成功时间
        {
            if (classId <= 0 || seminarId <= 0 || userId <= 0)
            {
                throw new ArgumentException();
            }
            if (_db.Seminar.Find(seminarId) == null)
                throw new SeminarNotFoundException();
            if (_db.ClassInfo.Find(classId) == null)
                throw new ClassNotFoundException();
            var attendance = _db.Attendances.Include(a => a.ClassInfo)
                .Include(a => a.Seminar).SingleOrDefault(a => (a.SeminarId == seminarId && a.ClassId == classId && a.StudentId == userId));
            var location = _db.Location
                .Include(l => l.Seminar)
                .Include(l => l.ClassInfo)
                .SingleOrDefault(s => (s.Seminar.Id == seminarId && s.ClassInfo.Id == classId));
            double distance = GetDistance((double)latitude, (double)longitude, (double)location.Latitude, (double)location.Longitude);
            if (distance < 20)
                if (location.Status == 1)
                {
                    attendance.AttendanceStatus = AttendanceStatus.Present;
                }
                else
                {
                    attendance.AttendanceStatus = AttendanceStatus.Late;
                }
            //else
                //attendance.AttendanceStatus = AttendanceStatus.Absent;
            _db.SaveChanges();
            return attendance.AttendanceStatus;
        }

        public IList<UserInfo> ListAbsenceStudent(long seminarId, long classId) //测试成功
        {
            if (classId <= 0 || seminarId <= 0)
            {
                throw new ArgumentException();
            }
            var u = (from class1 in _db.ClassInfo
                     where class1.Id == classId
                     select class1).SingleOrDefault();
            if (u == null)
                throw new ClassNotFoundException();
            var v = (from seminar in _db.Seminar
                     where seminar.Id == seminarId
                     select seminar).SingleOrDefault();
            if (v == null)
                throw new SeminarNotFoundException();
            var u1 = (from user in _db.Attendances//.Include(a => a.Student)user.Student
                      where user.AttendanceStatus == AttendanceStatus.Absent && user.Seminar.Id == seminarId && user.ClassInfo.Id == classId
                      select new UserInfo
                     {
                         Id = user.Student.Id,
                         Phone = user.Student.Phone,
                         Avatar = user.Student.Avatar,
                         Password = user.Student.Password,
                         Name = user.Student.Name,
                         School = new School { Id = user.Student.School.Id },
                         Gender = user.Student.Gender,
                         Type = user.Student.Type,
                         Number = user.Student.Number,
                         Education = user.Student.Education,
                         Title = user.Student.Title,
                         Email = user.Student.Email
                     }).ToList();
            return u1;
        }

        public IList<Attendance> ListAttendanceById(long classId, long seminarId)//测试成功
        {
            var u = (from a in _db.Attendances
                     where a.ClassInfo.Id == classId && a.Seminar.Id == seminarId
                     select new Attendance
                     {
                         Id = a.Id,
                         Student = new UserInfo { Id = a.Student.Id },
                         ClassInfo = new ClassInfo { Id = a.ClassInfo.Id },
                         Seminar = new Seminar { Id = a.Seminar.Id },
                         AttendanceStatus = a.AttendanceStatus
                     }).ToList();
            return u;
        }

        public IList<Course> ListCourseByTeacherName(string teacherName)//测试成功
        {
            var u = (from course in _db.Course.Include(a => a.Teacher)
                     where course.Teacher.Name.Equals(teacherName)
                     select new Course
                     {
                         Id = course.Id,
                         Name = course.Name,
                         StartDate = course.StartDate,
                         EndDate = course.EndDate,
                         Teacher = new UserInfo { Id=course.Teacher.Id},
                         Description = course.Description,
                         ReportPercentage = course.ReportPercentage,
                         PresentationPercentage = course.PresentationPercentage,
                         FivePointPercentage = course.FivePointPercentage,
                         FourPointPercentage = course.FourPointPercentage,
                         ThreePointPercentage = course.ThreePointPercentage
                     }).ToList();
            return u;
        }

        public IList<UserInfo> ListLateStudent(long seminarId, long classId)//测试成功
        {
            if (classId <= 0 || seminarId <= 0)
            {
                throw new ArgumentException();
            }
            var u = (from class1 in _db.ClassInfo
                     where class1.Id == classId
                     select class1).SingleOrDefault();
            if (u == null)
                throw new ClassNotFoundException();
            var v = (from seminar in _db.Seminar
                     where seminar.Id == seminarId
                     select seminar).SingleOrDefault();
            if (v == null)
                throw new SeminarNotFoundException();
            var u1 = (from user in _db.Attendances//.Include(a => a.Student)user.Student
                      where user.AttendanceStatus == AttendanceStatus.Late && user.Seminar.Id == seminarId && user.ClassInfo.Id == classId
                      select new UserInfo
                      {
                          Id = user.Student.Id,
                          Phone = user.Student.Phone,
                          Avatar = user.Student.Avatar,
                          Password = user.Student.Password,
                          Name = user.Student.Name,
                          School = new School { Id = user.Student.School.Id },
                          Gender = user.Student.Gender,
                          Type = user.Student.Type,
                          Number = user.Student.Number,
                          Education = user.Student.Education,
                          Title = user.Student.Title,
                          Email = user.Student.Email
                      }).ToList();
            return u1;
        }

        public IList<UserInfo> ListPresentStudent(long seminarId, long classId)//测试成功
        {
            if (classId <= 0 || seminarId <= 0)
            {
                throw new ArgumentException();
            }
            var u = (from class1 in _db.ClassInfo
                     where class1.Id == classId
                     select class1).SingleOrDefault();
            if (u == null)
                throw new ClassNotFoundException();
            var v = (from seminar in _db.Seminar
                     where seminar.Id == seminarId
                     select seminar).SingleOrDefault();
            if (v == null)
                throw new SeminarNotFoundException();
            var u1 = (from user in _db.Attendances//.Include(a => a.Student)user.Student
                      where user.AttendanceStatus == AttendanceStatus.Present && user.Seminar.Id == seminarId && user.ClassInfo.Id == classId
                      select new UserInfo
                      {
                          Id = user.Student.Id,
                          Phone = user.Student.Phone,
                          Avatar = user.Student.Avatar,
                          Password = user.Student.Password,
                          Name = user.Student.Name,
                          School = new School { Id = user.Student.School.Id },
                          Gender = user.Student.Gender,
                          Type = user.Student.Type,
                          Number = user.Student.Number,
                          Education = user.Student.Education,
                          Title = user.Student.Title,
                          Email = user.Student.Email
                      }).ToList();
            return u1;
        }

        public IList<UserInfo> ListUserByClassId(long classId, string numBeginWith, string nameBeginWith)//测试成功
        {
            var user = (from class1 in _db.CourseSelection
                        from c in _db.UserInfo
                        where class1.ClassInfo.Id == classId && class1.Student.Id == c.Id && c.Name.ToString().StartsWith(nameBeginWith) && c.Number.ToString().StartsWith(numBeginWith)
                        select new UserInfo
                        {
                            Id = class1.Student.Id,
                            Phone = class1.Student.Phone,
                            Avatar = class1.Student.Avatar,
                            Password = class1.Student.Password,
                            Name = class1.Student.Name,
                            School = new School { Id = class1.Student.School.Id },
                            Gender = class1.Student.Gender,
                            Type = class1.Student.Type,
                            Number = class1.Student.Number,
                            Education = class1.Student.Education,
                            Title = class1.Student.Title,
                            Email = class1.Student.Email
                        }).ToList();
            return user;
        }

        public IList<UserInfo> ListUserByUserName(string userName)//测试成功
        {
            var user = (from user1 in _db.UserInfo
                        where user1.Name.Equals(userName)
                        select new UserInfo
                        {
                            Id = user1.Id,
                            Phone = user1.Phone,
                            Avatar = user1.Avatar,
                            Password = user1.Password,
                            Name = user1.Name,
                            School = new School { Id = user1.School.Id },
                            Gender = user1.Gender,
                            Type = user1.Type,
                            Number = user1.Number,
                            Education = user1.Education,
                            Title = user1.Title,
                            Email = user1.Email
                        }).ToList();
            return user;
        }

        public IList<long> ListUserIdByUserName(string userName)//测试成功
        {
            var user = (from user1 in _db.UserInfo
                        where user1.Name.Equals(userName)
                        select user1.Id).ToList();
            return user;
        }

        public void UpdateUserByUserId(long userId, UserInfo user)//测试成功
        {
            var usr = GetUserByUserId(userId);
            usr.Name = user.Name;
            usr.Avatar = user.Avatar;
            usr.Number = user.Number;
            //usr.Education = user.Education ?? Education.Bachelor;
            usr.Email = user.Email;
            usr.Gender = user.Gender;
            usr.SchoolId = user.SchoolId;
            if (user.School != null)
            {
                usr.School = _db.School.Find(user.School.Id);
            }
            if ((user.SchoolId ?? 0) != 0)
            {
                usr.School = _db.School.Find(user.SchoolId);
            }
            //usr.Title = user.Title ?? Title.Professer;
            if (usr.Type == Shared.Models.Type.Unbinded)
            {
                
                usr.Type = user.Type??Shared.Models.Type.Student;
                usr.Number = user.Number;
            }
            else if (user.Type != null && usr.Type != user.Type)
            {
                throw new InvalidOperationException();
            }
            _db.SaveChanges();
        }

        private const double EARTH_RADIUS = 6378.137;//地球半径
        private static double Rad(double d)
        {
            return d * Math.PI / 180.0;
        }

        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = Rad(lat1);
            double radLat2 = Rad(lat2);
            double a = radLat1 - radLat2;
            double b = Rad(lng1) - Rad(lng2);

            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }

    }
}
