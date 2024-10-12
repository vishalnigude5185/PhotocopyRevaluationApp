using Microsoft.AspNetCore.Authorization;
using PhotocopyRevaluationAppMVC.Middlewares;

namespace PhotocopyRevaluationAppMVC.Configurations
{
    public static class AppExtensions   
    {
        public static void UseCustomMiddlewares(this IApplicationBuilder app)
        {
            // Register your custom middleware components here

            // Uncomment the ones you want to use
            //app.UseMiddleware<ExceptionHandlingMiddleware>();
            //app.UseMiddleware<RateLimitingMiddleware>();
            //app.UseMiddleware<SlidingWindowRateLimitingMiddleware>();
            //app.UseMiddleware<TokenBucketRateLimitingMiddleware>();
            //app.UseMiddleware<FixedWindowRateLimitingMiddleware>();

            app.UseMiddleware<LoggingMiddleware>();  // Register LoggingMiddleware
            //app.UseMiddleware<TrackingMiddleware>();  // Optionally register TrackingMiddleware
        }
        // Map endpoints and apply the respective policies
        // Autherize user throw polycies
        // Require authenticated user
        public static void UseCustomPolicyEndpoints(this WebApplication app)
        {
            app.MapGet("/authenticated", [Authorize(Policy = "RequireAuthenticatedUser")] (HttpContext context) =>
            {
                return "Authenticated user accessed!";
            });

            // Admin only
            app.MapGet("/admin", [Authorize(Policy = "AdminOnly")] (HttpContext context) =>
            {
                return "Admin only access!";
            });

            // Administrator only
            app.MapGet("/administrator", [Authorize(Policy = "AdminastratorOnly")] (HttpContext context) =>
            {
                return "Administrator only access!";
            });

            // User only
            app.MapGet("/user", [Authorize(Policy = "UserOnly")] (HttpContext context) =>
            {
                return "User role only access!";
            });

            // Require permission claim
            app.MapGet("/edit-posts", [Authorize(Policy = "RequirePermission")] (HttpContext context) =>
            {
                return "User with 'CanEditPosts' permission accessed!";
            });

            // Require email confirmed
            app.MapGet("/email-confirmed", [Authorize(Policy = "RequireEmailConfirmed")] (HttpContext context) =>
            {
                return "User with email confirmed accessed!";
            });

            // Custom roles
            app.MapGet("/custom-roles", [Authorize(Policy = "RequireCustomRole")] (HttpContext context) =>
            {
                return "User with CustomRole1 or CustomRole2 accessed!";
            });
            // Custom assertion policy
            app.MapGet("/custom-assertion", [Authorize(Policy = "CustomPolicy")] (HttpContext context) =>
            {
                return "Custom policy access based on assertion!";
            });
            // Require minimum age
            app.MapGet("/minimum-age", [Authorize(Policy = "RequireMinimumAge")] (HttpContext context) =>
            {
                return "User meets minimum age requirement!";
            });
            // Require specific user
            app.MapGet("/specific-user", [Authorize(Policy = "RequireSpecificUser")] (HttpContext context) =>
            {
                return "Specific user 'AdminUser' accessed!";
            });
            // Require same user
            app.MapGet("/same-user/{userId}", [Authorize(Policy = "RequireSameUser")] (HttpContext context, string userId) =>
            {
                return $"Same user with ID {userId} accessed!";
            });
            // Require role-based permission
            app.MapGet("/role-permission", [Authorize(Policy = "RequireRoleBasedPermission")] (HttpContext context) =>
            {
                return "User with Admin role and CanDelete permission accessed!";
            });
        }
        public static void UseCustomControllerRoute(this WebApplication app)
        {
            //================================== [default code] =================================
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Accounts}/{action=Login}/{id?}");
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Accounts}/{action=Login}/{id?}");
            //    endpoints.MapHub<SignOutHub>("/signOutHub");  // Map the SignOutHub
            //    endpoints.MapHub<NotificationHub>("/notificationHub");
            //});
        }
        public static void UseCustomEnvironments(this WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // Show detailed error information to the developer
                app.UseDeveloperExceptionPage();
                app.UseCors(builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
                app.Use(async (context, next) =>
                {
                    await next();
                    context.Response.Headers.Add("X-Developer", "Development Mode");  // Custom response header
                });
                // No bundling/minification in development
                app.UseStaticFiles();  // Serve static files as-is
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            else if (app.Environment.IsEnvironment("Testing"))
            {
                // Use a simple exception handler that shows detailed error messages for debugging
                app.UseExceptionHandler("/Error/Testing"); // Testing error page with more details

                // Optionally, enforce HTTPS even in testing (but it's often not enforced in tests)
                app.UseHttpsRedirection();

                // Enable custom security headers
                // Add relaxed security headers or disable certain headers for testing
                app.Use(async (context, next) =>
                {
                    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                    context.Response.Headers.Add("X-Frame-Options", "DENY");
                    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                    // Example: Content-Security-Policy (CSP) - You may need to customize this based on your app
                    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; img-src 'self' data:; style-src 'self' 'unsafe-inline'; script-src 'self' 'unsafe-inline'");

                    await next();
                });

                // Enable logging of detailed request and response information for testing
                app.Use(async (context, next) =>
                {
                    // Log detailed information for debugging purposes
                    // Include things like headers, body, query strings, etc.
                    Console.WriteLine($"Request URL: {context.Request.Path}");
                    Console.WriteLine($"Query: {context.Request.QueryString}");
                    await next();
                });

                // Skip static file caching to allow changes to be reflected immediately in tests
                app.UseStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = ctx =>
                    {
                        // Cache static files for 30 days
                        ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=2592000");
                    }
                });

                // Enable response compression to simulate production performance
                app.UseResponseCompression();
            }
            else
            {   //
                //$env:ASPNETCORE_ENVIRONMENT="Production"

                // Use exception handler middleware in production to handle errors gracefully
                app.UseExceptionHandler("/Home/Error"); // Redirect to a user-friendly error page

                // Enforce HTTPS for all requests
                app.UseHttpsRedirection();

                // HSTS: Enforce HTTPS with a long max age (1 year) and apply to subdomains
                app.UseHsts(
                //options =>
                //{
                //    options.MaxAge(TimeSpan.FromDays(365)) // Set the max age for HSTS
                //           .IncludeSubdomains() // Apply HSTS to all subdomains
                //           .Preload(); // Optional: Add preload directive
                //}
                );

                // Add advanced security headers
                app.Use(async (context, next) =>
                {
                    context.Response.Headers.Add("X-Content-Type-Options", "nosniff"); // Prevent MIME type sniffing
                    context.Response.Headers.Add("X-Frame-Options", "DENY"); // Prevent clickjacking
                    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block"); // XSS protection
                    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains"); // Enforce HTTPS
                    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; img-src 'self' data:; style-src 'self' 'unsafe-inline'; script-src 'self' 'unsafe-inline';");

                    await next();
                });

                // Enable response caching for static files (e.g., images, CSS, JS) for better performance
                app.UseStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = ctx =>
                    {
                        // Cache static files for 30 days (in seconds: 2592000)
                        ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=2592000");
                    }
                });

                // Enable logging and monitoring
                app.Use(async (context, next) =>
                {
                    // Log request details for production diagnostics (use structured logging)
                    // Add Application Insights or other logging middleware if needed
                    await next();
                });

                // Other production optimizations like response compression, minification, etc.
                app.UseResponseCompression(); // Enable response compression for better performance
            }
        }
    }
}
