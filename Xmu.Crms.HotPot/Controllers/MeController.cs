using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace CourseManagementSystem.Controllers
{
    public class MeController : ApiController
    {
        private CourseManagementSystemContext db = new CourseManagementSystemContext();
        /*     
        public IEnumerable<User> GetAllUsers() 
        { 
     
        }
        */
        //GET api/me
        [Route("me")]
        [HttpGet]
        public User GetInfo()
        {
            return new User { Id = 1, Email = "1@1.com", Name = "1", Number = "1", Phone = "1", Sex = "male", Type = "student" };
        }

        [Route("me")]
        [HttpPut]
        public IHttpActionResult ChooseCharacter()
        {
            return StatusCode(HttpStatusCode.NoContent);

        }

        
        [Route("signin")]
        [HttpPost]
        public SignAndRegisterViewModel Signin(LoginData data)
        {
            if(String.IsNullOrEmpty(data.phone) || String.IsNullOrEmpty(data.password))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "密码错误" });
            }
            User user = db.Users.Where(b => b.Phone == data.phone).FirstOrDefault();
           
            if(user == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "无此用户" });
            }
            if(user.Password != data.password)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "用户密码错误" });
            }
            return new SignAndRegisterViewModel { Id = user.Id, Type = user.Type, Name = user.Name };
        }

        [Route("register")]
        [HttpPost]
        public SignAndRegisterViewModel Register(LoginData data)
        {
            //User user = db.Users.Where(b => b.Phone == data.phone).FirstOrDefault();
            //if (user != null)
            //{
                //throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "已存在此账户" });
            //}
            var newUser = new Models.User { Phone = data.phone, Password = data.password, Type = "unbinded" };
            db.Users.Add(newUser);
            db.SaveChanges();
            return new SignAndRegisterViewModel { Id = newUser.Id, Type = newUser.Type, Name = newUser.Name };
        }

        //[Route("Course")]
        //[HttpGet]
        //public SignAndRegisterViewModel GetCourseInfo()
        //{
        //    User user = db.Users.Where(b => b.Phone == data.phone).FirstOrDefault();
        //    if (user != null)
        //    {
        //        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "已存在此账户" });
        //    }
        //    var newUser = new Models.User { Phone = data.phone, Password = data.password, Type = "unbinded" };
        //    db.Users.Add(newUser);
        //    db.SaveChanges();
        //    return new SignAndRegisterViewModel { Id = newUser.Id, Type = newUser.Type, Name = newUser.Name };
        //}

    }
}
