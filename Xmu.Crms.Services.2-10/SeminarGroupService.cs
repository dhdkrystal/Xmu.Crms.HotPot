using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Services.Group2_10
{

    public class SeminarGroupService : ISeminarGroupService
    {
        private CrmsContext _db;
        private IUserService _us;

        public SeminarGroupService(CrmsContext db, IUserService us)
        {
            _db = db;
            _us = us;
        }

        /*
         * author：许驹雄
         * QQ：980753915
         */

        /// <summary>
        /// 成为组长.
        /// 同学按小组id和自身id成为组长
        /// </summary>
        /// <param name="groupId">小组id</param>
        /// <param name="userId">学生id</param>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.UserNotFoundException">不存在该学生</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.GroupNotFoundException">未找到小组</exception>
        /// <exception cref="T:System.InvalidOperationException">已经有组长了</exception>
        public void AssignLeaderById(long groupId, long userId)
        {
            UserInfo _user;
            SeminarGroup _group;

            //id格式异常
            if (groupId <= 0 || userId <= 0) throw new System.ArgumentException("id格式错误");

            //找到user
            if ((_user = _db.UserInfo.Find(userId)) == null) throw new UserNotFoundException();

            //找到group并连接user
            _group = _db.SeminarGroup.Include(sg => sg.Leader).Single(sg => sg.Id == groupId);
            if (_group == null) throw new GroupNotFoundException();

            //已经有组长，抛出异常
            if (_group.Leader != null) throw new System.InvalidOperationException("已经有组长了");

            //设置组长
            _group.Leader = _user;

            //保存更改
            _db.SaveChanges();
        }

        ///新增定时器方法.
        ///<p>随机分组情况下，签到结束后十分钟给没有选择话题的小组分配话题<br>
        ///@author qinlingyun
        /// @param seminarId 讨论课的id
        /// @param seminarGroupId 小组的id
        /// @exception IllegalArgumentException 信息不合法，id格式错误
        /// @exception SeminarNotFoundException 未找到讨论课
        ///@exception GroupNotFoundException 未找到小组
        public void AutomaticallyAllotTopic(long seminarId)
        {
            if (seminarId <= 0)
                throw new System.ArgumentException("id格式错误");

            if (_db.Seminar.Find(seminarId) == null)
                throw new SeminarNotFoundException();//未找到讨论课

            //还没有选题的小组列表
            List<SeminarGroup> grouplist_not_have_topic = new List<SeminarGroup>();

            //筛选出这堂讨论课的小组列表
            List<SeminarGroup> grouplist_this_seminar = _db.SeminarGroup
                .Include(sg => sg.Seminar)
                .Where(sg => sg.Seminar.Id == seminarId)
                .ToList();

            //对每个SeminarGroup
            foreach (SeminarGroup sg in grouplist_this_seminar)
            {
                if (_db.SeminarGroupTopic
                    .Include(sgt => sgt.SeminarGroup)
                    //如果和它的Id相同的SeminarGroupTopic.SeminarGroup.Id行数为0
                    .Where(sgt => sg.Id == sgt.SeminarGroup.Id)
                    .Count() == 0)
                    //说明该小组未选题记录，
                    //将其添加进没有选题的小组列表
                    grouplist_not_have_topic.Add(sg);
            }

            //准备添加的SeminarGroupTopic表项
            SeminarGroupTopic new_sgt = new SeminarGroupTopic();

            //筛选出这堂讨论课的话题列表
            List<Topic> topiclist_this_seminar = _db.Topic
                .Include(tp => tp.Seminar)
                .Where(tp => tp.Seminar.Id == seminarId)
                .ToList();

            foreach (SeminarGroup sg_to_allot_topic in grouplist_not_have_topic)
            {
                new_sgt.SeminarGroup = sg_to_allot_topic;

                //被选择最少的话题
                Topic topic_be_chosen_least = null;
                int min_chosen_count = int.MaxValue;

                //对话题表中的每个话题
                foreach (Topic tp in topiclist_this_seminar)
                {
                    //统计被选择次数
                    int chosen_count = _db.SeminarGroupTopic
                        .Include(sgt => sgt.Topic)
                        .Where(sgt => sgt.Topic.Id == tp.Id)
                        .Count();

                    if (chosen_count < min_chosen_count)
                    {
                        min_chosen_count = chosen_count;
                        topic_be_chosen_least = tp;
                    }
                }//循环完之后，就找出当前被选择最少的话题

                //新表项的话题就是这个最少话题
                new_sgt.Topic = topic_be_chosen_least;

                //添加新表项
                _db.SeminarGroupTopic.Add(new_sgt);

                _db.SaveChanges();

            }//为没有选题的小组列表中所有组分配完话题

        }

        /// <summary>
        /// 定时器方法：自动分组.
        /// </summary>
        /// 
        /// 根据讨论课id和班级id，对签到的学生进行自动分组
        /// 
        /// <param name="seminarId">讨论课的id</param>
        /// <param name="classId">班级的id</param>
        /// <returns>Boolean 自动分组成功返回true，否则返回false</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IUserService.ListAttendanceById(System.Int64,System.Int64)"/>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.SeminarNotFoundException">未找到讨论课</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.ClassNotFoundException">未找到班级</exception>
        public void AutomaticallyGrouping(long seminarId, long classId)
        {

            List<Attendance> attendances;
            List<SeminarGroup> groups;
            Seminar seminar;
            ClassInfo classInfo;

            //调用ListAttendanceById获取签到学生信息
            attendances = _us.ListAttendanceById(classId, seminarId).ToList();

            //获取讨论课
            if ((seminar = _db.Seminar.Find(seminarId)) == null)
                throw new SeminarNotFoundException();

            //获取班级
            if ((classInfo = _db.ClassInfo.Find(classId)) == null)
                throw new ClassNotFoundException();

            var studentcount= _db.CourseSelection.Where(c => c.ClassId == classId).ToList().Count();
            var groupcount = studentcount / 5+1;
            while (groupcount > 0) {
                SeminarGroup group = new SeminarGroup
                {
                    ClassId = classId,
                    SeminarId = seminarId,
                   
                };
                InsertSeminarGroupBySeminarId(seminarId,classId,group);
                groupcount--;
            }
            _db.SaveChanges();
            //获取该讨论课的所有队伍
            groups = _db.SeminarGroup
                .Include(x => x.Seminar)
                .Include(x => x.ClassInfo)
                .Where(x => (x.Seminar.Id == seminarId && x.ClassInfo.Id == classId)).ToList();

            //对于每个签到的学生
            foreach (Attendance atten in attendances)
            {
                //如果是出勤或迟到状态，进入分配
                if (atten.AttendanceStatus == AttendanceStatus.Present)
                {
                    //获取该学生（需要返回的list有include学生对象）
                    UserInfo student = atten.Student;

                    try
                    {
                        //如果该学生已经分配了小组，就不需要再分配
                        GetSeminarGroupById(seminarId, student.Id);
                        continue;
                    }
                    //抛异常说明没有分配，则进入分配
                    catch (GroupNotFoundException)
                    {
                        //记录当前小组的最少人数和对应的组
                        int MinNum = 99999;
                        SeminarGroup InsertGroup = null;

                        //找到每个组的成员个数
                        foreach (SeminarGroup group in groups)
                        {
                            int num = 0;
                            //找到该组的成员个数
                            num = _db.SeminarGroupMember.Include(x => x.SeminarGroup)
                                .Where(x => x.SeminarGroup == group).ToList().Count;

                            //如果个数少于当前最少个数，赋值给MinNum和InsertGroup
                            if (num < MinNum)
                            {
                                MinNum = num;
                                InsertGroup = group;
                            }
                        }

                        //如果出错，返回false
                        if (MinNum == 99999 || InsertGroup == null) throw new InvalidOperationException("随机分组失败");

                        //往人数最少的组添加一个成员
                        InsertSeminarGroupMemberById(student.Id, InsertGroup.Id);
                    }

                }
            }
            return;

        }


        /// <summary>
        /// 删除讨论课小组.
        /// </summary>
        /// <param name="seminarGroupId">讨论课小组的id</param>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISeminarGroupService.DeleteSeminarGroupMemberBySeminarGroupId(System.Int64)"/>
        public void DeleteSeminarGroupByGroupId(long seminarGroupId)
        {
            SeminarGroup group;//删除seminar_group表数据
            List<SeminarGroupMember> GroupMemberss;//删除seminar_group_member表数据
            List<SeminarGroupTopic> GroupTopics;//删除seminar_group_topic表数据
            List<StudentScoreGroup> GroupScores;//删除student_score_group表数据
            List<long> TopicId = new List<long>();

            //id格式错误
            if (seminarGroupId <= 0) throw new System.ArgumentException("id格式错误");

            //找到小组
            if ((group = _db.SeminarGroup.Find(seminarGroupId)) == null)
                throw new GroupNotFoundException();

            //找到SeminarGroupMember
            GroupMemberss = _db.SeminarGroupMember.Include(x => x.SeminarGroup)
                .Where(x => x.SeminarGroup == group).ToList();

            //找到SeminarGroupTopic
            GroupTopics = _db.SeminarGroupTopic.Include(x => x.SeminarGroup)
                .Where(x => x.SeminarGroup == group).ToList();

            //把这些topicId集合起来
            foreach (SeminarGroupTopic topic in GroupTopics)
                TopicId.Add(topic.Id);

            //用TopicId找到GroupScores
            GroupScores = _db.StudentScoreGroup.Include(x => x.SeminarGroupTopic)
                .Where(x => TopicId.Contains(x.SeminarGroupTopic.Id)).ToList();

            //删除小组选择主题信息
            _db.SeminarGroupTopic.RemoveRange(GroupTopics);

            //删除小组成员
            _db.SeminarGroupMember.RemoveRange(GroupMemberss);

            //删除小组分数信息
            _db.StudentScoreGroup.RemoveRange(GroupScores);

            //删除小组
            _db.SeminarGroup.Remove(group);

            //保存更改
            _db.SaveChanges();
        }

        /// <summary>
        /// 按seminarId删除讨论课小组信息.
        /// 根据seminarId获得SeminarGroup，然后根据SeminarGroupId删除SeminarGroupMember信息，最后再删除SeminarGroup信息
        /// </summary>
        /// <param name="seminarId">讨论课Id</param>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISeminarGroupService.ListSeminarGroupBySeminarId(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISeminarGroupService.DeleteSeminarGroupMemberBySeminarGroupId(System.Int64)"/>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        public void DeleteSeminarGroupBySeminarId(long seminarId)
        {
            Seminar _seminar;
            List<SeminarGroup> Groups;

            //id格式错误
            if (seminarId <= 0) throw new System.ArgumentException("id格式错误");

            //讨论课不存在
            if ((_seminar = _db.Seminar.Find(seminarId)) == null)
                throw new SeminarNotFoundException();

            //获取小组列表
            Groups = _db.SeminarGroup.Include(x => x.Seminar)
                .Where(x => x.Seminar == _seminar).ToList();

            //对每个小组，调用DeleteSeminarGroupByGroupId函数
            foreach (SeminarGroup group in Groups)
                DeleteSeminarGroupByGroupId(group.Id);

        }

        /// <summary>
        /// 按seminarGroupId删除SeminarGroupMember信息.
        /// </summary>
        /// <param name="seminarGroupId">讨论课小组Id</param>
        public void DeleteSeminarGroupMemberBySeminarGroupId(long seminarGroupId)
        {
            List<SeminarGroupMember> members;

            //找不到小组
            if (_db.SeminarGroup.Find(seminarGroupId) == null)
                throw new GroupNotFoundException();

            //找到成员信息
            members = _db.SeminarGroupMember.Include(x => x.SeminarGroup)
                .Where(x => x.SeminarGroup.Id == seminarGroupId).ToList();

            //删除成员
            _db.SeminarGroupMember.RemoveRange(members);

            //保存更改
            _db.SaveChanges();
        }

        ///<summary>
        ///删除小组成员.
        ///在指定小组成员表下删除一个小组成员信息
        ///</summary>
        ///<param name="seminarGroupId">小组的id</param>
        ///<param name="userId">成员id</param>
        public void DeleteSeminarGroupMemberById(long seminarGroupId, long userId)
        {
            SeminarGroupMember record;

            //找不到小组
            if (_db.SeminarGroup.Find(seminarGroupId) == null)
                throw new GroupNotFoundException();

            //找不到用户
            if (_db.UserInfo.Find(userId) == null)
                throw new UserNotFoundException();

            //找到要删除的记录
            record = _db.SeminarGroupMember
                        .Include(x => x.SeminarGroup)
                        .Include(x => x.Student)
                        .Single(x => x.SeminarGroup.Id == seminarGroupId && x.Student.Id == userId);

            //删除记录
            _db.SeminarGroupMember.Remove(record);

            _db.SaveChanges();
        }


        /// <summary>
        /// 查询讨论课小组.
        /// 按照id查询某一讨论课小组的信息（包括成员）
        /// </summary>
        /// <param name="groupId">小组的id</param>
        /// <returns>seminarGroup 讨论课小组对象，若未找到相关小组返回空(null)</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISeminarGroupService.ListSeminarGroupMemberByGroupId(System.Int64)"/>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.GroupNotFoundException">未找到小组</exception>
        public SeminarGroup GetSeminarGroupByGroupId(long groupId)
        {
            SeminarGroup group;

            //id不合法
            if (groupId <= 0) throw new System.ArgumentException("id格式错误");

            //找到小组
            try
            {
                group = _db.SeminarGroup
                    //包含Seminar
                    .Include(x => x.Seminar)
                    //包含Class
                    .Include(x => x.ClassInfo)
                    //包含Leader
                    .Include(x => x.Leader)
                    //取出小组
                    .Single(x => x.Id == groupId);
            }
            catch (Exception e)

            {
                throw new GroupNotFoundException();
            }

            return group;
        }

        /// <summary>
        /// 根据讨论课Id及用户id，获得该用户所在的讨论课的小组的信息.
        /// </summary>
        /// <param name="seminarId">(讨论课的id)</param>
        /// <param name="userId"></param>
        /// <returns>SeminarGroup Group的相关信息</returns>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.GroupNotFoundException">未找到小组</exception>
        public SeminarGroup GetSeminarGroupById(long seminarId, long userId)
        {
            List<SeminarGroup> SeminarGroups;
            Seminar seminar;

            //id格式错误
            if (seminarId <= 0 || userId <= 0) throw new System.ArgumentException("id格式错误");

            //找到讨论课
            if ((seminar = _db.Seminar.Find(seminarId)) == null)
                throw new SeminarNotFoundException();

            //找出Seminar里的所有小组
            SeminarGroups = _db.SeminarGroup.Include(x => x.Seminar)
                .Where(x => x.Seminar == seminar).ToList();

            //在这些小组中，找到成员含有该user的小组
            foreach (SeminarGroup TempGroup in SeminarGroups)
            {
                List<SeminarGroupMember> members;

                //找到这个小组的所有成员
                members = _db.SeminarGroupMember
                    //连接SeminarGroup
                    .Include(x => x.SeminarGroup)
                    //连接Student
                    .Include(x => x.Student)
                    //找到该组的成员
                    .Where(x => x.SeminarGroup.Id == TempGroup.Id)
                    .ToList();

                //遍历成员，如果该成员id和userId一样，把这个小组返回
                foreach (SeminarGroupMember mem in members)
                    if (mem.Student.Id == userId) return TempGroup;
            }

            //没找到小组，抛出异常
            throw new GroupNotFoundException();
        }

        /// <summary>
        /// 查询讨论课小组队长id.
        /// </summary>
        /// <param name="groupId">要查询的讨论课小组id</param>
        /// <returns>leaderId 讨论课小组队长id</returns>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.GroupNotFoundException">未找到小组</exception>
        public long GetSeminarGroupLeaderByGroupId(long groupId)
        {
            SeminarGroup group;

            //找到group并连接leader
            if ((group = _db.SeminarGroup.Include(x => x.Leader)
                .Single(x => x.Id == groupId)) == null)
                throw new GroupNotFoundException();

            return group.Leader.Id;
        }

        /// <summary>
        /// 获取学生所在讨论课队长.
        /// </summary>
        /// <param name="userId">用户的id</param>
        /// <param name="seminarId">讨论课id</param>
        /// <returns>long 讨论课小组的队长id，若未找到相关小组队长返回空(null)</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISeminarGroupService.GetSeminarGroupById(System.Int64,System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISeminarGroupService.GetSeminarGroupLeaderByGroupId(System.Int64)"/>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        public long GetSeminarGroupLeaderById(long userId, long seminarId)
        {
            SeminarGroup group;

            //调用Get SeminarGroupById方法，获取学生所在小组
            group = GetSeminarGroupById(seminarId, userId);

            //返回队长id
            return group.Leader.Id;
        }

        /***************************我是萌萌哒的分界线*******************************************/
        /*
         * author：孙仲玄
         * QQ：1731744887
         */

        /// <summary>
        /// 创建讨论课小组.
        /// </summary>
        /// <param name="seminarId">讨论课的id</param>
        /// <param name="seminarGroup">小组信息</param>
        /// <returns>long 返回该小组的id</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISeminarGroupService.InsertSeminarGroupMemberById(System.Int64,System.Int64)"/>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        public long InsertSeminarGroupBySeminarId(long seminarId, long classId, SeminarGroup seminarGroup)
        {
            //id <= 0
            if (seminarId <= 0 || classId <= 0) throw new ArgumentException("id格式错误");
            Seminar seminar = _db.Seminar.Find(seminarId);
            if (seminar == null) throw new SeminarNotFoundException();
            ClassInfo classInfo = _db.ClassInfo.Find(classId);
            if (classInfo == null) throw new ClassNotFoundException();

            seminarGroup.Seminar = seminar;
            seminarGroup.ClassInfo = classInfo;
            _db.SeminarGroup.Add(seminarGroup);
            _db.SaveChanges();

            return seminarGroup.Id

;
        }


        /// <summary>
        /// 创建小组成员信息.
        /// 将小组成员加入某一小组
        /// </summary>
        /// <param name="groupId">小组的id</param>
        /// <param name="seminarGroupMember">小组成员信息</param>
        /// <returns>long 返回该小组成员表的id</returns>
        public long InsertSeminarGroupMemberByGroupId(long groupId, SeminarGroupMember seminarGroupMember)
        {
            if (groupId <= 0) throw new ArgumentException("groupId格式错误");
            SeminarGroup sg = _db.SeminarGroup.Find(groupId);
            if (sg == null) throw new GroupNotFoundException();

            seminarGroupMember.SeminarGroup = sg;
            _db.SeminarGroupMember.Add(seminarGroupMember);
            _db.SaveChanges();

            return seminarGroupMember.Id;
        }


        /// <summary>
        /// 将学生加入讨论课小组.
        /// </summary>
        /// <param name="userId">学生的id</param>
        /// <param name="groupId">要加入讨论课小组的id</param>
        /// <returns>long 该条记录的id</returns>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.GroupNotFoundException">未找到小组</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.UserNotFoundException">不存在该学生</exception>
        /// <exception cref="T:System.InvalidOperationException">待添加学生已经在小组里了</exception>
        public long InsertSeminarGroupMemberById(long userId, long groupId)
        {
            UserInfo _user;
            SeminarGroup _sg;

            //id <= 0
            if (userId <= 0 || groupId <= 0)
                throw new ArgumentException("id格式错误");

            //未找到小组
            if ((_sg = _db.SeminarGroup.Find(groupId)) == null)
                throw new GroupNotFoundException();

            //不存在该学生
            if ((_user = _db.UserInfo.Find(userId)) == null)
                throw new UserNotFoundException();

            //待添加学生已经在小组里
            if (_db.SeminarGroupMember
                .Include(sgt => sgt.SeminarGroup)
                .Include(sgt => sgt.Student)
                .Where(_sgm => _sgm.SeminarGroup.Id == groupId && _sgm.Student.Id == userId)
                .ToList().Count != 0)
                throw new System.InvalidOperationException("待添加学生已经在小组里了");

            //新建SeminarGroupMember对象
            SeminarGroupMember sgm = new SeminarGroupMember();
            sgm.SeminarGroup = _sg;
            sgm.Student = _user;

            _db.SeminarGroupMember.Add(sgm);

            _db.SaveChanges();

            return sgm.Id;
        }

        /// <summary>
        /// 小组按id选择话题.
        /// </summary>
        /// <param name="groupId">小组id</param>
        /// <param name="topicId">话题id</param>
        /// <returns>返回</returns>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.GroupNotFoundException">未找到小组</exception>
        public long InsertTopicByGroupId(long groupId, long topicId)
        {
            //id <= 0
            if (groupId <= 0 || topicId <= 0)
                throw new ArgumentException("id格式错误");

            //未找到小组
            SeminarGroup _sg;
            if ((_sg = _db.SeminarGroup.Find(groupId)) == null)
                throw new GroupNotFoundException();

            //新建SeminarGroupTopic对象
            SeminarGroupTopic sgt = new SeminarGroupTopic();
            sgt.SeminarGroup = _sg;
            sgt.Topic = _db.Topic.Find(topicId);

            _db.SeminarGroupTopic.Add(sgt);

            _db.SaveChanges();

            return sgt.Id;
        }

        /// <summary>
        /// 根据话题Id获得选择该话题的所有小组的信息.
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns>List&lt;GroupBO&gt;所有选择该话题的所有group的信息</returns>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.GroupNotFoundException">未找到小组</exception>
        public IList<SeminarGroup> ListGroupByTopicId(long topicId)
        {
            //id <= 0
            if (topicId <= 0)
                throw new ArgumentException("id格式错误");

            List<SeminarGroupTopic> sgtlist = new List<SeminarGroupTopic>();
            sgtlist = _db.SeminarGroupTopic
                //加载Topic与SeminarGroup
                .Include(sgt => sgt.Topic)
                .Include(sgt => sgt.SeminarGroup)
                //筛选topicId
                .Where(sgt => sgt.Topic.Id == topicId)
                .ToList();

            //未找到小组
            if (sgtlist.Count == 0)
                throw new SeminarNotFoundException();

            //将sgtlist中的每个SeminarGroup组成一个list
            List<SeminarGroup> sglist = new List<SeminarGroup>();
            sgtlist.ForEach(sgt => sglist.Add(sgt.SeminarGroup));

            return sglist;
        }

        /// <summary>
        /// 按seminarId获取SeminarGroup.
        /// </summary>
        /// <param name="seminarId">课程Id</param>
        /// <returns>讨论课小组列表</returns>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.SeminarNotFoundException">未找到小组</exception>
        public IList<SeminarGroup> ListSeminarGroupBySeminarId(long seminarId)
        {
            //id <= 0
            if (seminarId <= 0)
                throw new ArgumentException("id格式错误");

            List<SeminarGroup> sglist = new List<SeminarGroup>();

            sglist = _db.SeminarGroup
                //加载Seminar
                .Include(sg => sg.Seminar)
                //筛选seminarId
                .Where(sg => sg.Seminar.Id == seminarId)
                .ToList();

            //未找到小组
            if (sglist.Count == 0)
                throw new SeminarNotFoundException();
            else
                return sglist;
        }


        /// <summary>
        /// 获取某学生所有的讨论课小组.
        /// </summary>
        /// <param name="userId">学生id</param>
        /// <returns>list 讨论课小组列表</returns>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        public IList<SeminarGroup> ListSeminarGroupIdByStudentId(long userId)
        {
            //id <= 0
            if (userId <= 0)
                throw new ArgumentException("id格式错误");

            List<SeminarGroupMember> sgmlist = _db.SeminarGroupMember
                //加载SeminarGroup与Student
                .Include(sgm => sgm.SeminarGroup)
                .Include(sgm => sgm.Student)
                //筛选userId，生成SeminarGroupMember的list
                .Where(sgm => sgm.Student.Id == userId)
                .ToList();

            //SeminarGroup
            List<SeminarGroup> sglist = new List<SeminarGroup>();
            sgmlist.ForEach(sgm => sglist.Add(sgm.SeminarGroup));

            return sglist;
        }


        /// <summary>
        /// 查询讨论课小组成员.
        /// </summary>
        /// <param name="groupId">要查询的讨论课小组id</param>
        /// <returns>List 讨论课小组成员信息</returns>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.GroupNotFoundException">未找到小组</exception>
        public IList<UserInfo> ListSeminarGroupMemberByGroupId(long groupId)
        {
            //id <= 0
            if (groupId <= 0)
                throw new ArgumentException("id格式错误");

            //未找到小组
            if (_db.SeminarGroup.Find(groupId) == null)
                throw new GroupNotFoundException();

            //加载SeminarGroup与Student
            List<SeminarGroupMember> memlist = _db.SeminarGroupMember
                .Include(sgm => sgm.SeminarGroup)
                .Include(sgm => sgm.Student)
            //筛选groupId，生成SeminarGroupMember的list
                .Where(sgm => sgm.SeminarGroup.Id == groupId)
                .ToList();

            //取出Student组成list
            List<UserInfo> userlist = new List<UserInfo>();
            memlist.ForEach(sgm => userlist.Add(sgm.Student));

            return userlist;
        }

        /// <summary>
        /// 组长辞职.
        /// 同学按小组id和自身id,辞掉组长职位
        /// </summary>
        /// <param name="groupId">小组id</param>
        /// <param name="userId">学生id</param>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.UserNotFoundException">不存在该学生</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.GroupNotFoundException">未找到小组</exception>
        /// <exception cref="T:System.InvalidOperationException">学生不是组长</exception>
        public void ResignLeaderById(long groupId, long userId)
        {
            UserInfo _user;
            SeminarGroup _group;

            //id <= 0
            if (groupId <= 0 || userId <= 0)
                throw new ArgumentException("id格式错误");

            //不存在该学生
            if ((_user = _db.UserInfo.Find(userId)) == null)
                throw new UserNotFoundException();

            //连接group与leader
            _group = _db.SeminarGroup.Include(sg => sg.Leader).Single(sg => sg.Id == groupId);

            //未找到小组
            if (_group == null) throw new GroupNotFoundException();
            //传入id和组长id不符
            if (_user.Id != _group.Leader.Id)
                throw new System.InvalidOperationException("学生不是组长");

            //辞掉组长
            _group.Leader = null;

            _db.SaveChanges();
        }

        string ISeminarGroupService.InsertTopicByGroupId(long groupId, long topicId)
        {
            throw new NotImplementedException();
        }
    }
}
