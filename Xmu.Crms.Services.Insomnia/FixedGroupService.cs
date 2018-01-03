using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Services.Insomnia
{
    public class FixedGroupService : IFixGroupService
    {
        private readonly CrmsContext _db;

        public FixedGroupService(CrmsContext db) => _db = db;

        public long InsertFixGroupByClassId(long classId, long userId)
        {
            if (classId <= 0)
            {
                throw new ArgumentException(nameof(classId));
            }

            if (userId <= 0)
            {
                throw new ArgumentException(nameof(userId));
            }

            var cls = _db.ClassInfo.Find(classId) ?? throw new ClassNotFoundException();
            var usr = _db.UserInfo.Find(userId) ?? throw new UserNotFoundException();
            var fg = _db.FixGroup.Add(new FixGroup {ClassInfo = cls, Leader = usr});
            _db.SaveChanges();
            return fg.Entity.Id;
        }

        public void DeleteFixGroupMemberByFixGroupId(long fixGroupId)
        {
            if (fixGroupId <= 0)
            {
                throw new ArgumentException(nameof(fixGroupId));
            }

            _db.FixGroupMember.RemoveRange(_db.FixGroupMember.Include(m => m.FixGroup)
                .Where(m => m.FixGroup.Id == fixGroupId));
            _db.SaveChanges();
        }

        public long InsertFixGroupMemberById(long userId, long groupId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException(nameof(userId));
            }

            if (groupId <= 0)
            {
                throw new ArgumentException(nameof(groupId));
            }

            var grp = _db.FixGroup.Find(groupId) ?? throw new FixGroupNotFoundException();
            var usr = _db.UserInfo.Find(userId) ?? throw new UserNotFoundException();
            var fgm = _db.FixGroupMember.Add(new FixGroupMember {FixGroup = grp, Student = usr});
            _db.SaveChanges();
            return fgm.Entity.Id;
        }

        public IList<UserInfo> ListFixGroupMemberByGroupId(long groupId)
        {
            if (groupId <= 0)
            {
                throw new ArgumentException(nameof(groupId));
            }

            var fixGroup = _db.FixGroup.Find(groupId) ?? throw new FixGroupNotFoundException();
            return _db.FixGroupMember.Include(f => f.FixGroup).Include(f => f.Student)
                .Where(f => f.FixGroup == fixGroup).Select(f => f.Student).ToList();
        }

        public IList<FixGroup> ListFixGroupByClassId(long classId)
        {
            if (classId <= 0)
            {
                throw new ArgumentException(nameof(classId));
            }

            var cls = _db.ClassInfo.Find(classId) ?? throw new ClassNotFoundException();
            return _db.FixGroup.Include(f => f.ClassInfo).Where(f => f.ClassInfo == cls).ToList();
        }

        public void DeleteFixGroupByClassId(long classId)
        {
            if (classId <= 0)
            {
                throw new ArgumentException(nameof(classId));
            }

            var cls = _db.ClassInfo.Find(classId) ?? throw new ClassNotFoundException();
            var members = _db.FixGroupMember.Include(f => f.FixGroup).ThenInclude(f => f.ClassInfo)
                .Where(f => f.FixGroup.ClassInfo == cls);
            var fixGroups = members.Select(m => m.FixGroup).Distinct();
            _db.FixGroupMember.RemoveRange(members);
            _db.FixGroup.RemoveRange(fixGroups);
            _db.SaveChanges();
        }

        public void DeleteFixGroupByGroupId(long groupId)
        {
            if (groupId <= 0)
            {
                throw new ArgumentException(nameof(groupId));
            }

            DeleteFixGroupMemberByFixGroupId(groupId);
            _db.Remove(_db.FixGroup.Find(groupId) ?? throw new FixGroupNotFoundException());
        }

        public void UpdateFixGroupByGroupId(long groupId, FixGroup fixGroupBo)
        {
            if (groupId <= 0)
            {
                throw new ArgumentException(nameof(groupId));
            }

            var fixGroup = _db.FixGroup.Find(groupId) ?? throw new FixGroupNotFoundException();
            fixGroup.ClassInfo = fixGroupBo.ClassInfo;
            fixGroup.Leader = fixGroupBo.Leader;
            _db.SaveChanges();
        }

        public long InsertStudentIntoGroup(long userId, long groupId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException(nameof(userId));
            }

            if (groupId <= 0)
            {
                throw new ArgumentException(nameof(groupId));
            }

            var fixGroup = _db.FixGroup.Find(groupId) ?? throw new FixGroupNotFoundException();
            var entry = _db.FixGroupMember.Add(new FixGroupMember
            {
                FixGroup = fixGroup,
                Student = _db.UserInfo.Find(userId) ?? throw new UserNotFoundException()
            });
            _db.SaveChanges();
            return entry.Entity.Id;
        }

        public FixGroup GetFixedGroupById(long userId, long classId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException(nameof(userId));
            }

            if (classId <= 0)
            {
                throw new ArgumentException(nameof(classId));
            }

            var usr = _db.UserInfo.Find(userId) ?? throw new UserNotFoundException();
            var cls = _db.ClassInfo.Find(classId) ?? throw new ClassNotFoundException();
            var fixGroup = _db.FixGroupMember.Include(m => m.FixGroup)
                .Where(m => m.StudentId == usr.Id && m.FixGroup.ClassId == cls.Id).Select(m => m.FixGroup).SingleOrDefault();
            if (fixGroup != null)
            {
                return fixGroup;
            }
            fixGroup = _db.FixGroup.Find(InsertFixGroupByClassId(classId, userId));
            InsertFixGroupMemberById(userId, fixGroup.Id);

            return fixGroup;
        }

        public void FixedGroupToSeminarGroup(long semianrId, long fixedGroupId)
        {
            throw new NotImplementedException();
        }

        public void DeleteFixGroupUserById(long fixGroupId, long userId)
        {
            var grp = _db.FixGroup.Find(fixGroupId) ?? throw new GroupNotFoundException();
            var usr = _db.UserInfo.Find(userId) ?? throw new UserNotFoundException();
            _db.FixGroupMember.RemoveRange(_db.FixGroupMember.Include(f => f.FixGroup).Where(f => f.FixGroup == grp && f.Student == usr));
            _db.SaveChanges();
        }

        public IList<FixGroupMember> ListFixGroupByGroupId(long groupId)
        {
            var grp = _db.FixGroup.Find(groupId) ?? throw new GroupNotFoundException();
            return _db.FixGroupMember.Include(f => f.FixGroup).Where(f => f.FixGroup == grp).ToList();
        }
    }
}