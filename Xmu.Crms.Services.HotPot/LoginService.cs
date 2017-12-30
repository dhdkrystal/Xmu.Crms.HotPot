using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;
using static Xmu.Crms.Services.HotPot.PasswordUtils;


namespace Xmu.Crms.Services.HotPot
{
    public class LoginService : ILoginService
    {
        private readonly CrmsContext _db;
        private readonly ICourseService _courseService;
        private readonly IClassService _classService;
        private readonly IFixGroupService _fixGroupService;
        private readonly ISeminarGroupService _seminarGroupService;

        //在构造函数里添加依赖的Service
        public LoginService(CrmsContext db, ICourseService courseService, IClassService classService,
            ISeminarGroupService seminarGroupService, IFixGroupService fixGroupService)
        {
            _db = db;
            _courseService = courseService;
            _classService = classService;
            _fixGroupService = fixGroupService;
            _seminarGroupService = seminarGroupService;
        }
        //密码加密
        public string GetMd5(string strPwd)
        {
            using (var md5 = MD5.Create())
            {
                var byteHash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(strPwd));
                var strRes = BitConverter.ToString(byteHash).Replace("-", "");
                strRes = strRes.ToUpper();
                return strRes.Length > 24 ? strRes.Substring(8, 16) : strRes;
            }
        }
        /// <summary>
        /// 用户解绑.学生解绑账号
        /// @author dwy
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>true 解绑成功 false 解绑失败</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IClassService.DeleteCourseSelectionById(System.Int64,System.Int64)"/>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.UserNotFoundException">未找到对应用户</exception>
        public void DeleteStudentAccount(long userId)
        {
            if (userId < 0)
                throw new ArgumentException("userId格式错误");
            var user = _db.UserInfo.Find(userId) ??
               throw new UserNotFoundException();
            IList<ClassInfo> courses = _classService.ListClassByUserId(userId);//根据学生ID获取班级列表
            IList<SeminarGroup> groups = _seminarGroupService.ListSeminarGroupIdByStudentId(userId);//获取学生的所有讨论课小组
            foreach (SeminarGroup s in groups)
            {
                if (userId == _seminarGroupService.GetSeminarGroupLeaderByGroupId(s.Id))//如果是组长
                {
                    _seminarGroupService.ResignLeaderById(s.Id, userId);//组长辞职
                }
                _seminarGroupService.DeleteSeminarGroupMemberById(s.Id, userId);   //在小组中删除该成员
            }
            foreach (ClassInfo c in courses)
            {
                FixGroup fixGroup = _fixGroupService.GetFixedGroupById(userId, c.Id);//找到学生所在的固定小组
                _fixGroupService.DeleteFixGroupUserById(fixGroup.Id, userId);//将学生从固定小组中删去              
                _classService.DeleteCourseSelectionById(userId, c.Id);//学生按班级ID取消选择班级
            }

            _db.RemoveRange(_db.UserInfo.Where(u => u.Id == userId));//删除学生账号
            _db.SaveChanges();
        }

        /// <summary>
        /// 用户解绑.教师解绑账号
        /// @author dwy
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>true 解绑成功 false 解绑失败</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ICourseService.ListCourseByUserId(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ICourseService.DeleteCourseByCourseId(System.Int64)"/>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.UserNotFoundException">未找到对应用户</exception>
        public void DeleteTeacherAccount(long userId)
        {
            if (userId < 0)
                throw new ArgumentException("userId格式错误");
            var user = _db.UserInfo.Find(userId) ??
               throw new UserNotFoundException();  //未找到用户

            IList<Course> courses = _courseService.ListCourseByUserId(userId);//按useId获取与教师相关联的课程列表
            foreach (Course c in courses)
            {
                _courseService.DeleteCourseByCourseId(c.Id);//按courseId删除课程
            }

            _db.RemoveRange(_db.UserInfo.Where(u => u.Id == userId));//删除老师账号
            _db.SaveChanges();

        }


        /// <summary>
        /// 手机号登录.
        /// @author dwy 测试成功
        /// User中只有phone和password，用于判断用户名密码是否正确
        /// </summary>
        /// <param name="user">用户信息(手机号Phone和密码Password)</param>
        /// <returns>UserInfo该用户信息</returns>
        public UserInfo SignInPhone(UserInfo user)
        {
            //找到用户
            var u = _db.UserInfo.SingleOrDefault(c => c.Phone == user.Phone) ??
                 throw new UserNotFoundException();
            //判断密码是否正确
            //if (GetMd5(user.Password) == u.Password)
            if (user.Password == u.Password)
            {
                return u;
            }
            //  if (IsExpectedPassword(user.Password, ReadHashString(u.Password)))
            // {
            //      return u;
            //  }
            else
                throw new PasswordErrorException();
        }

        public UserInfo SignInWeChat(long userId, string code, string state, string successUrl)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 手机号注册 User中只有phone和password，userId是注册后才有并且在数据库自增
        /// @author dwy 测试成功
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>UserInfo该用户信息</returns>
        public UserInfo SignUpPhone(UserInfo user)
        {
            //  user.Password = HashString(user.Password);
            //user.Password = GetMd5(user.Password);  //Md5加密
            //判断手机号是否已经注册
            if (_db.UserInfo.Any(u => u.Phone == user.Phone))
            {
                throw new PhoneAlreadyExistsException();
            }
            user.Type = Shared.Models.Type.Unbinded;
            var entry = _db.UserInfo.Add(user);
            _db.SaveChanges();
            return entry.Entity;
        }
    }
}
