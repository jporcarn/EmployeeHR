using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EmployeeHR.Dto
{
    public class Employee : ICloneable
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SocialSecurityNumber { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime RowVersion { get; set; }

        public object Clone()
        {
            Employee other = (Employee)this.MemberwiseClone();
            return other;
        }
    }
}