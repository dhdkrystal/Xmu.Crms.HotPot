using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;
using Type = Xmu.Crms.Shared.Models.Type;

namespace Xmu.Crms.Services.Insomnia
{
    public class UserService : IUserService
    {
        private readonly CrmsContext _db;

        public UserService(CrmsContext db) => _db = db;

        public void InsertAttendanceById(long classId, long seminarId, long userId, double longitude, double latitude)
        {
            var usr = GetUserByUserId(userId);
            var cls = _db.ClassInfo.Find(classId) ?? throw new ClassNotFoundException();
            var sem = _db.Seminar.Find(seminarId) ?? throw new SeminarNotFoundException();
            var loc = _db.Location.Include(a => a.ClassInfo).Include(a => a.Seminar)
                          .SingleOrDefault(l => l.Seminar == sem && l.ClassInfo == cls) ??
                      throw new InvalidOperationException();
            _db.Attendances.Add(new Attendance
            {
                AttendanceStatus = loc.Status == 1 ? AttendanceStatus.Present : AttendanceStatus.Late,
                ClassInfo = cls,
                Seminar = sem,
                Student = usr
            });
            _db.SaveChanges();
        }

        public IList<Attendance> ListAttendanceById(long classId, long seminarId)
        {
            var cls = _db.ClassInfo.Find(classId) ?? throw new ClassNotFoundException();
            var sem = _db.Seminar.Find(seminarId) ?? throw new SeminarNotFoundException();
            return _db.Attendances.Include(a => a.ClassInfo).Include(a => a.Seminar)
                .Where(a => a.ClassInfo == cls && a.Seminar == sem).ToList();
        }

        public UserInfo GetUserByUserId(long userId)
        {
            return _db.UserInfo.Find(userId) ??
                   throw new UserNotFoundException();
        }

        public UserInfo GetUserByUserNumber(string userNumber)
        {
            throw new NotImplementedException();
        }

        public IList<long> ListUserIdByUserName(string userName)
        {
            return _db.UserInfo.Where(u => u.Name.StartsWith(userName)).Select(u => u.Id).ToList();
        }

        public void UpdateUserByUserId(long userId, UserInfo user)
        {
            var usr = GetUserByUserId(userId);
            usr.Name = user.Name;
            usr.Avatar = user.Avatar;
            usr.Education = user.Education ?? Education.Bachelor;
            usr.Email = user.Email;
            usr.Gender = user.Gender;
            if (user.School != null)
            {
                usr.School = _db.School.Find(user.School.Id);
            }
            if ((user.SchoolId ?? 0) != 0)
            {
                usr.School = _db.School.Find(user.SchoolId);
            }
            usr.Title = user.Title ?? Title.Professer;
            if (usr.Type == Type.Unbinded)
            {
                usr.Type = user.Type;
                usr.Number = user.Number;
            }
            else if (user.Type != null && usr.Type != user.Type)
            {
                throw new InvalidOperationException();
            }

            _db.SaveChanges();
        }

        public IList<UserInfo> ListUserByClassId(long classId, string numBeginWith, string nameBeginWith)
        {
            var cls = _db.ClassInfo.Find(classId) ?? throw new ClassNotFoundException();
            var userInfos = _db.CourseSelection.Include(c => c.ClassInfo).Include(c => c.Student)
                .Where(c => c.ClassInfo == cls).Select(c => c.Student);
            if (!string.IsNullOrEmpty(nameBeginWith))
            {
                userInfos = userInfos.Where(u => u.Name.StartsWith(nameBeginWith));
            }

            if (!string.IsNullOrEmpty(nameBeginWith))
            {
                userInfos = userInfos.Where(u => u.Number.StartsWith(numBeginWith));
            }

            return userInfos.ToList();
        }

        public IList<UserInfo> ListUserByUserName(string userName)
        {
            return _db.UserInfo.Where(u => u.Name.StartsWith(userName)).ToList();
        }

        public IList<UserInfo> ListPresentStudent(long seminarId, long classId)
        {
            var cls = _db.ClassInfo.Find(classId) ?? throw new ClassNotFoundException();
            var sem = _db.Seminar.Find(seminarId) ?? throw new SeminarNotFoundException();
            return _db.Attendances.Include(a => a.ClassInfo).Include(a => a.Seminar).Where(a =>
                    a.ClassInfo == cls && a.Seminar == sem && a.AttendanceStatus == AttendanceStatus.Present)
                .Select(a => a.Student).ToList();
        }

        public IList<UserInfo> ListLateStudent(long seminarId, long classId)
        {
            var cls = _db.ClassInfo.Find(classId) ?? throw new ClassNotFoundException();
            var sem = _db.Seminar.Find(seminarId) ?? throw new SeminarNotFoundException();
            return _db.Attendances.Include(a => a.ClassInfo).Include(a => a.Seminar).Where(a =>
                    a.ClassInfo == cls && a.Seminar == sem && a.AttendanceStatus == AttendanceStatus.Late)
                .Select(a => a.Student).ToList();
        }

        public IList<UserInfo> ListAbsenceStudent(long seminarId, long classId)
        {
            var cls = _db.ClassInfo.Find(classId) ?? throw new ClassNotFoundException();
            var sem = _db.Seminar.Find(seminarId) ?? throw new SeminarNotFoundException();
            return _db.Attendances.Include(a => a.ClassInfo).Include(a => a.Seminar).Where(a =>
                    a.ClassInfo == cls && a.Seminar == sem && a.AttendanceStatus == AttendanceStatus.Absent)
                .Select(a => a.Student).ToList();
        }

        public IList<Course> ListCourseByTeacherName(string teacherName)
        {
            return _db.Course.Include(c => c.Teacher).Where(c => c.Teacher.Name.StartsWith(teacherName)).ToList();
        }

        public int CheckLocation(long classId, long seminarId)//add
        {
            var loc = (from y in _db.Location
                       where y.ClassInfo.Id == classId && y.Seminar.Id == seminarId && y.Status == 1
                       select y.Status).SingleOrDefault();
            if (loc == null)
                return 0;
            else
                return 1;
        }
        public void InsertPresentById(long classId, long seminarId, long userId, double longitude, double latitude)//测试成功时间 add
        {
            if (classId <= 0 || seminarId <= 0 || userId <= 0)
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
            var att = (from i in _db.Attendances
                       where i.Seminar.Id == seminarId && i.Student.Id == userId && i.ClassInfo.Id == classId
                       select i).SingleOrDefault();
            att.AttendanceStatus = AttendanceStatus.Present;

            //Attendance attendance = new Attendance();
            //attendance.ClassInfo = (from i in _db.ClassInfo
            //                        where i.Id == classId
            //                        select i).SingleOrDefault();
            //attendance.Student = (from j in _db.UserInfo
            //                      where j.Id == userId
            //                      select j).SingleOrDefault();
            //attendance.Seminar = (from k in _db.Seminar
            //                      where k.Id == seminarId
            //                      select k).SingleOrDefault();
            //_db.Attendences.Add(attendance);
            _db.SaveChanges();
        }

        public AttendanceStatus? UpdateAttendanceById(long classId, long seminarId, long userId, decimal longitude, decimal latitude)
        {
            throw new NotImplementedException();
        }

        public Attendance GetAttendanceById(long classId, long seminarId, long userId)
        {
            throw new NotImplementedException();
        }

        public AttendanceStatus InsertAttendanceById(long classId, long seminarId, long userId, decimal longitude, decimal latitude)
        {
            throw new NotImplementedException();
        }

        public void InsertClassAttendanceById(long classId, long seminarId)
        {
            throw new NotImplementedException();
        }
    }
}