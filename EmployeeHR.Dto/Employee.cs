using System;

namespace EmployeeHR.Dto
{
    public class Employee : ICloneable, IEquatable<Employee>
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

        public bool Equals(Employee other)
        {
            bool isEqual = this.Id.Equals(other.Id) &&
                            this.FirstName.Equals(other.FirstName) &&
                            this.LastName.Equals(other.LastName);
            return isEqual;
        }
    }
}