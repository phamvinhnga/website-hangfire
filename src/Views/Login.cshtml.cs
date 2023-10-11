using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Polly;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Website.Views
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginInputModel Input { get; set; }
    }

    public class LoginInputModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

}
