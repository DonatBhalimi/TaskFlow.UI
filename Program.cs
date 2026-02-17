using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TaskFlow.UI.Authorization;
using TaskFlow.UI.Components;
using TaskFlow.UI.Data;
using TaskFlow.UI.Models;
using TaskFlow.UI.Models.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/auth-debug";
    options.AccessDeniedPath = "/auth-debug";
    options.Cookie.Name = "taskflow.auth";
});

builder.Services.AddScoped(sp =>
{
    var nav = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(nav.BaseUri) };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        Permissions.ManageUsers,
        policy => policy.Requirements.Add(
            new PermissionRequirement(Permissions.ManageUsers)));

    options.AddPolicy(
        Permissions.ManageSystem,
        policy => policy.Requirements.Add(
            new PermissionRequirement(Permissions.ManageSystem)));

    options.AddPolicy(
        Permissions.ManageOrganization,
        policy => policy.Requirements.Add(
            new PermissionRequirement(Permissions.ManageOrganization)));
    
    options.AddPolicy(
        Permissions.ManageWorkspaces,
        policy => policy.Requirements.Add(
            new PermissionRequirement(Permissions.ManageWorkspaces)));

    options.AddPolicy(
        Permissions.ManageTemplates,
        policy => policy.Requirements.Add(
            new PermissionRequirement(Permissions.ManageTemplates)));

    options.AddPolicy(
        Permissions.ManageSettings,
        policy => policy.Requirements.Add(
            new PermissionRequirement(Permissions.ManageSettings)));

    options.AddPolicy(
        Permissions.CreateTasks,
        policy => policy.Requirements.Add(
            new PermissionRequirement(Permissions.CreateTasks)));

    options.AddPolicy(
        Permissions.AssignTasks,
        policy => policy.Requirements.Add(
            new PermissionRequirement(Permissions.AssignTasks)));

    options.AddPolicy(
        Permissions.EditTasks,
        policy => policy.Requirements.Add(
            new PermissionRequirement(Permissions.EditTasks)));

    options.AddPolicy(
        Permissions.ViewTasks,
        policy => policy.Requirements.Add(
            new PermissionRequirement(Permissions.ViewTasks)));
});

builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration["MongoSettings:ConnectionString"]));

builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var db = client.GetDatabase(builder.Configuration["MongoSettings:Database"]);
    return db.GetCollection<User>("users");
});

builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var db = client.GetDatabase(builder.Configuration["MongoSettings:Database"]);
    return db.GetCollection<TaskItem>("tasks");
});

builder.Services.AddSingleton<PasswordHasher<User>>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddScoped<UserRoleService>();
builder.Services.AddScoped<TaskRepository>();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();  
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/login", async (
    HttpContext context,
    AuthService auth,
    [FromForm] LogInRequestDTO request) =>
{
    var principal = await auth.LoginAsync(request.Email, request.Password);

    await context.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal);

    return Results.Redirect("/manageusers");
}).DisableAntiforgery();

app.MapPost("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    return Results.Ok();
}).DisableAntiforgery();

app.Run();
