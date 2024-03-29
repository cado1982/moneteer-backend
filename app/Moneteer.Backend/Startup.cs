﻿using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Moneteer.Backend.Managers;
using Moneteer.Backend.Services;
using Moneteer.Domain.Guards;
using Moneteer.Domain.Helpers;
using Moneteer.Domain.Repositories;
using Moneteer.Models.Validation;
using Moneteer.Models;
using Microsoft.AspNetCore.HttpOverrides;
using System;
using Serilog;
using Moneteer.Backend.Handlers;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.AspNetCore;
using FluentValidation;

[assembly:ApiController]
namespace Moneteer.Backend
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IHostingEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (Environment.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true;
            }
            
            services.AddMvcCore()
                    .SetCompatibilityVersion(CompatibilityVersion.Latest)
                    .AddAuthorization()
                    .AddJsonFormatters()
                    .AddFluentValidation(fv => 
                    {
                        fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                        fv.RegisterValidatorsFromAssemblyContaining<TransactionValidator>();
                    });

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = Configuration["OpenIdConnectAuthority"];
                        options.RequireHttpsMetadata = false;
                        options.ApiName = "moneteer-api";
                        options.ApiSecret = Configuration["ApiSecret"];
                        options.EnableCaching = true;
                        options.CacheDuration = TimeSpan.FromMinutes(10);
                    });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins(Configuration["AllowedCorsOrigins"])
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            services.AddLazyCache();

            services.AddHttpContextAccessor();

            services.AddSingleton(new DatabaseConnectionInfo { ConnectionString = Configuration.GetConnectionString("Moneteer") });

            services.AddSingleton<IConnectionProvider, ConnectionProvider>();

            // Repositories
            services.AddTransient<IBudgetRepository, BudgetRepository>();
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IEnvelopeRepository, EnvelopeRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            services.AddTransient<ITransactionAssignmentRepository, TransactionAssignmentRepository>();
            services.AddTransient<IPayeeRepository, PayeeRepository>();
            services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();

            // Guards
            services.AddSingleton<BudgetGuard>();
            services.AddSingleton<AccountGuard>();
            services.AddSingleton<PayeeGuard>();
            services.AddSingleton<TransactionGuard>();
            services.AddSingleton<EnvelopeGuard>();
            services.AddSingleton<EnvelopeCategoryGuard>();
            services.AddSingleton<Guards>();

            // Managers
            services.AddTransient<IBudgetManager, BudgetManager>();
            services.AddTransient<IAccountManager, AccountManager>();
            services.AddTransient<ITransactionManager, TransactionManager>();
            services.AddTransient<IPayeeManager, PayeeManager>();
            services.AddTransient<IEnvelopeManager, EnvelopeManager>();

            // Services
            services.AddTransient<IUserInfoService, UserInfoService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSerilogRequestLogging();
            app.UseForwardedHeaders();

            app.UseCors("default");
            app.UseAuthentication();
            app.UseMiddleware<SubscriptionMiddleware>();
            app.UseMvc();
        }
    }
}
