using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OpenIDConnectPlayground;

var builder = WebApplication.CreateBuilder(args);

// Needed for session storage
builder.Services.AddDistributedMemoryCache();

// Our own implementation of the ITicketStore
builder.Services.AddTransient<ITicketStore, TicketStore>();

// As the session store is retrieved through DI, it needs to be configured like this.
builder.Services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
    .Configure<ITicketStore>((options, store) =>
    {
        options.SessionStore = store;
        options.Cookie.Name = "oidctest";
    });

// Add services to the container.
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    // Be sure to include the authentication scheme, otherwise Configure() on the section above will not be called
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(options =>
    {
        // dotnet user-secrets set OIDC:ClientId
        options.ClientId = builder.Configuration["OIDC:ClientId"];
        // dotnet user-secrets set OIDC:ClientSecret
        options.ClientSecret = builder.Configuration["OIDC:ClientSecret"];
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.ResponseMode = OpenIdConnectResponseMode.Query;

        // See appsettings.json
        options.Authority = builder.Configuration["OIDC:Authority"];

        options.GetClaimsFromUserInfoEndpoint = true;
        options.SaveTokens = true;

        options.Scope.Add("openid");
        options.Scope.Add("profile");
    });


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    IdentityModelEventSource.ShowPII = true;
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();
app.MapControllers();

app.Run();