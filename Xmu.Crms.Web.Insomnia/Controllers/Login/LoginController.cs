using Microsoft.AspNetCore.Mvc;

namespace Xmu.Crms.Web.Insomnia.Controllers.Login
{
    
    public class LoginController : Controller
    {
        private const string PREFIX = "/desktop";
        [Route(PREFIX + "/Login")]
        public IActionResult AccountLoginPage()
        {
            return View();
        }

        [Route(PREFIX + "/RegisterPage")]
        public IActionResult RegisterPage()
        {
            return View();
        }
    }
}