using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;
using Xmu.Crms.Shared.Exceptions;

namespace Xmu.Crms.HotPot.Controllers
{
    [Route("")]
    [Produces("application/json")]
    public class UserController : Controller
    {
        private readonly IUserService _service;
        private readonly ILoginService _loginService;
        private readonly JwtHeader _header;

        public UserController(IUserService service, JwtHeader header)
        {
            _service = service;
            _header = header;
        }

        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("/me")]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var user = _service.GetUserByUserId(Shared.Models.User.Id);
                return Json(user, Utils.Ignoring("City", "Province"));
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
        public IActionResult UpdateCurrentUser([FromBody] User updated) => NoContent();

        
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
                var user = _loginService.SignUpPhone(new User {Phone = uap.Phone, Password = uap.Password});
                return Json(new SigninResult
                {
                    Exp = DateTime.UtcNow.AddDays(7)
                              .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks /
                          TimeSpan.TicksPerSecond,
                    Id = user.Id,
                    //Type = user.Type;
                    Name = user.Name,
                    Jwt = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(_header,
                        new JwtPayload(
                            null,
                            null,
                            new[]
                            {
                                new Claim("id", user.Id.ToString()),
                                new Claim("type", ""),
                            },
                            null,
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
        public IActionResult RegisterPassword([FromBody] UsernameAndPassword uap) => Json(new SigninResult());

        [HttpPost("/upload/avatar")]
        public IActionResult UploadAvatar(IFormFile file) =>
            Created("/upload/avatar.png", new {url = "/upload/avatar.png"});

        public class UsernameAndPassword
        {
            public string Phone { get; set; }
            public string Password { get; set; }
        }

        public class SigninResult
        {
            public long Id { get; set; }

            //public Xmu.Crms.Shared.Models.Type Type { get; set; }

            public string Name { get; set; }

            public long Exp { get; set; }

            public string Jwt { get; set; }
        }
    }
}