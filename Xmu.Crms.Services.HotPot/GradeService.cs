using System;
using System.Collections.Generic;
using System.Linq;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;
using Microsoft.EntityFrameworkCore;

namespace Xmu.Crms.Services.HotPot
{
    public class GradeService : IGradeService
    {
        private readonly CrmsContext _db;
        private readonly ISeminarGroupService _iSeminarGroupService;
        private readonly ISeminarService _iSeminarService;
        private readonly ITopicService _iTopicService;
        public GradeService(CrmsContext db, ISeminarGroupService iSeminarGroupService, ISeminarService iSeminarService, ITopicService iTopicService)
        {
            _db = db;
            _iSeminarGroupService = iSeminarGroupService;
            _iSeminarService = iSeminarService;
            _iTopicService = iTopicService;
        }

        //快排,根据小组分数进行按543比例给分时用到。s表示id编号，a表示分数。按a进行排序。相应移动s
        void quick_sort(long[] s, double[] a, int l, int r)
        {
            if (l < r)
            {
                int i = l, j = r;
                long x = s[l];
                double y = a[l];
                while (i < j)
                {
                    while (i < j && a[j] <= y)
                        j--;
                    if (i < j)
                    {
                        s[i] = s[j];
                        a[i] = a[j];
                        i++;
                    }
                    while (i < j && a[i] > y)
                        i++;
                    if (i < j)
                    {
                        s[j] = s[i];
                        a[j] = a[i];
                        j--;
                    }
                }
                s[i] = x;
                a[i] = y;
                quick_sort(s, a, l, i);
                quick_sort(s, a, i + 1, r);
            }
        }

        /**
        * 按seminarGroupTopicId删除学生打分表.
        *
         * @param seminarGroupTopicId  小组话题表的Id
         * @throws IllegalArgumentException topicId格式错误时抛出
         * @author FJL
         */
        public void DeleteStudentScoreGroupByTopicId(long topicId)//成功测试！！！！！！
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    //查找到所有的seminarGroupTopic
                    //查找到所有的studentScoreGroup
                    List<SeminarGroupTopic> seminarGroupTopicList = _db.SeminarGroupTopic.Include(u => u.Topic).Where(u => u.Topic.Id == topicId).ToList();
                    if (seminarGroupTopicList == null)
                        throw new GroupNotFoundException();
                    foreach (var seminarGroupTopic in seminarGroupTopicList)
                    {
                        List<StudentScoreGroup> studentScoreGroupList = _db.StudentScoreGroup.Include(u => u.SeminarGroupTopic).Where(u => u.SeminarGroupTopic.Id == seminarGroupTopic.Id).ToList();
                        foreach (var studentScoreGroup in studentScoreGroupList)
                        {
                            //将实体附加到对象管理器中
                            _db.StudentScoreGroup.Attach(studentScoreGroup);
                            //删除
                            _db.StudentScoreGroup.Remove(studentScoreGroup);
                        }
                    }
                    _db.SaveChanges();
                }
                catch { scope.Rollback(); throw; }
            }
        }
        /// <summary>
        /// 仅作为普通方法，被下面的定时器方法调用.
        /// 讨论课结束后计算展示得分.
        /// @author fengjinliu
        /// 条件: 讨论课已结束  *GradeService
        /// </summary>
        /// <param name="seminarId">讨论课id</param>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        public void CountPresentationGrade(long seminarId)//成功测试！！！！！！
        {
            //List<SeminarGroup> seminarGroupList = _db.SeminarGroup.Include(u => u.Seminar).Where(u => u.Seminar.Id == seminarId).ToList();
            List<Topic> topicList = _db.Topic.Include(u => u.Seminar).Where(u => u.Seminar.Id == seminarId).ToList();
            foreach (var topic in topicList)
            {
                //通过seminarGroupId获得List<SeminarGroupTopic>  
                //通过seminarGrouptopicId获得List<StudentScoreGroup>
                long[] idList;
                double[] gradeList;

                //获取选择该topic的所有小组
                var seminarGroupTopicList = _db.SeminarGroupTopic.Include(u => u.SeminarGroup).Include(u => u.Topic).Where(u => u.Topic.Id == topic.Id).ToList();
                if (seminarGroupTopicList == null) throw new GroupNotFoundException();
                else
                {
                    idList = new long[seminarGroupTopicList.Count];
                    gradeList = new double[seminarGroupTopicList.Count];

                    int groupNumber = 0;

                    //计算一个小组的所有学生打分情况
                    foreach (var i in seminarGroupTopicList)
                    {
                        //List<StudentScoreGroup> studentScoreList = new List<StudentScoreGroup>();
                        //获取学生打分列表
                        var studentScoreList = _db.StudentScoreGroup.Where(u => u.SeminarGroupTopic.Id == i.Id).ToList();
                        if (studentScoreList == null)//该组没有被打分
                            seminarGroupTopicList.Remove(i);
                        int? grade = 0; int k = 0;
                        foreach (var g in studentScoreList)
                        {
                            grade += g.Grade;
                            k++;
                        }
                        double avg = (double)grade / k;

                        //将小组该讨论课平均分和Id保存
                        idList[groupNumber] = i.Id;
                        gradeList[groupNumber] = avg;
                        groupNumber++;
                    }
                    //将小组成绩从大到小排序
                    quick_sort(idList, gradeList, 0, groupNumber - 1);

                    Seminar seminar;
                    ClassInfo classInfo;

                    seminar = _db.Seminar.Include(u => u.Course).Where(u => u.Id == seminarId).SingleOrDefault();
                    if (seminar == null) throw new SeminarNotFoundException();
                    classInfo = _db.ClassInfo.Where(u => u.Id == seminar.Course.Id).SingleOrDefault();
                    if (classInfo == null) throw new ClassNotFoundException();

                    //各小组按比例给分
                    int Five = Convert.ToInt32(groupNumber * classInfo.FivePointPercentage * 0.01);//考虑到四舍五入。使用convert.toint32
                    int Four = Convert.ToInt32(groupNumber * classInfo.FourPointPercentage * 0.01);
                    int Three = Convert.ToInt32(groupNumber * classInfo.ThreePointPercentage * 0.01);
                    if (Five + Four + Three != groupNumber)
                        throw new ArgumentException("classInfo规定比例失误！");//考虑到两个.5均入位产生过大的情况
                    for (int i = 0; i < groupNumber; i++)
                    {

                        SeminarGroupTopic seminarGroupTopic = _db.SeminarGroupTopic.SingleOrDefault(s => s.Id == idList[i]);
                        //如果找不到该组
                        if (seminarGroupTopic == null)
                        {
                            throw new GroupNotFoundException();
                        }
                        //更新报告分
                        if (i >= 0 && i < Five) seminarGroupTopic.PresentationGrade = 5;
                        else if (i >= Five && i < Five + Four) seminarGroupTopic.PresentationGrade = 4;
                        else seminarGroupTopic.PresentationGrade = 3;
                        _db.SaveChanges();
                    }

                }//if end

            }//foreach topic end
        }



        /// <summary>
        /// 定时器方法:讨论课结束后计算本次讨论课得分.
        /// @author fengjinliu
        /// 条件: 讨论课已结束，展示得分已算出  *GradeService
        /// </summary>
        /// <param name="seminarId">讨论课id</param>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// 

        public void CountGroupGradeBySerminarId(long seminarId)//成功测试！！！1
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                List<SeminarGroup> seminarGroupList = _db.SeminarGroup.Include(u => u.Seminar).Include(u => u.ClassInfo).Where(u => u.Seminar.Id == seminarId).ToList();
                //根据seminarGroupList中元素，依次计算
                //SeminarGroup实体中保存了ClassInfo实体对象，可以查到成绩计算方法
                double[] tempTotalGrade = new double[seminarGroupList.Count];
                long[] tempId = new long[seminarGroupList.Count];
                foreach (var seminarGroup in seminarGroupList)
                {
                    //根据seminarGroupId获得seminarGroupTopicList,计算每一个seminarGroup的展示分数
                    List<SeminarGroupTopic> seminarGroupTopicList = _db.SeminarGroupTopic.Include(u => u.SeminarGroup).Where(u => u.SeminarGroup.Id == seminarGroup.Id).ToList();
                    if (seminarGroupTopicList == null)//该组没有展示分数
                        seminarGroupList.Remove(seminarGroup);
                    int? grade = 0;
                    int number = 0;
                    foreach (var seminarGroupTopic in seminarGroupTopicList)
                    {
                        grade += seminarGroupTopic.PresentationGrade;
                        number++;
                    }
                    try
                    {
                        //更新seminarGroup中的展示成绩
                        int? avgPreGrade = grade / number;
                        _db.SeminarGroup.Attach(seminarGroup);
                        seminarGroup.PresentationGrade = avgPreGrade;
                        _db.SaveChanges();
                    }
                    catch
                    {
                        scope.Rollback(); throw;
                    }

                }
                for (int i = 0; i < seminarGroupList.Count; i++)
                {
                    tempTotalGrade[i] = ((double)(seminarGroupList[i].ClassInfo.PresentationPercentage * seminarGroupList[i].PresentationGrade
                        + seminarGroupList[i].ClassInfo.ReportPercentage * seminarGroupList[i].ReportGrade)) / 100;
                    tempId[i] = seminarGroupList[i].Id;
                }
                //排序
                //将小组总成绩从大到小排序
                quick_sort(tempId, tempTotalGrade, 0, seminarGroupList.Count - 1);
                //根据排序和比例计算组数，四舍五入
                int Five = Convert.ToInt32(seminarGroupList.Count * seminarGroupList[0].ClassInfo.FivePointPercentage * 0.01);
                int Four = Convert.ToInt32(seminarGroupList.Count * seminarGroupList[0].ClassInfo.FourPointPercentage * 0.01);
                int Three = Convert.ToInt32(seminarGroupList.Count * seminarGroupList[0].ClassInfo.ThreePointPercentage * 0.01);
                if (Five + Four + Three != seminarGroupList.Count)
                    throw new ArgumentException("比例设置问题，与人数冲突");
                //各小组按比例给分
                for (int i = 0; i < seminarGroupList.Count; i++)
                {

                    SeminarGroup seminarGroup = _db.SeminarGroup.SingleOrDefault(s => s.Id == tempId[i]);
                    //更新报告分

                    if (i >= 0 && i < Five) { seminarGroup.FinalGrade = 5; _db.SaveChanges(); }//每一步都保存。我真的是怕了。
                    else if (i >= Five && i < Five + Four) { seminarGroup.FinalGrade = 4; _db.SaveChanges(); }
                    else if (i >= Four + Five && i < seminarGroupList.Count) { seminarGroup.FinalGrade = 3; _db.SaveChanges(); }

                    _db.SaveChanges();
                }
                _db.SaveChanges();
            }//using scope end
        }




        //@author cuisy

        //<summary>按讨论课小组id获取讨论课小组</summary>
        //<param name="userId">讨论课小组Id</param>
        //<param name="seminarGroupId">讨论课小组Id</param>
        //<returns>SeminarGroup 讨论课小组信息</returns>
        //<exception cref="T:System.ArgumentException">SeminarId格式错误</exception>
        //<exception cref="T:System.ArgumentException">UserId格式错误</exception>
        public SeminarGroup GetSeminarGroupBySeminarGroupId(long seminarGroupId)
        {
            //测试成功
            if (seminarGroupId <= 0)
                throw new ArgumentException(nameof(seminarGroupId));

            SeminarGroup group = _db.SeminarGroup.SingleOrDefault(c => c.Id == seminarGroupId)
                ?? throw new InvalidOperationException();

            return group;

        }

        //<summary>提交对其他小组的打分</summary>
        //<param name="topicId">话题Id</param>
        //<param name="userId">用户Id</param>
        //<param name="seminarId">讨论课Id</param>
        //<param name="groupId">小组Id</param>
        //<param name="grade">成绩</param>
        //<returns></returns>
        public void InsertGroupGradeByUserId(long topicId, long userId, long groupId, int grade)
        {
            //测试成功
            //数据库标准内Topic表中没有serial列，故在Xmu.Crms.Shared/Models/Topic.cs
            //和Xmu.Crms.Shared/Models/CrmsContext.cs中暂时删除serial实体

            if (topicId <= 0) throw new ArgumentException(nameof(topicId));
            if (userId <= 0) throw new ArgumentException(nameof(userId));
            if (groupId <= 0) throw new ArgumentException(nameof(groupId));

            var usr = _db.UserInfo.Find(userId) ?? throw new UserNotFoundException();
            var tpc = _iTopicService.GetSeminarGroupTopicById(topicId, groupId) ?? throw new TopicNotFoundException();

            StudentScoreGroup ssg = new StudentScoreGroup { Student = usr, SeminarGroupTopic = tpc, Grade = grade };
            _db.StudentScoreGroup.Add(ssg);
            _db.SaveChanges();
        }

        /// <summary>
        /// 按课程id获取学生该课程所有讨论课
        /// 通过课程id获取该课程下学生所有讨论课详细信息（包括成绩）
        /// </summary>
        /// <param name="userId">学生id</param>
        /// <param name="courseId">课程id</param>
        /// <returns>list 该课程下所有讨论课列表</returns>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISeminarService.ListSeminarByCourseId(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IGradeService.ListSeminarGradeByUserId(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISeminarGroupService.ListSeminarGroupBySeminarId(System.Int64)"/>
        public IList<SeminarGroup> ListSeminarGradeByCourseId(long userId, long courseId)
        {
            //测试完成
            List<SeminarGroup> seminarGroupList = new List<SeminarGroup>();

            //调用SeminarService 中 IList<Seminar> ListSeminarByCourseId(long courseId)方法
            IList<Seminar> seminarList = _iSeminarService.ListSeminarByCourseId(courseId);
            //调用SeminarGroupService 中 SeminarGroup GetSeminarGroupById(long seminarId, long userId)
            for (int i = 0; i < seminarList.Count; i++)
                try
                {
                    seminarGroupList.Add(_iSeminarGroupService.GetSeminarGroupById(seminarList[i].Id, userId));
                }
                catch (FixGroupNotFoundException)//不明白为什么要用这个exception
                { }
            return seminarGroupList;
        }
        /*public IList<SeminarGroup> ListSeminarGradeByCourseId(long userId, long courseId)
        {
            throw new NotImplementedException();
        }*/

        /// <summary>
        /// 按ID设置小组报告分.
        /// </summary>
        /// <param name="seminarGroupId">讨论课组id</param>
        /// <param name="grade">分数</param>
        public void UpdateGroupByGroupId(long seminarGroupId, int grade)
        {
            //测试成功
            if (seminarGroupId <= 0) throw new ArgumentException(nameof(seminarGroupId));

            SeminarGroup smg = _db.SeminarGroup.Find(seminarGroupId);
            smg.ReportGrade = grade;

            _db.SaveChanges();
        }
    }
}