using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazorStripe.Data;
using RazorStripe.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RazorStripe.Extensions;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Stripe;
using RazorStripe.Data.Models;
using Microsoft.AspNetCore.Identity.UI;

namespace RazorStripe
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                //.AddDefaultUI(UIFramework.Bootstrap4)
                .AddDefaultTokenProviders();

            // Configure Identity
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(24);

                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddRazorPagesOptions(options =>
            {
                //Logged in user pages
                options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                options.Conventions.AuthorizeFolder("/ClientProfiles");
                options.Conventions.AuthorizeFolder("/ClientLocations");
                options.Conventions.AuthorizeFolder("/ClientContacts");
                options.Conventions.AuthorizeFolder("/SalesManager");
                options.Conventions.AuthorizeFolder("/MasterAgentContacts");

                //customer pages

                //admin pages
            });

            services.AddAuthorization(options =>
            {
            });

            //services.AddAuthentication().AddFacebook(facebookOptions =>
            //{
            //    facebookOptions.AppId = "xxx";
            //    facebookOptions.AppSecret = "xxx";
            //});

            //services.AddAuthentication().AddGoogle(googleOptions =>
            //{
            //    googleOptions.ClientId = "xxx";
            //    googleOptions.ClientSecret = "xxx";
            //});

            //Register no-op EmailSender used by account confirmation and password reset during
            // development For more information on how to enable account confirmation and password
            // reset please visit https://go.microsoft.com/fwlink/?LinkID=532713

            services.AddSingleton<IEmailSender, EmailSender>();
            //services.Configure<AuthMessageSenderOptions>(Configuration);

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            StripeConfiguration.SetApiKey(Configuration.GetSection("Stripe")["SecretKey"]);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{acion=Index}/{id?}");
            });

            // Set up custom content types -associating file extension to MIME type
            var provider = new FileExtensionContentTypeProvider();
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });
        }
    }
}