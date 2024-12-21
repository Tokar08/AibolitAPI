using AibolitAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AibolitAPI.Attributes;

public class AuthorizeRoleAttribute : TypeFilterAttribute
{
    public AuthorizeRoleAttribute(params string[] requiredRoles)
        : base(typeof(AuthorizeRoleFilter))
    {
        Arguments = new object[] { requiredRoles };
    }

    private class AuthorizeRoleFilter : IAsyncAuthorizationFilter
    {
        private readonly string[] _requiredRoles;
        private readonly IRoleValidator _roleValidator;

        public AuthorizeRoleFilter(string[] requiredRoles, IRoleValidator roleValidator)
        {
            _requiredRoles = requiredRoles ?? throw new ArgumentNullException(nameof(requiredRoles));
            _roleValidator = roleValidator ?? throw new ArgumentNullException(nameof(roleValidator));
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var rolesString = string.Join(",", _requiredRoles);
            Console.WriteLine($"[AuthorizeRole(\"{rolesString}\")] Ожидаемые роли: {rolesString}");

            var validRole = false;
            foreach (var role in _requiredRoles)
            {
                validRole = await _roleValidator.ValidateUserRoleAsync(context.HttpContext, role);
                if (validRole)
                    break;
            }

            if (!validRole)
                context.Result = new ForbidResult();
        }
    }
}