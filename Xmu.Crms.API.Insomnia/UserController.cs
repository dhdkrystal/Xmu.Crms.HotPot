using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;
using static Xmu.Crms.Insomnia.Utils;
using System.Text.RegularExpressions;
//using Xmu.Crms.Mobile.HighGrade;


namespace Xmu.Crms.Insomnia
{
    [Route("")]
    [Produces("application/json")]
    public class UserController : Controller
    {
        private readonly JwtHeader _header;
        private readonly ILoginService _loginService;
        private readonly IUserService _userService;
        private readonly ISchoolService _schoolService;

        public UserController(JwtHeader header, ILoginService loginService, IUserService userService, ISchoolService schoolService)
        {
            _header = header;
            _loginService = loginService;
            _userService = userService;
            _schoolService = schoolService;
        }

        [HttpGet(API.Insomnia.Constant.PREFIX + "/me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetCurrentUser()
        {
            
            try
            {
                
                var user = _userService.GetUserByUserId(User.Id());
                user.School = _schoolService.GetSchoolBySchoolId(user.SchoolId ?? -1);
                return Json(user, Ignoring("City", "Province", "Password"));
            }
            catch (UserNotFoundException)
            {
                return StatusCode(404, new { msg = "用户不存在" });
            }
        }

        [HttpPut(API.Insomnia.Constant.PREFIX + "/me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult UpdateCurrentUser([FromBody] UserInfo updated)
        {
            try
            {
                _userService.UpdateUserByUserId(User.Id(), updated);
                return NoContent();
            }
            catch (UserNotFoundException)
            {
                return StatusCode(404, new { msg = "用户不存在" });
            }
        }

        [HttpGet(API.Insomnia.Constant.PREFIX + "/signin")]
        public IActionResult SigninWechat([FromQuery] string code, [FromQuery] string state,
            [FromQuery(Name = "success_url")] string successUrl) => throw new NotSupportedException();

        [HttpPost(API.Insomnia.Constant.PREFIX + "/signin")]
        public IActionResult SigninPassword([FromBody] UsernameAndPassword uap)
        {
            try
            {
                var user = _loginService.SignInPhone(new UserInfo {Phone = uap.Phone, Password = uap.Password});
                HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme, new ClaimsPrincipal());
                //Console.WriteLine("\n\n\n\n\n\n" + User.Id() + "\n\n\n\n\n\n");
                return Json(CreateSigninResult(user));
            }
            catch (PasswordErrorException)
            {
                return StatusCode(401, new {msg = "用户名或密码错误"});
            }
            catch (UserNotFoundException)
            {
                return StatusCode(404, new {msg = "用户不存在"});
            }
        }

        [HttpPost(API.Insomnia.Constant.PREFIX + "/register")]
        public IActionResult RegisterPassword([FromBody] UsernameAndPassword uap)
        {
            try
            {
                var user = _loginService.SignUpPhone(new UserInfo {Phone = uap.Phone, Password = uap.Password});
                return Json(CreateSigninResult(user));
            }
            catch (PhoneAlreadyExistsException)
            {
                return StatusCode(409, new {msg = "手机已注册"});
            }
        }

        private SigninResult CreateSigninResult(UserInfo user) => new SigninResult
        {
            Id = user.Id,
            Name = user.Name,
            Type = user.Type.ToString().ToLower(),
            Jwt = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(_header,
                new JwtPayload(
                    null,
                    null,
                    new[]
                    {
                        new Claim("id", user.Id.ToString()),
                        new Claim("type", user.Type.ToString().ToLower())
                    },
                    null,
                    DateTime.Now.AddDays(7)
                )))
        };

        [HttpPost(API.Insomnia.Constant.PREFIX + "/upload/avatar")]
        public IActionResult UploadAvatar(IFormFile file) =>
            Created("/upload/avatar.png", new {url = "/upload/avatar.png"});

        [Route(API.Insomnia.Constant.PREFIX + "/")]
        public IActionResult HomePage()
        {
            //IsMobileBrowser(HttpContext.Request);
            string userAgent = HttpContext.Request.Headers["User-Agent"];
            Console.WriteLine("\n\n\n\n\n"+userAgent +"\n\n\n\n\n");
            if ((b.IsMatch(userAgent) || v.IsMatch(userAgent.Substring(0, 4))))
            {
                Console.WriteLine("1\n");
                //return true;
                
                return Redirect("/m/LoginPage");
            }
            else
            {
                Console.WriteLine("2\n");
                return Redirect("/Login");
                
            }
            //return false;
            //return Redirect("/Login");
        }
        private static readonly Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static readonly Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //public static bool IsMobileBrowser(this HttpRequest request)
        //{
        //    var userAgent = UserAgent(request);
        //    if ((b.IsMatch(userAgent) || v.IsMatch(userAgent.Substring(0, 4))))
        //    {
        //        Console.WriteLine("1\n");
        //        return true;
        //    }
        //    Console.WriteLine("2\n");
        //    return false;
        //}

        //public static string UserAgent(this HttpRequest request)
        //{
        //    return request.Headers["User-Agent"];
        //}
        public class UsernameAndPassword
        {
            public string Phone { get; set; }
            public string Password { get; set; }
        }

        public class SigninResult
        {
            public long Id { get; set; }

            public string Name { get; set; }

            public string Type { get; set; }

            public string Jwt { get; set; }
        }
    }
}