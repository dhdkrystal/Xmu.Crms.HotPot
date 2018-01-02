using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Mobile;
using Xmu.Crms.Mobile.HotPot.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
using System.IO;

namespace Xmu.Crms.HotPot.Controllers
{
    /// <summary>
    /// @author dhd
    /// </summary>
    [Route("")]
    [Produces("application/json")]
    public class UserController : Controller
    {
        
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;
        private readonly ISchoolService _schoolService;
        private readonly IUploadService _uploadService;
        private readonly JwtHeader _header;

        public UserController(IUploadService uploadService,IUserService userService,ILoginService loginService,ISchoolService schoolSevice,JwtHeader header)
        {
            _uploadService = uploadService;
            _userService = userService;
            _loginService = loginService;
            _schoolService = schoolSevice;
            _header = header;
        }

        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns></returns>
        
        [HttpGet("/me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var user = _userService.GetUserByUserId(long.Parse(User.Claims.Single(c => c.Type == "id").Value));
                user.School = _schoolService.GetSchoolBySchoolId(user.SchoolId ?? -0);
                return Json(new UserInfo
                {
                    Name = user.Name,
                    Number = user.Number,
                    Phone = user.Phone,
                    School = user.School,
                    Avatar=user.Avatar
                }
                   );
            }
            catch (UserNotFoundException)
            {
                return StatusCode(404, new { msg = "用户不存在" });
            }
            catch (InvalidOperationException)
            {
                return StatusCode(404, new { msg = "不合法操作" });
            }
        }

        /// <summary>
        /// 修改当前用户
        /// </summary>
        /// <param name="updated"></param>
        /// <returns></returns>
        [HttpPut("/me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult UpdateCurrentUser([FromBody] UserInfo updated)
        {
            try
            {
                
                _userService.UpdateUserByUserId(long.Parse(User.Claims.Single(c => c.Type == "id").Value), updated);
                return NoContent();
            }
            catch (UserNotFoundException)
            {
                return StatusCode(404, new { msg = "用户不存在" });
            }
        }

        
        /// <summary>
        /// 手机号密码登录
        /// </summary>
        /// <param name="uap"></param>
        /// <returns></returns>
        [HttpPost("/signin")]
        public IActionResult SigninPassword([FromBody] UsernameAndPassword uap)
        {
            try
            {
                var user = _loginService.SignInPhone(new UserInfo {Phone = uap.Phone, Password = uap.Password});
                HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme,new ClaimsPrincipal());
                return Json(new SigninResult
                {
                    Id = user.Id,
                    Type = user.Type.ToString(),
                    Name = user.Name,
                    Jwt = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(_header,
                        new JwtPayload(
                            null,
                            null,
                            new[]
                            {
                                new Claim("id", user.Id.ToString()),
                                new Claim("type", user.Type.ToString()),
                            },
                            null,
                            //token过期时间
                            DateTime.Now.AddDays(7)
                        )))
                });
            }
            
            catch (PasswordErrorException)
            {
                return StatusCode(401, new { msg = "用户名或密码错误" });
            }
            catch (UserNotFoundException)
            {
                return StatusCode(404, new { msg = "用户不存在" });
            }
        }

        /// <summary>
        /// 手机号密码注册
        /// </summary>
        /// <param name="uap"></param>
        /// <returns></returns>
        [HttpPost("/register")]
        public IActionResult RegisterPassword([FromBody] UsernameAndPassword uap)
        {
            try
            {
                var user = _loginService.SignUpPhone(new UserInfo { Phone = uap.Phone, Password = uap.Password });
                HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme, new ClaimsPrincipal());
                return Json(new SigninResult
                {
                    Id = user.Id,
                    Type = user.Type.ToString(),
                    Name = user.Name,
                    Jwt = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(_header,
                        new JwtPayload(
                            null,
                            null,
                            new[]
                            {
                                new Claim("id", user.Id.ToString()),
                                new Claim("type", user.Type.ToString()),
                            },
                            null,
                            //token过期时间
                            DateTime.Now.AddDays(7)
                        )))
                });
            }
            catch (PhoneAlreadyExistsException)
            {
                return StatusCode(401, new { msg = "该手机号已被注册" });
            }
            catch (PasswordErrorException)
            {
                return StatusCode(401, new { msg = "用户名或密码错误" });
            }
            catch (UserNotFoundException)
            {
                return StatusCode(404, new { msg = "用户不存在" });
            }
        }

        [HttpGet("/upload/avatar")]
        public IActionResult UploadAvatar2(IFormFile file) =>
            Created("/upload/avatar.png", new {url = "/upload/avatar.png"});
        
        [HttpPost("/upload/avatar")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult UploadAvatar()
        {
            try
            {
                var file = Request.Form.Files[0];
                string path = "../Xmu.Crms.Mobile.HotPot/wwwroot/upload/" + User.Claims.Single(c => c.Type == "id").Value.ToString()+file.Name;
                string path2 = "/upload/" + User.Claims.Single(c => c.Type == "id").Value.ToString() + file.Name;
                using (FileStream fs = System.IO.File.Create(path))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                _uploadService.UploadAvater(long.Parse(User.Claims.Single(c => c.Type == "id").Value), path2);
                return Created(path, new { url = path });
            }
            catch(UserNotFoundException)
            {
                return StatusCode(404, new { msg = "用户不存在" });
            }
            catch (ArgumentException)
            {
                return StatusCode(404, new { msg = "路径为空" });
            }
        }
        

    }
    public class UsernameAndPassword
    {
        public string Phone { get; set; }
        public string Password { get; set; }
    }

    public class SigninResult
    {
        public long Id { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string Jwt { get; set; }
    }
}