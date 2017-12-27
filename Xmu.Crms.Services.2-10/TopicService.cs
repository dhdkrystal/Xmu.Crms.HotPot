using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Services.Group2_10
{
    public class TopicService : ITopicService
    {
        private CrmsContext _db;
        public TopicService(CrmsContext db)
        {
            _db = db;
        }

        /*
         * author:汪亚东
         * QQ:994094745
         */

        /// <summary>
        /// 按topicId删除SeminarGroupTopic表信息.
        /// </summary>
        /// <param name="topicId">讨论课Id</param>
        /// <exception cref="T:System.ArgumentException">topicId格式错误</exception>
        /// /// <exception cref="T:TopicNotFoundException">topic不存在</exception>
        public void DeleteSeminarGroupTopicByTopicId(long topicId)
        {
            if (topicId <= 0) throw new ArgumentException("topicId格式错误");//topicId不合法时抛出异常
            Topic topic = _db.Topic.Find(topicId);//根据topicId找到topic实体
            if (topic == null) throw new TopicNotFoundException();//找不到topic时抛出

            //将两表连接起来查询
            List<SeminarGroupTopic> sgtList = _db.SeminarGroupTopic.Include(x => x.Topic).ToList();

            //根据topic实体找到SeminarGroupTopic实体集
            List<SeminarGroupTopic> sgtListToDelete = sgtList.FindAll(x => x.Topic == topic);

            //级联查找student_score_group表中的记录
            List<StudentScoreGroup> ssgList = _db.StudentScoreGroup
                .Include(x => x.SeminarGroupTopic)
                .ToList();

            List<StudentScoreGroup> ssgListToDelete = ssgList.FindAll(x => sgtListToDelete.Contains(x.SeminarGroupTopic));

            _db.StudentScoreGroup.RemoveRange(ssgListToDelete);//级联删除student_score_group表中的记录
            _db.SeminarGroupTopic.RemoveRange(sgtListToDelete);//删除找到的SeminarGroupTopic实体集
            _db.SaveChanges();//提交事务
        }

        /// <summary>
        /// 小组取消选择话题.
        /// 删除seminar_group_topic表的记录
        /// </summary>
        /// <param name="groupId">小组Id</param>
        /// <param name="topicId">话题Id</param>
        /// <exception cref="T:System.ArgumentException">groupId格式错误或topicId格式错误时抛出</exception>
        public void DeleteSeminarGroupTopicById(long groupId, long topicId)
        {
            if (groupId <= 0) throw new ArgumentException("groupId格式错误");//groupId不合法时抛出异常
            if (topicId <= 0) throw new ArgumentException("topicId格式错误");//topicId不合法时抛出异常

            Topic topic = _db.Topic.Find(topicId);//根据topicId找到topic实体
            if (topic == null) throw new TopicNotFoundException();//找不到topic时抛出

            SeminarGroup group = _db.SeminarGroup.Find(groupId);//根据groupId找到group实体
            if (group == null) throw new GroupNotFoundException();//找不到group时抛出

            //链接三表查询
            List<SeminarGroupTopic> sgtList = _db.SeminarGroupTopic.Include(x => x.Topic)
                .Include(x => x.SeminarGroup)
                .Where(x => x.Topic == topic && x.SeminarGroup == group).ToList();

            //级联查找student_score_group表中的记录
            List<StudentScoreGroup> ssgList = _db.StudentScoreGroup
                .Include(x => x.SeminarGroupTopic)
                .ToList();

            List<StudentScoreGroup> ssgListToDelete = ssgList.FindAll(x => sgtList.Contains(x.SeminarGroupTopic));

            _db.StudentScoreGroup.RemoveRange(ssgListToDelete);//级联删除student_score_group表中的记录

            //删除找到的记录,此记录只有一条，但由于存储在List中，故使用RemoveRange()
            _db.SeminarGroupTopic.RemoveRange(sgtList);
            _db.SaveChanges();
        }

        /// <summary>
        /// 按seminarId删除话题.
        /// 根据seminarId获得topic信息，然后再根据topic删除SeminarGroupTopic信息和StudentScoreGroup信息，最后再根据删除topic信息
        /// </summary>
        /// <param name="seminarId">讨论课Id</param>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ITopicService.ListTopicBySeminarId(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ITopicService.DeleteSeminarGroupTopicByTopicId(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IGradeService.DeleteStudentScoreGroupByTopicId(System.Int64)"/>
        /// <exception cref="T:System.ArgumentException">seminarId格式错误</exception>
        public void DeleteTopicBySeminarId(long seminarId)
        {
            if (seminarId <= 0) throw new ArgumentException("seminarId不合法");
            Seminar seminar = _db.Seminar.Find(seminarId);
            if (seminar == null) throw new SeminarNotFoundException();

            List<Topic> topicList = _db.Topic
                .Include(x => x.Seminar)
                .Where(x => x.Seminar == seminar)
                .ToList();

            //级联删除
            topicList.ForEach(x => DeleteSeminarGroupTopicByTopicId(x.Id));

            //删除topic
            _db.Topic.RemoveRange(topicList);
            _db.SaveChanges();
        }

        /// <summary>
        /// 删除topic.
        /// </summary>
        /// <param name="topicId">要删除的topic的topicId</param>
        /// <param name="seminarId">要删除topic所属seminar的id</param>
        /// <exception cref="T:System.ArgumentException">Id格式错误时抛出</exception>
        public void DeleteTopicByTopicId(long topicId)
        {
            Topic topic;
            if (topicId <= 0) throw new ArgumentException("topicId不合法");
            if ((topic = _db.Topic.Find(topicId)) == null)
                throw new TopicNotFoundException();

            DeleteSeminarGroupTopicByTopicId(topic.Id);
            _db.Topic.Remove(topic);

            _db.SaveChanges();
        }

        ///<summary>
        ///按话题id和小组id获取讨论课小组选题信息
        ///</summary>
        ///<param name="topicId">讨论课Id</param>
        ///<param name="groupId">小组Id</param>
        ///<returns>seminarGroupTopic 讨论课小组选题信息</returns>
        ///<exception cref="T:System.ArgumentException">topicId格式错误</exception>
        ///<exception cref="T:System.ArgumentException">groupId格式错误</exception>
        public SeminarGroupTopic GetSeminarGroupTopicById(long topicId, long groupId)
        {
            Topic topic;
            SeminarGroup group;
            if (topicId <= 0) throw new ArgumentException("topicId不合法");
            if (groupId <= 0) throw new ArgumentException("groupId不合法");

            if ((topic = _db.Topic.Find(topicId)) == null)
                throw new TopicNotFoundException();
            if ((group = _db.SeminarGroup.Find(groupId)) == null)
                throw new GroupNotFoundException();

            List<SeminarGroupTopic> sgList = _db.SeminarGroupTopic
                .Include(x => x.Topic)
                .Include(x => x.SeminarGroup)
                .Where(x => x.Topic.Id == topicId && x.SeminarGroup.Id == groupId)
                .ToList();

            return sgList[0];
        }

        /// <summary>
        /// 按topicId获取topic.
        /// </summary>
        /// <param name="topicId">要获取的topic的topicId</param>
        /// <returns>该topic</returns>
        /// <exception cref="T:System.ArgumentException">Id格式错误时抛出</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.TopicNotFoundException">无此小组或Id错误</exception>
        public Topic GetTopicByTopicId(long topicId)
        {
            if (topicId <= 0) throw new ArgumentException("topicId不合法");

            //根据Id查找到topic
            List<Topic> topicList = _db.Topic.Include(x => x.Seminar)
                .Where(x => x.Id == topicId)
                .ToList();

            //找不到topic抛出异常
            if (topicList == null) throw new TopicNotFoundException();

            //由于topicList只可能有一个元素，故直接返回该元素
            return topicList[0];
        }

        /// <summary>
        /// 根据讨论课Id和topic信息创建一个话题.
        /// </summary>
        /// <param name="seminarId">话题所属讨论课的Id</param>
        /// <param name="topic">话题</param>
        /// <returns>新建话题后给topic分配的Id</returns>
        /// <exception cref="T:System.ArgumentException">Id格式错误时抛出</exception>
        public long InsertTopicBySeminarId(long seminarId, Topic topic)
        {
            if (seminarId <= 0) throw new ArgumentException("seminarId错误");
            Seminar seminar = _db.Seminar.Find(seminarId);
            if (seminar == null) throw new SeminarNotFoundException();//找不到seminar抛出异常

            //将seminarId对应得seminar赋值给要插入的topic对象
            topic.Seminar = seminar;

            //插入到Topic数据集中
            _db.Topic.Add(topic);

            //更新数据库
            _db.SaveChanges();

            return topic.Id;
        }

        /// <summary>
        /// 按seminarId获取Topic.
        /// </summary>
        /// <param name="seminarId">讨论课Id</param>
        /// <returns>null</returns>
        /// <exception cref="T:System.ArgumentException">Id格式错误时抛出</exception>
        public IList<Topic> ListTopicBySeminarId(long seminarId)
        {
            if (seminarId <= 0) throw new ArgumentException("seminarId错误");
            Seminar seminar = _db.Seminar.Find(seminarId);
            if (seminar == null) throw new SeminarNotFoundException();//找不到seminar抛出异常

            //查找出topic
            List<Topic> topicList = _db.Topic.Where(x => x.Seminar == seminar).ToList();
            return topicList;
        }

        /// <summary>
        /// 根据topicId修改topic.
        /// </summary>
        /// <param name="topicId">讨论课的ID</param>
        /// <param name="topic">修改后的讨论课</param>
        /// <exception cref="T:System.ArgumentException">Id格式错误时抛出</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.TopicNotFoundException">无此topic错误</exception>
        public void UpdateTopicByTopicId(long topicId, Topic topic)
        {
            if (topicId <= 0) throw new ArgumentException("Id格式错误");
            Topic topicToUpdate = _db.Topic.Find(topicId);//查找出要修改的topic
            if (topicToUpdate == null) throw new TopicNotFoundException();//找不到topic抛出异常

            //修改信息,由于直接topicToUpdate=topic不起作用，所以逐个属性赋值
            topicToUpdate.Name = topic.Name;
            topicToUpdate.Description = topic.Description;
            if (topic.GroupStudentLimit != null) topicToUpdate.GroupNumberLimit = topic.GroupNumberLimit;
            if (topic.GroupStudentLimit != null) topicToUpdate.GroupStudentLimit = topic.GroupStudentLimit;

            _db.SaveChanges();//提交事务
        }

        public List<SeminarGroupTopic> ListSeminarGroupTopicByGroupId(long groupId)
        {
            SeminarGroup group;
            if (groupId <= 0) throw new ArgumentException("groupId不合法");
            if ((group = _db.SeminarGroup.Find(groupId)) == null)
                throw new GroupNotFoundException();

            List<SeminarGroupTopic> sgtList = _db.SeminarGroupTopic.Include(x => x.Topic)
                .Include(x => x.SeminarGroup)
                .ToList();
            List<SeminarGroupTopic> sgtListToReturn = sgtList.FindAll(x => x.SeminarGroup == group);
            return sgtListToReturn;
        }

        public int GetRestTopicById(long topicId, long classId)
        {
            throw new NotImplementedException();
        }
    }
}
