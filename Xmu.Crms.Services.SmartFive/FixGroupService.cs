using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Services.SmartFive
{
    public class FixGroupService : IFixGroupService
    {
        private readonly CrmsContext _db;

        // 在构造函数里添加依赖的Service（参考模块标准组的类图）
        public FixGroupService(CrmsContext db)
        {
            _db = db;
        }

        public void DeleteFixGroupByClassId(long classId)  //测试成功 zhh
        {   //1.
            //if (classId <= 0)
            //{
            //    throw new ArgumentException();
            //}
            //var class1 = (from class2 in _db.ClassInfo  //判断是否有这个班级
            //              where class2.Id == classId
            //              select class2).SingleOrDefault();
            //if (class1 == null)
            //    throw new ClassNotFoundException();

            //var fgm = (from fgmm in _db.FixGroupMember  //根据班级删除班级里所有fixgroup的成员
            //           from fg in _db.FixGroup
            //           where fgmm.FixGroup.Id == fg.Id && fg.ClassInfo.Id == classId
            //           select fgmm).ToList();
            //foreach (var i in fgm)
            //{
            //    //Console.WriteLine("\n1\n");
            //    _db.FixGroupMember.Remove(i);
            //}

            //var u = (from group1 in _db.FixGroup    //删除班级里所有fixgroup
            //         where group1.ClassInfo.Id == classId
            //         select group1).ToList();
            //foreach (var i in u)
            //{
            //    //Console.WriteLine("\n1\n");
            //    _db.FixGroup.Remove(i);
            //}
            //_db.SaveChanges();

            //2.
            IList<FixGroup> fg = ListFixGroupByClassId(classId);
            Console.WriteLine("fgleagth:"+fg.Count());
            foreach(var i in fg)
            {
                DeleteFixGroupMemberByFixGroupId(i.Id);
                //Console.WriteLine("1");
            }
            var u = (from group1 in _db.FixGroup    //删除班级里所有fixgroup
                     where group1.ClassInfo.Id == classId
                     select group1).ToList();
            foreach (var i in u)
            {
                //Console.WriteLine("\n1\n");
                _db.FixGroup.Remove(i);
            }
            _db.SaveChanges();

        }

        public void DeleteFixGroupByGroupId(long groupId)//测试成功
        {
            if (groupId <= 0)
            {
                throw new ArgumentException();
            }
            var group1 = (from group2 in _db.FixGroup
                          where group2.Id == groupId
                          select group2).SingleOrDefault();
            if (group1 == null)
                throw new FixGroupNotFoundException();

            _db.FixGroup.Remove(group1);
            _db.SaveChanges();
        }

        public void DeleteFixGroupMemberByFixGroupId(long fixGroupId)//测试成功
        {
            //Console.WriteLine(fixGroupId);
            if (fixGroupId <= 0)
            {
                throw new ArgumentException();
            }
            var group1 = (from group2 in _db.FixGroup
                          where group2.Id == fixGroupId
                          select group2).SingleOrDefault();
            if (group1 == null)
                throw new FixGroupNotFoundException();
            var user = (from user1 in _db.FixGroupMember
                        where user1.FixGroup.Id == group1.Id
                        select user1).ToList();
            foreach (var i in user)
            {
                _db.FixGroupMember.Remove(i);
            }
            _db.SaveChanges();
            //DeleteFixGroupUserById(1, 2);
        }

        public void DeleteFixGroupUserById(long fixGroupId, long userId)
        {   /*zhh
             * 在fixgroupmem表删除一个fixgroup里的一个指定学生，没有考虑删除小组最后一个学生后是否需要删除此小组
             * */
            if (fixGroupId <= 0||userId<=0)
            {
                throw new ArgumentException();
            }
            var group1 = (from group2 in _db.FixGroup
                          where group2.Id == fixGroupId
                          select group2).SingleOrDefault();
            if (group1 == null)
                throw new FixGroupNotFoundException();
            var user = (from user1 in _db.FixGroupMember
                        where user1.FixGroup.Id == group1.Id&&user1.Student.Id==userId
                        select user1).SingleOrDefault();
            if (user == null)
                throw new UserNotFoundException();
            _db.FixGroupMember.Remove(user);
            _db.SaveChanges();
        }

        public void DeleteTopicByGroupId(long groupId)
        {
            throw new NotImplementedException();
        }

        //public void DeleteTopicByGroupId(long groupId)//测试成功  已删除
        //{
        //    if (groupId <= 0)
        //    {
        //        throw new ArgumentException();
        //    }
        //    var group1 = (from group2 in _db.FixGroup
        //                  where group2.Id == groupId
        //                  select group2).SingleOrDefault();
        //    if (group1 == null)
        //        throw new FixGroupNotFoundException();
        //    var group3 = (from group4 in _db.SeminarGroupTopic
        //                  where group4.SeminarGroup.Id == groupId
        //                  select group4).ToList();
        //    foreach (var i in group3)
        //    {
        //        _db.SeminarGroupTopic.Remove(i);
        //    }
        //    _db.SaveChanges();
        //}

        public void FixedGroupToSeminarGroup(long seminarId, long fixedGroupId) //2 1
        {   //zhh 从fixgroup的三张表中拿到数据导入三个seminargoup，不修改student_score_group表，此函数重复执行会多次插入重复数据
            if(seminarId<=0||fixedGroupId<=0)
            {
                throw new ArgumentException();
            }
            var fg = (from groupp in _db.FixGroup
                          where groupp.Id == fixedGroupId
                          select new FixGroup
                          {
                              Id=groupp.Id,
                              ClassInfo=new ClassInfo { Id=groupp.ClassInfo.Id},
                              Leader=new UserInfo { Id=groupp.Leader.Id}
                          }  ).SingleOrDefault();
            if (fg == null)
                throw new FixGroupNotFoundException();
            var sm = (from smm in _db.Seminar
                      where smm.Id == seminarId
                      select smm).SingleOrDefault();
            if (sm == null)
                throw new SeminarNotFoundException();

            SeminarGroup sg = new SeminarGroup        //插入sminargroup表
            {
                Seminar = sm,
                ClassInfo = (from c in _db.ClassInfo
                            where c.Id == fg.ClassInfo.Id
                            select c).SingleOrDefault(),
                Leader = (from u in _db.UserInfo
                         where u.Id == fg.Leader.Id
                         select u).SingleOrDefault()
            };
            
            var xx=_db.SeminarGroup.Add(sg);
            _db.SaveChanges();  //Console.WriteLine(xx.Entity.Id);

            var fgm = (from fgmm in _db.FixGroupMember
                       where fgmm.FixGroup.Id == fixedGroupId
                       select new FixGroupMember
                       {
                           Id=fgmm.Id,
                           Student=new UserInfo {Id=fgmm.Student.Id }
                       }).ToList();
            foreach(var tmp in fgm)
            {
                SeminarGroupMember sgm = new SeminarGroupMember  //插入讨论课队伍成员表
                {
                    SeminarGroup = xx.Entity,
                    Student = (from u in _db.UserInfo
                               where u.Id == tmp.Student.Id
                               select u).SingleOrDefault()
                };
                _db.SeminarGroupMember.Add(sgm);
            }
            _db.SaveChanges();

            var fgt = (from fgtt in _db.FixGroupTopic
                       where fgtt.FixGroup.Id == fixedGroupId
                       select new FixGroupTopic
                       {
                           Topic = new Topic { Id = fgtt.Topic.Id }
                       }).ToList();
            foreach (var tmp in fgt)
            {
                SeminarGroupTopic sgt = new SeminarGroupTopic  //插入讨论课队伍选题表
                {
                    SeminarGroup = xx.Entity,
                    Topic  = (from t in _db.Topic
                               where t.Id == tmp.Topic.Id
                               select t).SingleOrDefault()
                };
                _db.SeminarGroupTopic.Add(sgt);
            }
            _db.SaveChanges();
            //long smgid=(from x in _db.SeminarGroup
            //            where x.)
        }

        public FixGroup GetFixedGroupById(long userId, long classId)//测试成功
        {
            Console.WriteLine("\n\n\nGetFixedGroupByIddasdasdasdasdsssa\n\n\n");
            if (userId <= 0 || classId <= 0)
            {
                throw new ArgumentException();
            }

            var c = (from inclass in _db.ClassInfo
                     where inclass.Id == classId
                     select inclass).SingleOrDefault();
            if (c == null)
            {
                throw new ClassNotFoundException();
            }

            //1.不用IUserService.GetUserByUserId(System.Int64)的代码
            //var s = (from student in _db.UserInfo
            //        where student.Id == userId
            //        select student).SingleOrDefault();
            //if(s==null)
            //{
            //    throw new UserNotFoundException();
            //}

            //2.用IUserService.GetUserByUserId(System.Int64)

            UserService x = new UserService(_db);
            x.GetUserByUserId(userId);


            var singlefg = (from fgm in _db.FixGroupMember
                            from fg in _db.FixGroup
                            where fgm.Student.Id == userId && fg.ClassInfo.Id == classId && fgm.FixGroup.Id == fg.Id
                            select new FixGroup
                            {
                                Id = fg.Id,
                                ClassInfo = new ClassInfo { Id = fg.ClassInfo.Id },
                                Leader = new UserInfo { Id = fg.Leader.Id }
                            }).SingleOrDefault();
            return singlefg;
        }

        public IList<FixGroupMember> GetFixGroupByGroupId(long groupId)
        {
            throw new NotImplementedException();
        }

        //public IList<FixGroupMember> GetFixGroupByGroupId(long groupId)//方法已删除
        //{

        //    throw new NotImplementedException();
        //}

        public long InsertFixGroupByClassId(long classId, long userId)//测试成功
        {
            if (classId <= 0 || userId <= 0)
            {
                throw new ArgumentException();
            }

            var class1 = (from class2 in _db.ClassInfo
                          where class2.Id == classId
                          select class2).SingleOrDefault();
            if (class1 == null)
                throw new ClassNotFoundException();

            FixGroup a = new FixGroup();
            a.ClassInfo = (from u in _db.ClassInfo
                           where u.Id == classId
                           select u).SingleOrDefault();
            a.Leader = (from v in _db.UserInfo
                        where v.Id == userId
                        select v).SingleOrDefault();
            //a.ClassInfo.Id = classId;
            //a.Leader = new UserInfo();
            //a.Leader.Id = userId;
            _db.FixGroup.Add(a);
            _db.SaveChanges();
            var b = (from user in _db.FixGroup
                     where user.ClassInfo.Id == classId && user.Leader.Id == userId
                     select user).SingleOrDefault();
            return b.Id;
        }

        public long InsertFixGroupMemberById(long userId, long groupId)
        {
            throw new NotImplementedException();
        }

        //public long InsertFixGroupMemberById(long userId, long groupId)//测试成功  已删除
        //{
        //    if (userId <= 0 || groupId <= 0)
        //    {
        //        throw new ArgumentException();
        //    }
        //    var group1 = (from group2 in _db.FixGroup
        //                  where group2.Id == groupId
        //                  select group2).SingleOrDefault();
        //    if (group1 == null)
        //        throw new FixGroupNotFoundException();
        //    var u = (from user in _db.UserInfo
        //             where user.Id == userId
        //             select user).SingleOrDefault();
        //    if (u == null)
        //    {
        //        throw new UserNotFoundException();
        //    }
        //    var v = (from v1 in _db.SeminarGroupMember
        //             where v1.SeminarGroup.Id == groupId && v1.Student.Id == userId
        //             select v1).SingleOrDefault();
        //    if (v != null)
        //        throw new InvalidOperationException();
        //    FixGroupMember a = new FixGroupMember();
        //    a.FixGroup = (from a1 in _db.FixGroup
        //                  where a1.Id == groupId
        //                  select a1).SingleOrDefault();
        //    a.Student = (from a2 in _db.UserInfo
        //                 where a2.Id == userId
        //                 select a2).SingleOrDefault();
        //    _db.FixGroupMember.Add(a);
        //    _db.SaveChanges();
        //    var b = (from user in _db.FixGroupMember
        //             where user.FixGroup.Id == groupId && user.Student.Id == userId
        //             select user).SingleOrDefault();
        //    return b.Id;
        //}

        public long InsertStudentIntoGroup(long userId, long groupId) //同InsertFixGroupMemberById？？
        {
            if (userId <= 0 || groupId <= 0)
            {
                throw new ArgumentException();
            }
            var group1 = (from group2 in _db.FixGroup
                          where group2.Id == groupId
                          select group2).SingleOrDefault();
            if (group1 == null)
                throw new FixGroupNotFoundException();
            var u = (from user in _db.UserInfo
                     where user.Id == userId
                     select user).SingleOrDefault();
            if (u == null)
            {
                throw new UserNotFoundException();
            }
            var v = (from v1 in _db.SeminarGroupMember
                     where v1.SeminarGroup.Id == groupId && v1.Student.Id == userId
                     select v1).SingleOrDefault();
            if (v != null)
                throw new InvalidOperationException();
            FixGroupMember a = new FixGroupMember();
            a.FixGroup = (from a1 in _db.FixGroup
                          where a1.Id == groupId
                          select a1).SingleOrDefault();
            a.Student = (from a2 in _db.UserInfo
                         where a2.Id == userId
                         select a2).SingleOrDefault();
            _db.FixGroupMember.Add(a);
            _db.SaveChanges();
            var b = (from user in _db.FixGroupMember
                     where user.FixGroup.Id == groupId && user.Student.Id == userId
                     select user).SingleOrDefault();
            return b.Id;
        }

        public IList<FixGroup> ListFixGroupByClassId(long classId)//测试成功辉煌
        {
            if(classId<=0)
                throw new ArgumentException();
            var g = (from groupp in _db.FixGroup
                     where groupp.ClassInfo.Id == classId
                     select new FixGroup
                     {
                         Id = groupp.Id,
                         ClassInfo = new ClassInfo { Id = groupp.ClassInfo.Id },
                         Leader = new UserInfo { Id = groupp.Leader.Id }
                     }).ToList();
            return g;
        }

        public IList<FixGroupMember> ListFixGroupByGroupId(long groupId)
        {   //zhh 返回的list里的fixgroupmem只有id 所在组id和学号 不知道标准组给出需要调用下面的ListFixGroupMemberByGroupId有什么用
            if (groupId <= 0)
                throw new ArgumentException();
            var fgm = (from fgmm in _db.FixGroupMember
                       where fgmm.FixGroup.Id == groupId
                       select new FixGroupMember
                       {
                           Id=fgmm.Id,
                           FixGroup=new FixGroup { Id=fgmm.FixGroup.Id},
                           Student=new UserInfo { Id=fgmm.Student.Id}
                       }).ToList();
            //ListFixGroupMemberByGroupId(groupId);  
            return fgm;
        }

        public IList<UserInfo> ListFixGroupMemberByGroupId(long groupId)
        {//测试成功zhh 
            if(groupId<=0)
                throw new ArgumentException();
            //Console.WriteLine("\n\ninto ListFixGroupMemberByGroupId\n\n");
            var g = (from groupp in _db.FixGroup
                     where groupp.Id == groupId
                     select groupp).SingleOrDefault();
            if (g == null)
            {
                throw new FixGroupNotFoundException();
            }
            //Console.WriteLine("\n\nhas this group\n\n");
            var mem = (from fgm in _db.FixGroupMember
                       from user in _db.UserInfo
                       where fgm.FixGroup.Id == groupId && fgm.Student.Id == user.Id
                       select user/*fgm.Student*/).ToList();
            return mem;
        }

        public void UpdateFixGroupByGroupId(long groupId, FixGroup fixGroupBo)//测试成功辉煌
        {
            if (groupId <= 0)
            {
                throw new ArgumentException();
            }
            var g = (from groupp in _db.FixGroup.Include(a => a.ClassInfo).Include(b => b.Leader)

                     where groupp.Id == groupId
                     select groupp).SingleOrDefault();
            if (g == null)
                throw new FixGroupNotFoundException();
            //Console.WriteLine(g.Id + "  " + g.ClassInfo.Id + "  " + g.Leader.Id);
            g.Id = fixGroupBo.Id;
            g.ClassInfo = (from c in _db.ClassInfo
                           where c.Id == fixGroupBo.ClassInfo.Id
                           select c).SingleOrDefault();
            g.Leader = (from u in _db.UserInfo
                        where u.Id == fixGroupBo.Leader.Id
                        select u).SingleOrDefault();
            _db.SaveChanges();
        }

        public void UpdateSeminarGroupById(long groupId, SeminarGroup group)
        {
            throw new NotImplementedException();
        }

        //public void UpdateSeminarGroupById(long groupId, SeminarGroup group)//此函数已被删除
        //{
        //    if (groupId <= 0)
        //    {
        //        throw new ArgumentException();
        //    }

        //    throw new NotImplementedException();
        //}

    }
}
