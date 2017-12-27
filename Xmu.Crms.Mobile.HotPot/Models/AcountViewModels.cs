using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Xmu.Crms.Mobile.HotPot.Models
{

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "手机号")]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

    }

    public class RegisterViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "手机号")]
        public string Phone { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
    } 

    public class CourseInfoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CourseListInfoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumClass { get; set; }
    }

    public class AddClassResultModel
    {
        public int Id { get; set; }
    }
}