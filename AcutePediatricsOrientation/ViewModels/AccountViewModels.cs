using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AcutePediatricsOrientation.ViewModels
{
    public class StaffListViewModel
    {
        public List<StaffViewModel> Users { get; set; }
    }

    public class StaffViewModel
    {
        public string Name { get; set; }
        public int UserId { get; set; }
        public double Progress { get; set; }
    }

    public class RegisterViewModel
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Confirm Password")]
        [Required]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Role")]
        [Required]
        public int RoleId { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}
