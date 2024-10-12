using FluentEmail.Core;
using FluentEmail.Smtp;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Context;
using Serilog.Sinks.MSSqlServer;
using System.Data;
using System.Net;
using System.Net.Mail;

namespace PhotocopyRevaluationAppMVC.Logging
{
    public class SerilogLoggingConfiguration
    {
        private readonly ILogger<SerilogLoggingConfiguration> _logger;

        public SerilogLoggingConfiguration(ILogger<SerilogLoggingConfiguration> logger)
        {
            _logger = logger;
        }

        [Obsolete]
        public static void ConfigureLogging(IServiceCollection services, IConfiguration configuration)
        {
            //var emailInfo = new Serilog.Sinks.Email.EmailConnectionInfo
            //{
            //    FromEmail = "vishalnigude5185@gmail.com",
            //    ToEmail = "vishalnigude5185@gmail.com",
            //    MailServer = "smtp.gmail.com",
            //    NetworkCredentials = new NetworkCredential("vishalnigude5185@gmail.com", "yvsl klxp ezyd cirn"),
            //    EnableSsl = true,
            //    Port = 587,
            //    EmailSubject = "Log Error"
            //};
            // Configure Serilog
            var logger = Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)  // Load static properties from appsettings.json
                .Enrich.FromLogContext()                // Add dynamic properties with LogContext / Enables logging of properties pushed to LogContext
                .Enrich.WithProperty("Application", "PhotocopyRevaluationApp")
                //.Enrich.WithProperty("Context", "Authentication") // Add global context property
                //.Enrich.WithProperty("UserId", 12345) // Global UserId property
                //.Enrich.WithProperty("IpAddress", "192.168.1.1") // Global IP Address
                //.Enrich.WithProperty("CorrelationId", "abc-123-def") // Global Correlation ID
                //.Enrich.WithProperty("Location", "HomePage") // Location
                //.Enrich.WithProperty("CustomData", )
                //.Enrich.WithProperty("Action", )
                //.Enrich.WithProperty("CreatedAt", )
                //.Enrich.WithProperty("DurationMs", )
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}Context: {Context} | UserId: {UserId} | Action: {Action} | Location: {Location} | IpAddress: {IpAddress} | CorrelationId: {CorrelationId} | DurationMs: {DurationMs} | CustomData: {CustomData} | CreatedAt: {CreatedAt}{NewLine}")
                //.WriteTo.Email(
                //        emailInfo,
                //        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error, // Sends email only for errors and above
                //        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",  // Log format
                //        batchPostingLimit: 50, // Batch emails
                //        period: TimeSpan.FromMinutes(5)) // Send emails every 5 minutes
                .WriteTo.File(
                        path: "Logs/log.txt",  // Base log file
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",  // Log format
                        rollingInterval: RollingInterval.Day,  // Rolls daily
                        fileSizeLimitBytes: 1_000_000,  // 1 MB size limit before rolling
                        rollOnFileSizeLimit: true,  // Automatically roll file when size limit is reached
                        retainedFileCountLimit: 5  // Keep the last 5 log files
                                   )
                .WriteTo.RollingFile(
                        pathFormat: "Logs/log-{Date}.txt",  // Rolls the log file daily
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",  // Log format
                        retainedFileCountLimit: 7,  // Keeps the last 7 days' logs, older ones are deleted
                        fileSizeLimitBytes: null,   // No size limit per file, rolls based on the date
                        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information  // Log level threshold
                        )
                .WriteTo.MSSqlServer(
                        connectionString: configuration.GetConnectionString("DefaultConnection"), // Make sure this is set   in appsettings.json
                        tableName: "Logs",
                        autoCreateSqlTable: false, // Set to false if you already have the table created
                        columnOptions: GetColumnOptions()
                )
               .CreateLogger();
            //    .WriteTo.GrafanaLoki("https://vishalnigude5185.grafana.net/loki/api/v1/push") // Loki URL
            //    .WithBasicAuth("<your_grafana_cloud_username>", "<your_api_key>") // Replace with your username and API key
            //// Log at different levels
            //   .Information("User clicked on login.")
            ////Logging Specific Properties for Certain Logs
            //   .Error("User login failed", new Exception("Invalid password"), new
            //   {
            //       DurationMs = 300,
            //       CustomData = new { loginAttempts = 3 }
            //   });

            // Add Serilog to the DI container
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders(); // Optional: Clear default providers 
                loggingBuilder.AddSerilog(); // Add Serilog
            });

            // Ensure to flush logs before the application exits
            Log.CloseAndFlush();

            //map Serilog log properties to existing table columns
            // Example log entries
            Log.Information("User {UserId} performed action {Action} at {Time}", "123", "Login", DateTime.UtcNow);
            Log.Warning("User {UserId} performed action {Action} at {Time}", "123", "SuspiciousActivity", DateTime.UtcNow);
        }
        
        public static ColumnOptions GetColumnOptions()
        {
            // Define the SQL column options if using MSSqlServer sink
            var columnOptions = new ColumnOptions
            {
                AdditionalColumns = new List<SqlColumn>
                {
                     //new SqlColumn { ColumnName = "Timestamp", DataType = SqlDbType.DateTime },
                     //new SqlColumn { ColumnName = "Level", DataType = SqlDbType.NVarChar, DataLength = 50 },
                     //new SqlColumn { ColumnName = "Message", DataType = SqlDbType.NVarChar, DataLength = int.MaxValue }, // MAX for NVARCHAR(MAX)
                     //new SqlColumn { ColumnName = "Exception", DataType = SqlDbType.NVarChar, DataLength = int.MaxValue }, // MAX for NVARCHAR(MAX)
                     new SqlColumn { ColumnName = "Context", DataType = SqlDbType.NVarChar, DataLength = int.MaxValue }, // MAX for NVARCHAR(MAX)
                     new SqlColumn { ColumnName = "UserId", DataType = SqlDbType.NVarChar, DataLength = 255 },
                     new SqlColumn { ColumnName = "Action", DataType = SqlDbType.NVarChar, DataLength = 255 },
                     new SqlColumn { ColumnName = "Location", DataType = SqlDbType.NVarChar, DataLength = 255 },
                     new SqlColumn { ColumnName = "IpAddress", DataType = SqlDbType.NVarChar, DataLength = 50 },
                     new SqlColumn { ColumnName = "CorrelationId", DataType = SqlDbType.NVarChar, DataLength = 255 },
                     new SqlColumn { ColumnName = "DurationMs", DataType = SqlDbType.Int },
                     new SqlColumn { ColumnName = "CustomData", DataType = SqlDbType.NVarChar, DataLength = int.MaxValue }, // MAX for NVARCHAR(MAX)
                     new SqlColumn { ColumnName = "CreatedAt", DataType = SqlDbType.DateTime }
                }
            };
            return columnOptions;
        }
    }
}
