using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace PhotocopyRevaluationApp.Autherization {
    public class UserNameRequirement : IAuthorizationRequirement {
        public string RequiredUserName { get; }

        public UserNameRequirement(string requiredUserName) {
            RequiredUserName = requiredUserName;
        }
    }

    public class UserNameRequirementHandler : AuthorizationHandler<UserNameRequirement> {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserNameRequirement requirement) {
            if (context.User.HasClaim(c => c.Type == ClaimTypes.Name) &&
                context.User.Identity.Name == requirement.RequiredUserName) {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

}
