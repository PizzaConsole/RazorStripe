using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorStripe.Extensions
{
    public static class PageConventionCollectionExtensions
    {
        public static PageConventionCollection AuthorizeFolder(this PageConventionCollection conventions, string folderPath, string[] roles)
        {
            if (conventions == null)
            {
                throw new ArgumentNullException(nameof(conventions));
            }

            if (string.IsNullOrEmpty(folderPath))
            {
                throw new ArgumentException("Argument cannot be null or empty.", nameof(folderPath));
            }

            var policy = new AuthorizationPolicyBuilder().
                RequireRole(roles).Build();
            var authorizeFilter = new AuthorizeFilter(policy);
            conventions.AddFolderApplicationModelConvention(folderPath, model => model.Filters.Add(authorizeFilter));
            return conventions;
        }

        public static PageConventionCollection AuthorizePage(this PageConventionCollection conventions, string pageName, string[] roles)
        {
            if (conventions == null)
            {
                throw new ArgumentNullException(nameof(conventions));
            }

            if (string.IsNullOrEmpty(pageName))
            {
                throw new ArgumentException("Argument cannot be null or empty.", nameof(pageName));
            }

            var policy = new AuthorizationPolicyBuilder().
                RequireRole(roles).Build();
            var authorizeFilter = new AuthorizeFilter(policy);
            conventions.AddPageApplicationModelConvention(pageName, model => model.Filters.Add(authorizeFilter));
            return conventions;
        }
    }
}