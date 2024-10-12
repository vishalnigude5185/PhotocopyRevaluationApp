using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PhotocopyRevaluationAppMVC.ChatBoat;
using PhotocopyRevaluationAppMVC.Data;
using PhotocopyRevaluationAppMVC.Hubs;
using PhotocopyRevaluationAppMVC.Logging;
using PhotocopyRevaluationAppMVC.ML;
using PhotocopyRevaluationAppMVC.Models;
using PhotocopyRevaluationAppMVC.Services;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;
using PhotocopyRevaluationAppMVC.Autherization;
using PhotocopyRevaluationAppMVC.Global.Exceptions;
using Microsoft.FeatureManagement.FeatureFilters;
using Microsoft.FeatureManagement;
using PhotocopyRevaluationAppMVC.Exceptions;
using PhotocopyRevaluationAppMVC.Services.FeatureManagement;

namespace PhotocopyRevaluationAppMVC.Configurations
{
    public static class ServiceExtensions
    {
        // Combined method to register all services
        public static IServiceCollection RegisterAllServices(this IServiceCollection services)
        {
            services.GetScoped();
            services.GetTransient();
            services.GetSingleton();
            return services;
        }
        public static IServiceCollection AddHostedServices(this IServiceCollection services)
        {
            //================================ [Hosted services Registeration start] =====================
            //// Configure Kestrel with options
            ////builder.WebHost.UseKestrel(options =>
            ////{
            ////    // Example: Configure Kestrel to listen on specific ports
            ////    options.Listen(IPAddress.Any, 5000); // HTTP
            ////    options.Listen(IPAddress.Any, 5001, listenOptions =>
            ////    {
            ////        listenOptions.UseHttps(); // HTTPS
            ////    });
            ////});

            // Register the TimerService as a hosted service
            services.AddHostedService<TimerService>();

            //================================ [Hosted services Registeration end] =====================
            return services;
        }
        public static IServiceCollection AddOtherServices(this IServiceCollection services)
        {
            //// Add feature management services
            //services.AddFeatureManagement()
            //        .AddFeatureFilter<PercentageFilter>(); // Register the custom filter
            //// Add feature management services
            //services.AddFeatureManagement()
            //        .AddFeatureFilter<RoleFilter>(); // Register the custom filter

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("https://yourwebsite.com")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod());
            });

            // Register the IHttpClientFactory service
            services.AddHttpClient();

            //services.AddApplicationInsightsTelemetry(builder.Configuration["ApplicationInsights:InstrumentationKey"]);

            // Configure FluentEmail with SMTP
            services.AddFluentEmail("your-email@example.com", "Your Name")
                //.AddRazorRenderer()  // Optional: To use Razor templating for emails
                .AddSendGridSender("your-sendgrid-api-key")
                .AddSmtpSender(new SmtpClient("smtp.example.com")
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("your-email@example.com", "your-password"),
                    EnableSsl = true,
                    Port = 587 // Use the correct port for your SMTP provider
                });

            // Add ASP.NET Core Identity and link with ApplicationUser
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<PhotocopyRevaluationAppContext>()
                .AddDefaultTokenProviders();

            // Add SignalRz
            services.AddSignalR();

            return services;
        }
        public static IServiceCollection AddInMemoryCollectionServices(this IServiceCollection services)
        {
            // Add MemoryCache service
            services.AddMemoryCache();
            // Add session support
            services.AddDistributedMemoryCache();
            // For in-memory session storage
            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = "localhost"; // Your Redis server configuration
            //    options.InstanceName = "SampleInstance";
            //});
            //services.AddDistributedSqlServerCache(options =>
            //{
            //    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection"); ; // Your SQL Server connection string
            //    options.SchemaName = "dbo";
            //    options.TableName = "SessionData";
            //});
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(4); // Session timeout
                options.Cookie.HttpOnly = true; // Cookie settings for security
                options.Cookie.IsEssential = true; // Required for GDPR compliance
            });

            return services;
        }
        public static void AddChatBoatConfiguration(this IServiceCollection services)
        {
            // Specify the path to your training data
            string dataPath = "path_to_your_data.csv";

            // Initialize the model trainer
            var modelTrainer = new ChatBotModelTrainer(dataPath);

            // Initialize the chat bot service
            var chatBotService = new ChatBotService(modelTrainer);

            Console.WriteLine("Welcome to the ChatBot! Type 'exit' to quit.");

            while (true)
            {
                Console.Write("You: ");
                string userMessage = Console.ReadLine();
                if (userMessage.ToLower() == "exit") break;

                chatBotService.HandleUserMessage(userMessage);
            }
        }
       
        // Extension method to configure Entity Framework
        public static IServiceCollection AddDbContextConfiguration(this IServiceCollection services, IConfiguration configuration, IConfigurationBuilder configBuilder, IHostEnvironment env)
        {
            // Register DbContext
            services.AddDbContext<PhotocopyRevaluationAppContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<LoggingContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection/*LoggingDatabase*/")));

            // Bind MySettings section to the MySettings class
            services.Configure<MySettings>(configuration.GetSection("MySettings"));
            // Load PayPal settings
            services.Configure<PayPalSettings>(configuration.GetSection("PayPal"));


            // Load environment-specific configurations (appsettings.{environment}.json)
            configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            return services;
        }
        // Extension method to configure Identity
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
        {
            //services.AddIdentity<IdentityUser, IdentityRole>()
            //.AddEntityFrameworkStores<PhotocopyRevaluationAppContext>()
            //.AddDefaultTokenProviders();

            // Add ASP.NET Core Identity and link with ApplicationUser
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Configure password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
            })
                .AddEntityFrameworkStores<PhotocopyRevaluationAppContext>()
                .AddDefaultTokenProviders();

            return services;
        }
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services)
        {
            // Add Authentication with Cookie-based scheme AND
            // JWT Configuration    
            services.AddAuthentication(options =>
            {
                //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Set default to cookie
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Set challenge to cookie
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            //.AddGoogle(googleOptions =>
            //{
            //    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
            //    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            //    googleOptions.Events.OnCreatingTicket = ctx =>
            //    {
            //        // Add custom claims here
            //        ctx.Identity.AddClaim(new Claim("custom-claim", "claim-value"));
            //        return Task.CompletedTask;
            //    };
            //})
            //.AddFacebook(facebookOptions =>
            //{
            //    facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
            //    facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
            //})
            .AddCookie(options =>
            {
                options.LoginPath = "/Accounts/Login";  // Redirect to this path if not authenticated
                options.LogoutPath = "/Accounts/Logout";
                options.AccessDeniedPath = "/Accounts/AccessDenied";  // Redirect to this path if access is denied
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);  // Expiry time for the cookie
                options.SlidingExpiration = true;  // Refresh the cookie expiration on each request
                options.Cookie.HttpOnly = true;  // Ensures cookie is not accessible via JavaScript
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // Cookie is only sent over HTTPS
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "https://auth.yourapp.com",
                    ValidAudience = "https://api.yourapp.com",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KeyVaultManager.GetKeyVaultSecretKey("JWTTokenSecretKey").GetAwaiter().GetResult()))
                };
            });

            return services;
        }
        public static void AddCustomAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));  // Example of role-based policy
               
                // Add Authorization
                options.AddPolicy("RequireAuthenticatedUser", policy =>
                    policy.RequireAuthenticatedUser());

                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy("AdminastratorOnly", policy =>
                    policy.RequireRole("Administrator"));

                options.AddPolicy("UserOnly", policy =>
                    policy.RequireRole("User"));

                options.AddPolicy("RequirePermission", policy =>
                    policy.RequireClaim("Permission", "CanEditPosts"));

                options.AddPolicy("RequireEmailConfirmed", policy =>
                    policy.RequireClaim("EmailConfirmed", "True"));

                options.AddPolicy("RequireCustomRole", policy =>
                    policy.RequireRole("CustomRole1", "CustomRole2"));

                options.AddPolicy("CustomPolicy", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(claim => claim.Type == "CustomClaim")));

                options.AddPolicy("RequireSpecificUser", policy =>
                    policy.RequireUserName("AdminUser"));

                options.AddPolicy("RequireSameUser", policy =>
                    policy.RequireAssertion(context =>
                        context.User.Identity != null &&
                        context.User.Identity.Name == context.Resource.ToString()));

                options.AddPolicy("RequireRoleBasedPermission", policy =>
                    policy.RequireRole("Admin").RequireClaim("Permission", "CanDelete"));
                
                //options.AddPolicy("RequireSpecificUser", policy =>
                //    policy.Requirements.Add(new UserNameRequirement("AdminUser")));
                //options.AddPolicy("RequireSameUser", policy =>
                //    policy.Requirements.Add(new SameUserRequirement()));
                //options.AddPolicy("RequireRoleBasedPermission", policy =>
                //    policy.Requirements.Add(new RoleBasedPermissionRequirement("Admin", "CanDelete")));
                //options.AddPolicy("RequireMinimumAge", policy =>
                //   policy.Requirements.Add(new MinimumAgeRequirement(18)));
            });
        }

        // Method to register scoped services
        //============================= Services Registeration Section start =============================
        //==========================Section for Scoped =========================
        public static IServiceCollection GetScoped(this IServiceCollection services)
        {
            services.AddScoped<UserSessionService>();
            services.AddScoped<EmailService>();
            services.AddScoped<SMSService>();
            services.AddScoped<GenerateUidService>();
            services.AddScoped<SignOutHub>();
            services.AddScoped<NotificationHub>();
            // Step 1: Register your custom DatabaseLoggerProvider as a singleton or scoped service
            services.AddScoped<DatabaseLoggerProvider>();
            // Register PayPalService and pass configuration settings (replace with real clientId and clientSecret)
            services.AddScoped<IGlobalExceptionHandler, GlobalExceptionHandler>();
            services.AddScoped<IPayPalService, PayPalService>();
            services.AddScoped<IExportDataService, ExportDataService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITimerService, TimerService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IUserSessionService, UserSessionService>();
            services.AddScoped<IUserConnectionManager, UserConnectionManager>();  // Scoped or Singleton, depending on dependencies
            services.AddScoped<PhotocopyRevaluationAppMVC.Services.INotificationService, NotificationService>();      // Scoped or Singleton, depending on dependencies
                                                                                                                      // Register IEmailSender with your email sender implementation
            services.AddScoped<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, EmailService>();

            return services;
        }

        // Method to register transient services
        //=====================================Section for Transient =============================
        public static IServiceCollection GetTransient(this IServiceCollection services)
        {
            // Hubs are typically registered as transient
            services.AddTransient<SignOutHub>();
            services.AddTransient<NotificationHub>();
            // Add other transient services here
            return services;
        }

        // Method to register singleton services
        //=============================== Section for Singlton =================================
        public static IServiceCollection GetSingleton(this IServiceCollection services)
        {
            // Add other singleton services here
            ////services.AddSingleton<UserService>();
            //To export PDF
            services.AddSingleton(typeof(DinkToPdf.Contracts.IConverter), new SynchronizedConverter(new PdfTools()));
            // Add DinkToPdf as a service
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            // Optionally add it as a singleton
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<PayPalSettings>>().Value);
            // Register MySettings for injection
            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<MySettings>>().Value);
            return services;
        }

    }
}
