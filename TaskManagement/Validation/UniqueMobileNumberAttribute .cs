using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TaskManagement.Data;
using TaskManagement.Models;

namespace TaskManagement.Validation
{
    public class UniqueMobileNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var mobileNumber = value as string;
            if (string.IsNullOrEmpty(mobileNumber))
            {
                return new ValidationResult("Mobile number is required.");
            }
            var dbContext = (TaskManagementDbContext)validationContext.GetService(typeof(TaskManagementDbContext));
            var userExists = dbContext.Users.Any(u => u.PhoneNumber == mobileNumber);

            if (userExists)
            {
                return new ValidationResult("This mobile number is already in use.");
            }

            return ValidationResult.Success;
        }
    }
}
