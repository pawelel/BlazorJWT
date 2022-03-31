using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorJWT.Client.Helpers
{
	public class ApiAuthenticationStateProvider : AuthenticationStateProvider
	{
		private readonly HttpClient _httpClient;
		private readonly ILocalStorageService _localStorage;

		public ApiAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
		{
			_httpClient = httpClient;
			_localStorage = localStorage;
		}
		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var savedToken = await _localStorage.GetItemAsync<string>("authToken");

			if (string.IsNullOrWhiteSpace(savedToken))
			{
				return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
			}

			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);

			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(savedToken), "jwt")));
		}

		public void MarkUserAsAuthenticated(string token)
		{
			var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
			var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
			NotifyAuthenticationStateChanged(authState);
		}

		public void MarkUserAsLoggedOut()
		{
			var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
			var authState = Task.FromResult(new AuthenticationState(anonymousUser));
			NotifyAuthenticationStateChanged(authState);
		}

		private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
		{
			var claims = new List<Claim>();
			var payload = jwt.Split('.')[1];
			var jsonBytes = ParseBase64WithoutPadding(payload);
			var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
			if(keyValuePairs is null) return  claims;
			keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);
			var rolesString = string.IsNullOrWhiteSpace(roles?.ToString())?string.Empty:roles.ToString();
			if(string.IsNullOrWhiteSpace(rolesString)) return claims;
			
				if (rolesString.Trim().StartsWith("["))
				{
					var parsedRoles = JsonSerializer.Deserialize<string[]>(rolesString);

					claims.AddRange(parsedRoles?.Select(parsedRole => new Claim(ClaimTypes.Role, parsedRole))??new List<Claim>());
				}
				else
				{
					claims.Add(new Claim(ClaimTypes.Role, rolesString));
				}

				keyValuePairs.Remove(ClaimTypes.Role);
			

			claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()??string.Empty)));

			return claims;
		}

		private static byte[] ParseBase64WithoutPadding(string base64)
		{
			switch (base64.Length % 4)
			{
				case 2: base64 += "=="; break;
				case 3: base64 += "="; break;
			}
			return Convert.FromBase64String(base64);
		}
	}
}
