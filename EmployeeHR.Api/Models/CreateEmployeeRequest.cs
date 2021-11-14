using System.ComponentModel.DataAnnotations;

namespace EmployeeHR.Api.Models
{
    public class CreateEmployeeRequest
    {

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string SocialSecurityNumber { get; set; }

        public string PhoneNumber { get; set; }

    }
}
