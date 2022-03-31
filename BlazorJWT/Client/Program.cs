using Blazored.LocalStorage;
using BlazorJWT.Client;
using BlazorJWT.Client.Helpers;
using BlazorJWT.Client.Services;
using BlazorJWT.Shared.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore(config =>
{
	config.AddPolicy(Policies.IsAdmin, Policies.IsAdminPolicy());
	config.AddPolicy(Policies.IsUser, Policies.IsUserPolicy());
	config.AddPolicy(Policies.IsManager, Policies.IsManagerPolicy());
});
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();

await builder.Build().RunAsync();

