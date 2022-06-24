using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using MTracking.API.Swagger;
using MTracking.BLL.BackgroundServices.JobFactory;
using MTracking.BLL.BackgroundServices.Models;
using MTracking.BLL.BackgroundServices.Services;
using MTracking.BLL.Mappings;
using MTracking.BLL.Models.Implementations;
using MTracking.BLL.Services.Abstractions;
using MTracking.BLL.Services.Implementations;
using MTracking.DAL;
using MTracking.DAL.UnitOfWork;

namespace MTracking.API.Extensions
{
    public static class ServiceExtension
    {

        public static void AddSqlServer(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {

            var server = Environment.GetEnvironmentVariable("RDS_SERVER");
            var port = Environment.GetEnvironmentVariable("RDS_PORT");
            var dbName = Environment.GetEnvironmentVariable("RDS_DB_NAME");
            var userId = Environment.GetEnvironmentVariable("RDS_USERID");
            var password = Environment.GetEnvironmentVariable("RDS_PASSWORD");

            var prodConnection = $"Server={server},{port};Database={dbName};User={userId};Password={password};TrustServerCertificate=true";
            var connectionString = webHostEnvironment.IsDevelopment() 
                ? configuration.GetConnectionString("DevConnection") 
                : configuration.GetConnectionString("ProdConnection");

            //services.AddDbContext<MTrackingDbContext>(x => x.UseSqlServer(prodConnection).EnableSensitiveDataLogging(), ServiceLifetime.Transient);
            services.AddDbContext<MTrackingDbContext>(x => x.UseSqlServer(connectionString).EnableSensitiveDataLogging(), ServiceLifetime.Transient);
        }

        public static void AddServices(this IServiceCollection services)
        {
            services
                .AddTransient<IUserInfoService, UserInfo>()
                .AddTransient<IUnitOfWork, UnitOfWork>()
                .AddScoped<IJwtService, JwtService>()
                .AddScoped<IFileService, FileService>()
                .AddScoped<ITopicService, TopicService>()
                .AddScoped<IDescriptionService, DescriptionService>()
                .AddScoped<ITimeLogService, TimeLogService>()
                .AddScoped<IReminderService, ReminderService>()
                .AddScoped<IEmailService, EmailService>()
                .AddScoped<ITimerService, TimerService>()
                .AddScoped<IDeviceService, DeviceService>()
                .AddScoped<IFirebaseService, FirebaseService>()
                .AddScoped<IPushService, PushService>()
                .AddHostedService<PushNotificationBackgroundService>()
                .AddScoped<IAuthenticationService, AuthenticationService>();
        }

        public static void AddImportExportService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            services.AddSingleton<ImportCsvQuartzJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(ImportCsvQuartzJob),
                minutes: int.Parse(configuration["ImportCSV:TimeDelay"])));

            services.AddHostedService<QuartzImportService>();
        }

        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddSingleton(new MapperConfiguration(c =>
            {
                c.AddProfile(new UserProfile());
                c.AddProfile(new FileProfile());
                c.AddProfile(new TopicProfile());
                c.AddProfile(new DescriptionProfile());
                c.AddProfile(new TimeLogProfile());
                c.AddProfile(new ReminderProfile());
                c.AddProfile(new TimerProfile());
                c.AddProfile(new DeviceProfile());
            }).CreateMapper());
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MTracking.API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer info field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.OperationFilter<AddRequiredHeaderParameter>();
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                        },
                        new string[] {}
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, true);
            });

            services.AddSwaggerGenNewtonsoftSupport();
        }

        public static void AddJWTAuthentication(this IServiceCollection services, IConfiguration configurations)
        {
            var key = configurations["Jwt:Key"];
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = configurations[$"Jwt:{nameof(JwtIssuerOptions.Issuer)}"];
                options.Audience = configurations[$"Jwt:{nameof(JwtIssuerOptions.Audience)}"];
                options.AccessTokenLifetime = int.Parse(configurations[$"Jwt:{nameof(JwtIssuerOptions.AccessTokenLifetime)}"]);
                options.RefreshTokenLifetime = int.Parse(configurations[$"Jwt:{nameof(JwtIssuerOptions.RefreshTokenLifetime)}"]);
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.RequireHttpsMetadata = bool.Parse(configurations["Jwt:RequireHttpsMetadata"]);
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateActor = bool.Parse(configurations["Jwt:ValidateActor"]),
                        ValidateAudience = bool.Parse(configurations["Jwt:ValidateAudience"]),
                        ValidateLifetime = bool.Parse(configurations["Jwt:ValidateLifetime"]),
                        ValidateIssuerSigningKey = bool.Parse(configurations["Jwt:ValidateIssuerSigningKey"]),
                        ValidIssuer = configurations["Jwt:Issuer"],
                        ValidAudience = configurations["Jwt:Audience"],
                        IssuerSigningKey = signingKey,
                    };
                });
        }
    }
}
