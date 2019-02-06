using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RazorStripe.Models
{
    public class UserSubscription
    {
        [Key]
        public Guid UserSubscriptionId { get; set; }

        public DateTime PurchaseDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        public int Amount { get; set; }

        public string UserId { get; set; }

        public string PaymentSubscriptionId { get; set; }

        public Guid ApiKey { get; set; }
    }
}