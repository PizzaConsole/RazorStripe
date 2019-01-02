using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorStripe.Services
{
    public class StripeSettings
    {
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
    }
}