namespace BlazorJWT.Shared.Models
{
	public class UserModel
	{
		public string Email { get; set; } = string.Empty;
		public bool IsAuthenticated { get; set; }
	}
}
