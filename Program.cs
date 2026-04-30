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
    options.AddPolicy("action:app.users.manage",
        p => p.Requirements.Add(new ActionRequirement("app.users.manage")));
    options.AddPolicy("action:workspaces.create",
        p => p.Requirements.Add(new ActionRequirement("workspaces.create")));
    options.AddPolicy("action:app.users.create",
        p => p.Requirements.Add(new ActionRequirement("app.users.create")));
});

builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration["MongoSettings:ConnectionString"]));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(builder.Configuration["MongoSettings:Database"]);
});

builder.Services.AddScoped<IMongoCollection<User>>(sp =>
{
    var db = sp.GetRequiredService<IMongoDatabase>();
    return db.GetCollection<User>("users");
});

builder.Services.AddScoped<IMongoCollection<TaskItem>>(sp =>
{
    var db = sp.GetRequiredService<IMongoDatabase>();
    return db.GetCollection<TaskItem>("tasks");
});

builder.Services.AddScoped<IMongoCollection<Workspace>>(sp =>
{
    var db = sp.GetRequiredService<IMongoDatabase>();
    return db.GetCollection<Workspace>("workspaces");
});

builder.Services.AddScoped<IMongoCollection<AppAction>>(sp =>
{
    var db = sp.GetRequiredService<IMongoDatabase>();
    return db.GetCollection<AppAction>("appActions");
});

builder.Services.AddScoped<IMongoCollection<ActionBundle>>(sp =>
{
    var db = sp.GetRequiredService<IMongoDatabase>();
    return db.GetCollection<ActionBundle>("actionBundles");
});

builder.Services.AddScoped<IMongoCollection<AppRole>>(sp =>
{
    var db = sp.GetRequiredService<IMongoDatabase>();
    return db.GetCollection<AppRole>("appRoles");
});

builder.Services.AddScoped<IMongoCollection<WorkspaceAction>>(sp =>
{
    var db = sp.GetRequiredService<IMongoDatabase>();
    return db.GetCollection<WorkspaceAction>("workspaceActions");
});

builder.Services.AddScoped<IMongoCollection<WorkspaceBundle>>(sp =>
{
    var db = sp.GetRequiredService<IMongoDatabase>();
    return db.GetCollection<WorkspaceBundle>("workspaceBundles");
});

builder.Services.AddScoped<IMongoCollection<WorkspaceRole>>(sp =>
{
    var db = sp.GetRequiredService<IMongoDatabase>();
    return db.GetCollection<WorkspaceRole>("workspaceRoles");
});

builder.Services.AddScoped<IMongoCollection<Notification>>(sp =>
{
    var db = sp.GetRequiredService<IMongoDatabase>();
    return db.GetCollection<Notification>("notifications");
});

builder.Services.AddScoped<AppActionRepository>();
builder.Services.AddScoped<ActionBundleRepository>();
builder.Services.AddScoped<AppRoleRepository>();
builder.Services.AddScoped<WorkspaceActionRepository>();
builder.Services.AddScoped<WorkspaceBundleRepository>();
builder.Services.AddScoped<WorkspaceRoleRepository>();
builder.Services.AddHostedService<ReminderWorker>();

builder.Services.AddSingleton<PasswordHasher<User>>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserRoleService>();
builder.Services.AddScoped<TaskRepository>();
builder.Services.AddScoped<NotificationRepository>();
builder.Services.AddScoped<WorkspaceRepository>();
builder.Services.AddScoped<WorkspaceAuthorizationService>();
builder.Services.AddScoped<WorkspaceAuthSeeder>();
builder.Services.AddScoped<ActionCatalogSeeder>();
builder.Services.AddScoped<TaskReminderService>();

builder.Services.AddSingleton<IAuthorizationHandler, ActionHandler>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<WorkspaceAuthSeeder>();
    await seeder.SeedAsync();
}
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
    [FromForm] LogInRequestDTO request,
    string? returnUrl) =>
{
    var principal = await auth.LoginAsync(request.Email, request.Password);

    await context.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal);

    return Results.Redirect(returnUrl ?? "/workspaces");
}).DisableAntiforgery();

app.MapPost("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Ok();
}).DisableAntiforgery();

app.Run();