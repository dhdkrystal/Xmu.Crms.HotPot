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
    public class UploadService : IUploadService
    {
        private readonly CrmsContext _db;
        public UploadService(CrmsContext db)
        {
            _db = db;
        }
        ///<summary>
        ///上传用户头像名单
        ///上传用户头像
        ///</summary>
        ///<param name="userId">用户ID</param>
        ///<param name="pathName">文件路径</param>
        public void UploadAvater(long userId, string pathName)
        {
            var user = _db.UserInfo.SingleOrDefault(u => u.Id == userId);
            if(user == null)
            {
                throw new UserNotFoundException();
            }

            user.Avatar = pathName ?? throw new ArgumentException();
            _db.SaveChanges();
            
        }
        ///<summary>上传小组报告
        ///上传讨论课的报告
        ///</summary>
        ///<param name="seminaId">讨论课Id</param>
        ///<param name="pathName">文件路径</param>
        public void UploadReport(long seminaId, string pathName)
        {
            throw new NotImplementedException();
        }
        ///<summary>
        ///上传选课名单
        ///老师上传本班级的学生名单
        ///</summary>
        ///<param name="classId">班级Id</param>
        ///<param name="pathName">文件路径</param>
        public void UploadRoster(long classId, string pathName)
        {
            throw new NotImplementedException();
        }
    }
}
