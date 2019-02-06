using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RazorStripe.Models
{
    public enum Status
    {
        Active, Inactive
    }

    public class Contact
    {
        public int ContactId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        [Required]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        public string Title { get; set; }

        [Phone]
        public string Phone { get; set; }

        [EmailAddress]
        public string EmailAddress { get; set; }

        public string Details { get; set; }

        [Required]
        public Status Status { get; set; }
    }
}