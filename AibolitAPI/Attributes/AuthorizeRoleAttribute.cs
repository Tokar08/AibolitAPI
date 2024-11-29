using AibolitAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AibolitAPI.Attributes;

public class AuthorizeRoleAttribute : TypeFilterAttribute
{
    public AuthorizeRoleAttribute(string requiredRole)
        : base(typeof(AuthorizeRoleFilter))
    {
        Arguments = new object[] { requiredRole };
    }

    private class AuthorizeRoleFilter : IAsyncAuthorizationFilter
    {
        private readonly string _requiredRole;
        private readonly IRoleValidator _roleValidator;

        public AuthorizeRoleFilter(string requiredRole, IRoleValidator roleValidator)
        {
            _requiredRole = requiredRole ?? throw new ArgumentNullException(nameof(requiredRole));
            _roleValidator = roleValidator ?? throw new ArgumentNullException(nameof(roleValidator));
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!await _roleValidator.ValidateUserRoleAsync(context.HttpContext, _requiredRole))
                context.Result = new ForbidResult();
        }
    }
}