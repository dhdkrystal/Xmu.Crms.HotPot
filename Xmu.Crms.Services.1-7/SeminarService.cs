using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Services.Group1_7
{
    public class SeminarService : ISeminarService
    {
        private CrmsContext _db;
        private ISeminarGroupService _seminarGroupService;
        private ITopicService _topicService;

        public SeminarService(CrmsContext db,ISeminarGroupService seminarGroupService, ITopicService topicService)
        {
            _db = db;
            _seminarGroupService = seminarGroupService;
            _topicService = topicService;
        }

        /*
         * author：邓帅
         * QQ：540043604
         */

        /// <summary>
        /// 按courseId删除Seminar.
        /// 先根据CourseId获得所有的seminar的信息，然后根据seminar信息删除相关topic的记录，然后再根据SeminarId删除SeminarGroup表记录,最后再将seminar的信息删除
        /// </summary>
        /// <param name="courseId">课程Id</param>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISeminarService.ListSeminarByCourseId(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ITopicService.DeleteTopicBySeminarId(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISeminarGroupService.DeleteSeminarGroupBySeminarId(System.Int64)"/>
        /// <exception cref="T:System.ArgumentException">格式错误时抛出</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.CourseNotFoundException">该课程不存在时抛出</exception>
        public void DeleteSeminarByCourseId(long courseId)
        {
            if (courseId < 0)
                throw new ArgumentException();
            if (_db.Course.Find(courseId) == null)
                throw new CourseNotFoundException();

            var course = _db.Course.Find(courseId);
            List<Seminar> seminars;

            seminars = ListSeminarByCourseId(courseId).ToList();

            foreach (Seminar seminar in seminars)
            {
                _seminarGroupService.DeleteSeminarGroupBySeminarId(seminar.Id);
                _topicService.DeleteTopicBySeminarId(seminar.Id);
            }
            _db.Seminar.RemoveRange(seminars);

            _db.SaveChanges();
        }

        /// 按讨论课id删除讨论课.
        /// 用户（老师）通过seminarId删除讨论课(包括删除讨论课包含的topic信息和小组信息).
        /// <param name="seminarId">讨论课的id</param>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISeminarGroupService.DeleteSeminarGroupBySeminarId(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ITopicService.DeleteTopicBySeminarId(System.Int64)"/>
        /// <exception cref="T:System.ArgumentException">格式错误时抛出</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.SeminarNotFoundException">该讨论课不存在时抛出</exception>
        public void DeleteSeminarBySeminarId(long seminarId)
        {
            if (seminarId < 0)
                throw new ArgumentException();
            if (_db.Seminar.Find(seminarId) == null)
                throw new SeminarNotFoundException();

            var seminar = _db.Seminar.SingleOrDefault(s => s.Id == seminarId);
            _topicService.DeleteTopicBySeminarId(seminarId);
            _seminarGroupService.DeleteSeminarGroupBySeminarId(seminarId);
            _db.Seminar.Remove(seminar);

            _db.SaveChanges();
        }

        /// <summary>
        /// 用户通过讨论课id获得讨论课的信息.
        /// 用户通过讨论课id获得讨论课的信息（包括讨论课名称、讨论课描述、分组方式、开始时间、结束时间）
        /// <summary>      
        /// <param name="seminarId">讨论课的id</param>
        /// <returns>相应的讨论课信息</returns>
        /// <exception cref="T:System.ArgumentException">格式错误时抛出</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.CourseNotFoundException">该课程不存在时抛出</exception>?????????????????????????????
        public Seminar GetSeminarBySeminarId(long seminarId)
        {
            if (seminarId < 0)
                throw new ArgumentException();
            
            if (_db.Seminar.Find(seminarId) == null)
                throw new SeminarNotFoundException();

            var seminar = _db.Seminar.Find(seminarId);
            
            return seminar;
        }

        /// <summary>
        /// 新增讨论课.
        /// 用户（老师）在指定的课程下创建讨论课
        /// <summary>
        /// <param name="courseId">课程的id</param>
        /// <param name="seminar">讨论课信息</param>
        /// <returns>seminarId 若创建成功返回创建的讨论课id，失败则返回-1</returns>
        /// <exception cref="T:System.ArgumentException">格式错误时抛出</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.SeminarNotFoundException">该讨论课不存在时抛出</exception>
        public long InsertSeminarByCourseId(long courseId, Seminar seminar)
        {
            
            if (_db.Course.Find(courseId) == null)
                throw new CourseNotFoundException();
            if (_db.Seminar.Find(seminar.Id) != null)
                throw new InvalidOperationException();

            var course = _db.Course.Find(courseId);
            var seminar0 = _db.Seminar.Find(seminar.Id);

            seminar.Course = course;
            _db.Seminar.Add(seminar);
            _db.SaveChanges();

            if ((_db.Seminar.Find(seminar.Id)) == null)
                return -1;
            else 
                return seminar.Id;
        }

        /// <summary>
        /// 按courseId获取Seminar.
        /// <summary>
        /// <param name="courseId">课程Id</param>
        /// <returns>List 讨论课列表</returns>
        /// <exception cref="T:System.ArgumentException">格式错误、教师设置embedGrade为true时抛出</exception>??????????????????????
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.CourseNotFoundException">未找到该课程时抛出</exception>
        public IList<Seminar> ListSeminarByCourseId(long courseId)
        {
            if (courseId < 0)
                throw new ArgumentException();
            if (_db.Course.Find(courseId) == null)
                throw new CourseNotFoundException();

            var course = _db.Course.Find(courseId);
            List<Seminar> seminars;
            seminars = _db.Seminar.Include(x => x.Course)
                .Where(c => c.Course == course).OrderByDescending(a=>a.EndTime).ToList();
            return seminars;
        }

        /// <summary>
        /// 按讨论课id修改讨论课.
        /// 用户（老师）通过seminarId修改讨论课的相关信息
        /// <summary>
        /// <param name="seminarId">讨论课的id</param>
        /// <param name="seminar">讨论课信息</param>
        /// <exception cref="T:System.ArgumentException">格式错误时抛出</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.SeminarNotFoundException">该讨论课不存在时抛出</exception>
        public void UpdateSeminarBySeminarId(long seminarId, Seminar seminar)
        {
            if (seminarId < 0)
                throw new ArgumentException();
            if (_db.Seminar.Find(seminarId) == null)
                throw new SeminarNotFoundException();

            Seminar seminar0 = _db.Seminar.Find(seminarId);

            seminar0.Id = seminar.Id;
            seminar0.Name = seminar.Name;
            seminar0.Description = seminar.Description;
            seminar0.Course = seminar.Course;
            seminar0.IsFixed = seminar.IsFixed;
            seminar0.StartTime = seminar.StartTime;
            seminar0.EndTime = seminar.EndTime;

            _db.SaveChanges();
        }
        public Location GetLocation(long seminarId,long classId)
        {
            if (_db.Seminar.Find(seminarId) == null)
                throw new SeminarNotFoundException();
            if (_db.ClassInfo.Find(classId) == null)
                throw new ClassNotFoundException();
            var location = _db.Location
                .Include(l => l.Seminar)
                .Include(l => l.ClassInfo)
                .SingleOrDefault(s => (s.Seminar.Id == seminarId && s.ClassInfo.Id == classId));
            return location;
        }
    }
}
