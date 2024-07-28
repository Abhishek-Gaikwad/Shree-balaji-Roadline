using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trans9.Models
{
    public class LoginModel
    {
        public string? userName { get; set; }

        [Required]
        [DisplayName("Username / Email Id")]
        public string email { get; set; }

        [Required]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required]
        [DisplayName("Remember Me..")]
        public bool isRemember { get; set; }
    }
    public class ForgotModel
    {
        [Required]
        [DisplayName("Email Id")]
        public string email { get; set; }
    }
    public class ResetModel
    {
        [Required]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string confirmPassword { get; set; }
    }
    public class AppUser
    {
        [Key]
        public Int64 id { get; set; }

        [Display(Name = "Full Name"), StringLength(30)]
        public string displayName { get; set; }

        [Display(Name = "User Name"), StringLength(20)]
        public string userName { get; set; }

        [Display(Name = "Email Id"), StringLength(60)]
        public string email { get; set; }

        [Display(Name = "Password"), StringLength(20)]
        public string password { get; set; }

        [Display(Name = "Role"), StringLength(20)]
        public string roleName { get; set; }

        [NotMapped]
        public List<Pages> pages = new List<Pages>();

        [NotMapped]
        public List<DropDown> roles = new List<DropDown>();
        public string status { get; set; }
    }
    public class Pages
    {
        [Key]
        public int id { get; set; }
        public string pageName { get; set; }
        public string urlName { get; set; }
        public string icon { get; set; }
        public int priority { get; set; }
        public string pgGroup { get; set; }

        [NotMapped]
        public string? active { get; set; }
    }
    public class UserRole
    {

        [Key]
        public int id { get; set; }

        [Display(Name = "Role"), StringLength(20)]
        public string roleName { get; set; }

        [Display(Name = "Pages")]
        public string pageIds { get; set; }
    }
    public class RoleModal
    {
        public int id { get; set; }
        [Display(Name = "Role"), StringLength(20)]
        public string roleName { get; set; }
        [Display(Name = "Pages")]
        public List<int> pages { get; set; }
    }
}
