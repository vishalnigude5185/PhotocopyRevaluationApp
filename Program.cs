using PhotocopyRevaluationAppMVC.Hubs;
using PhotocopyRevaluationAppMVC.Configurations;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//=================================== [MY Logic Start] ===========================
// Use extension methods to add services
//================================ [Add Services] ==============================

//================================ [Add Other Services] ==============================
builder.Services.AddOtherServices();

//================================ [Add InMemory Collection Services] ==============================
builder.Services.AddInMemoryCollectionServices();

//================================ [Add Other Configuration Code] ==============================
builder.Services.AddChatBoatConfiguration();

//================================ [Add Authentication Services] ==============================
builder.Services.AddIdentityConfiguration();
builder.Services.AddCustomAuthentication();
builder.Services.AddCustomAuthorizationPolicies();

//======================= [Add configurations Configuration/settings Registeration] ========================
builder.Services.AddDbContextConfiguration(
    builder.Configuration,     // Assuming you're passing the IConfigurationBuilder as well (can be different)
    builder.Configuration,     // Pass configuration
    builder.Environment        // Pass the current environment
);

//================================== [Services Registeration] ================================
builder.Services.RegisterAllServices();

//================================== [Hosted Services Registeration] ================================
builder.Services.AddHostedServices();

//====================================== [MY Logic End] ================================

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

//=====================================================================================================

var app = builder.Build();
app.UseCustomEnvironments();

//=============================== [Custom middelware registeration] ===============================
app.UseCustomMiddlewares();

//============================== [Mapping/Routing/endpoints] ============================
app.UseCustomControllerRoute();

//============================== [Mapping the endpoints for policies] ===========================
app.UseCustomPolicyEndpoints();

//============================== [Hubs Mapping] ============================
app.MapHub<SignOutHub>("/signouthub");  // Map SignalR Hub
app.MapHub<NotificationHub>("/notificationHub");

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.Run();