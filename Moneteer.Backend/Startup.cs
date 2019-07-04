using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
using System;

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
            IdentityModelEventSource.ShowPII = true;

            services.AddMvcCore()
                    .AddAuthorization()
                    .AddJsonFormatters();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(options =>
            {
                options.Authority = $"https://localhost:4400";
                options.RequireHttpsMetadata = false;
                options.ApiName = "moneteer-api";
                options.ApiSecret = "eb18f78e-d660-448a-9e28-cae9790a2a2d";
            });

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });

            services.AddCors(options =>
            {
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins("https://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            services.AddHttpContextAccessor();
            services.AddHttpsRedirection(options => 
            {
                options.RedirectStatusCode = StatusCodes.Status301MovedPermanently;
                options.HttpsPort = 4300;
            });

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });

            services.AddSingleton(new DatabaseConnectionInfo { ConnectionString = Configuration["ConnectionStrings:App"] });

            services.AddSingleton<IConnectionProvider, ConnectionProvider>();

            // Repositories
            services.AddTransient<IBudgetRepository, BudgetRepository>();
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IEnvelopeRepository, EnvelopeRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            services.AddTransient<ITransactionAssignmentRepository, TransactionAssignmentRepository>();
            services.AddTransient<IPayeeRepository, PayeeRepository>();

            // Guards
            services.AddSingleton<BudgetGuard>();
            services.AddSingleton<AccountGuard>();
            services.AddSingleton<PayeeGuard>();
            services.AddSingleton<TransactionGuard>();
            services.AddSingleton<Guards>();

            // Managers
            services.AddTransient<IBudgetManager, BudgetManager>();
            services.AddTransient<IAccountManager, AccountManager>();
            services.AddTransient<ITransactionManager, TransactionManager>();
            services.AddTransient<IPayeeManager, PayeeManager>();
            services.AddTransient<IEnvelopeManager, EnvelopeManager>();

            // Validation
            services.AddSingleton<AccountValidationStrategy>();
            services.AddSingleton<BudgetValidationStrategy>();
            services.AddSingleton<EnvelopeValidationStrategy>();
            services.AddSingleton<EnvelopeCategoryValidationStrategy>();
            services.AddSingleton<IDataValidationStrategy<Transaction>, TransactionValidationStrategy>();

            // Services
            services.AddTransient<IUserInfoService, UserInfoService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseCors("default");
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
