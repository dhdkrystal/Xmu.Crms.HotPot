using System;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;
using Type = Xmu.Crms.Shared.Models.Type;

namespace Xmu.Crms.Services.Insomnia
{
    public class Md5LoginService : ILoginService
    {
        private readonly CrmsContext _db;

        public Md5LoginService(CrmsContext db) => _db = db;

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


        public UserInfo SignInWeChat(long userId, string code, string state, string successUrl) =>
            throw new NotImplementedException();

        public UserInfo SignInPhone(UserInfo user)
        {
            var userInfo = _db.UserInfo.Include(u => u.School).SingleOrDefault(u => u.Phone == user.Phone) ??
                           throw new UserNotFoundException();
            if (GetMd5(user.Password) == userInfo.Password)
            {
                return userInfo;
            }

            throw new PasswordErrorException();
        }

        public UserInfo SignUpPhone(UserInfo user)
        {
            user.Password = GetMd5(user.Password);
            if (_db.UserInfo.Any(u => u.Phone == user.Phone))
            {
                throw new PhoneAlreadyExistsException();
            }

            user.Type = Type.Unbinded;

            var entry = _db.UserInfo.Add(user);
            _db.SaveChanges();
            return entry.Entity;
        }

        public void DeleteTeacherAccount(long userId) => throw new NotImplementedException();

        public void DeleteStudentAccount(long userId) => throw new NotImplementedException();
    }
}