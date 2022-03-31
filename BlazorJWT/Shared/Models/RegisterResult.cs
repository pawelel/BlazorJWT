namespace BlazorJWT.Shared.Models
{
	public class RegisterResult
	{
		public bool Successful { get; init; }
		public IEnumerable<string>? Errors { get; init; }
	}
}
