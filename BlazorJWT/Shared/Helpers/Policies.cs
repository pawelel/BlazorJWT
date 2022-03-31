using Microsoft.AspNetCore.Authorization;

namespace BlazorJWT.Shared.Helpers
{
	public static class Policies
	{
		public const string IsAdmin = "IsAdmin";
		public const string IsUser = "IsUser";
		public const string IsManager = "IsManager";

		public static AuthorizationPolicy IsAdminPolicy()
		{
			return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
												   .RequireRole("Admin")
												   .Build();
		}

		public static AuthorizationPolicy IsUserPolicy()
		{
			return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
												   .RequireRole("User")
												   .Build();
		}
		public static AuthorizationPolicy IsManagerPolicy()
		{
			return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
												   .RequireRole("Manager")
												   .Build();
		}
	}
}
