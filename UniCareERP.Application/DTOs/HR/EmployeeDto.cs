using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniCareERP.Application.DTOs.HR
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? JobTitle { get; set; }
        public string? Department { get; set; }
        public string? Email { get; set; } // System Email from ApplicationUser
        public string? PhoneNumber { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateEmployeeDto
    {
        // ApplicationUser fields
        [Required]
        [EmailAddress]
        [Display(Name = "Login Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Initial Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Display(Name = "User Roles")]
        public List<string> Roles { get; set; } = new List<string>();

        // Employee fields
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(20)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; } = DateTime.Today;

        [MaxLength(100)]
        public string? JobTitle { get; set; }

        [MaxLength(100)]
        public string? Department { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        // This is the employee record status, ApplicationUser status will mirror this.
        public bool IsActive { get; set; } = true;
    }

    public class UpdateEmployeeDto
    {
        [Required]
        public Guid Id { get; set; }

        // Employee fields to update
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(20)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        [MaxLength(100)]
        public string? JobTitle { get; set; }

        [MaxLength(100)]
        public string? Department { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
