using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RazorStripe.Data;
using RazorStripe.Models;
using RazorStripe.Extensions;
using RazorStripe.Services;
using Stripe;
using Stripe.Infrastructure;

namespace RazorStripe.Areas.Identity.Pages.Account.Manage
{
    public class SubscriptionsModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly StripeSettings _stripeSettings;

        public SubscriptionsModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db,
            IOptions<StripeSettings> stripeSettings)
        {
            _roleManager = roleManager;
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _stripeSettings = stripeSettings.Value;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public SelectList SubscriptionProducts { get; set; }
        public SelectList SubscriptionPlans { get; set; }

        public List<Subscription> Subscriptions { get; set; }

        public string StripeKey { get; set; }
        public string UserEmail { get; set; }
        public string StripeCustomerId { get; set; }

        public long? PlanPrice { get; set; }

        public class InputModel
        {
            public string ProductId { get; set; }

            public string PlanId { get; set; }
        }

        public JsonResult OnGetProducts(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return new JsonResult(null);
            }
            StripeList<Plan> plans = GetProductPlans(productId);
            SubscriptionPlans = new SelectList(plans, "Id", "Nickname");
            return new JsonResult(SubscriptionPlans);
        }

        public JsonResult OnGetPlanPrice(string planId)
        {
            if (string.IsNullOrEmpty(planId))
            {
                return new JsonResult(null);
            }
            PlanPrice = GetPlanPrice(planId);

            return new JsonResult(PlanPrice);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            StripeCustomerId = user.CustomerIdentifier;
            var nullCustomerId = string.IsNullOrEmpty(StripeCustomerId);
            UserEmail = user.Email;

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var product = new ProductService();
            var productsOptions = new ProductListOptions
            {
            };
            StripeList<Product> products = product.List(productsOptions);

            SubscriptionProducts = new SelectList(products, "Id", "Name");

            if (!nullCustomerId)
            {
                var subcriptionService = new SubscriptionService();
                IEnumerable<Subscription> response = subcriptionService.List(new SubscriptionListOptions
                {
                    CustomerId = StripeCustomerId
                });
                Subscriptions = response.ToList();
            }

            StripeKey = _stripeSettings.PublishableKey;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromForm]string stripeToken)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            UserEmail = user.Email;
            StripeCustomerId = user.CustomerIdentifier;
            var nullCustomerId = string.IsNullOrEmpty(StripeCustomerId);
            var planId = Input.PlanId;
            var planAmount = GetPlanPrice(planId);

            var customerService = new CustomerService();
            Customer customerLookup = new Customer();

            if (!nullCustomerId)
            {
                customerLookup = customerService.Get(StripeCustomerId);
            }

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            //Create new customer if doesnt exist
            if (nullCustomerId || customerLookup.Deleted == true)
            {
                var customers = new CustomerService();

                var customer = customers.Create(new CustomerCreateOptions
                {
                    Email = UserEmail,
                    SourceToken = stripeToken,
                    PlanId = planId,
                    Description = UserEmail + " " + "[" + user.Id + "]"
                });

                user.CustomerIdentifier = customer.Id;
                StripeCustomerId = user.CustomerIdentifier;
            }
            else
            {
                var subcriptionService = new SubscriptionService();
                var subscriptionItems = new List<SubscriptionItemOption>
                {
                    new SubscriptionItemOption
                    {
                        PlanId = planId
                    }
                };
                var stripeSubscription = subcriptionService.Create(new SubscriptionCreateOptions
                {
                    CustomerId = StripeCustomerId,
                    Items = subscriptionItems
                });
            }

            Charge charge = new Charge();
            if (planAmount > 0)
            {
                var chargeOptions = new ChargeCreateOptions
                {
                    Amount = planAmount,
                    Currency = "usd",
                    Description = "RazorStripe for" + " " + UserEmail,
                    CustomerId = StripeCustomerId,
                };
                var chargeService = new ChargeService();
                charge = chargeService.Create(chargeOptions);
            }
            await _db.SaveChangesAsync();

            await _signInManager.RefreshSignInAsync(user);

            StatusMessage = "Your payment: " + charge.Status;

            return RedirectToPage();
        }

        private StripeList<Plan> GetProductPlans(string productId)
        {
            var service = new PlanService();
            var serviceOptions = new PlanListOptions
            {
                ProductId = productId
            };
            StripeList<Plan> plans = service.List(serviceOptions);

            return plans;
        }

        private long? GetPlanPrice(string planId)
        {
            var service = new PlanService();
            Plan plan = service.Get(planId);
            long? planPrice = plan.Amount;

            return planPrice;
        }
    }
}