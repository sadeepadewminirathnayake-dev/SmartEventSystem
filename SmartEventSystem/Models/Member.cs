using System;
using System.ComponentModel.DataAnnotations;

namespace SmartEventSystem.Models
{
    public class Member
    {
        public int MemberID { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Required]
        public int ContactNB { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string Preferences { get; set; }
    }


}
