using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Stripe;

namespace RazorStripe.Data.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Subscriptions = new List<UserSubscription>();
        }

        public virtual ICollection<UserSubscription> Subscriptions { get; set; }

        public string CustomerIdentifier { get; set; }

        [Display(Name = "Full Name")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

    }
}