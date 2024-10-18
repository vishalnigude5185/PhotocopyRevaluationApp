using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PhotocopyRevaluationApp.Autherization {
    public class RoleBasedPermissionRequirement : IAuthorizationRequirement {
        public string Role { get; }
        public string Permission { get; }

        public RoleBasedPermissionRequirement(string role, string permission) {
            Role = role;
            Permission = permission;
        }
    }

    public class RoleBasedPermissionHandler : AuthorizationHandler<RoleBasedPermissionRequirement> {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleBasedPermissionRequirement requirement) {
            if (context.User.IsInRole(requirement.Role) && context.User.HasClaim("Permission", requirement.Permission)) {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

}
