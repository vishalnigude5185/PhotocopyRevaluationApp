namespace PhotocopyRevaluationApp.Autherization {
    using System.Security.Claims;
    using System.Threading.Tasks;
    // File: Authorization/SameUserAuthorization.cs
    using Microsoft.AspNetCore.Authorization;

    public class SameUserRequirement : IAuthorizationRequirement {
    }

    public class SameUserRequirementHandler : AuthorizationHandler<SameUserRequirement, string> {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserRequirement requirement, string resourceOwnerId) {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == resourceOwnerId) {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

}
