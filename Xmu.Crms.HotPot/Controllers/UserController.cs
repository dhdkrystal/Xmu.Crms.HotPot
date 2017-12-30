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
        private readonly JwtHeader _header;

        public UserController(IUserService userService,ILoginService loginService,ISchoolService schoolSevice,JwtHeader header)
        {
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
                    School = user.School               
                }
                    );
            }
            catch (UserNotFoundException)
            {
                return StatusCode(404, new { msg = "用户不存在" });
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
            catch (PasswordErrorException)
            {
                return StatusCode(401, new { msg = "用户名或密码错误" });
            }
            catch (UserNotFoundException)
            {
                return StatusCode(404, new { msg = "用户不存在" });
            }
        }

        [HttpPost("/upload/avatar")]
        public IActionResult UploadAvatar(IFormFile file) =>
            Created("/upload/avatar.png", new {url = "/upload/avatar.png"});
          
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