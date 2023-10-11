using System.ComponentModel.DataAnnotations;

namespace Website.Models
{
    public class TaskSettingOption
    {
        public static string Position => "ConfigTask";
        public string TaskName { get; set; }
        public string Url { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
    }

    public class SystemUserOption
    {
        public static string Position => "SystemUserJob";
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Url { get; set; }
    }
}
