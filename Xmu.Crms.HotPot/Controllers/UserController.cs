using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Xmu.Crms.HotPot.Controllers
{
    [Route("")]
    [Produces("application/json")]
    public class UserController : Controller
    {
        private readonly IUserService _service;
        private readonly JwtHeader _header;

        public UserController(IUserService service, JwtHeader header)
        {
            _service = service;
            _header = header;
        }

        [Authorize]
        [HttpGet("/me")]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var user = _service.GetUserByUserId(User.Id());
                return Json(user, Utils.Ignoring("City", "Province"));
            }
            catch (UserNotFoundException)
            {
                return StatusCode(404, new { msg = "用户不存在" });
            }
        }

        [HttpPut("/me")]
        public IActionResult UpdateCurrentUser([FromBody] UserInfo updated) => NoContent();

        [HttpGet("/signin")]
        public IActionResult SigninWechat([FromQuery] string code, [FromQuery] string state,
            [FromQuery(Name = "success_url")] string successUrl) => Json(new SigninResult());

        [HttpPost("/signin")]
        public IActionResult SigninPassword([FromBody] UsernameAndPassword uap)
        {
            try
            {
                var user = _service.SignUpPhone(new UserInfo {Phone = uap.Phone, Password = uap.Password});
                return Json(new SigninResult
                {
                    Exp = DateTime.UtcNow.AddDays(7)
                              .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks /
                          TimeSpan.TicksPerSecond,
                    Id = user.Id,
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

            public string Name { get; set; }

            public long Exp { get; set; }

            public string Jwt { get; set; }
        }
    }
}