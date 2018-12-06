using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using TicketsWebApi.Helpers;
using TicketsWebApi.Models;
using TicketsWebApi.Services;

namespace TicketsWebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private AppSettings _settings { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _settings = Configuration.GetSection("AppSettings").Get<AppSettings>();
            if (!_settings.Initializated)
            {
                Initialize(_settings);
            }
        }

        //local db string -- data source=(localdb)\MSSQLLocalDB;Initial Catalog=TicketDbDefault.dev;Integrated Security=True;
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = "http://localhost:4000",
                        ValidAudience = "http://localhost:4000",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ticketwebapiservice@secretkey"))
                    };
                });

            services.Configure<SmtpData>(Configuration.GetSection("SmtpData"));
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));


            services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddScoped<ISmtpService, SmtpService>();

            services.AddDbContext<AppDbContext>(builder => builder.UseSqlServer(_settings.ConnectionString));
            services.AddCors();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, AppDbContext context,
            IAuthenticateService userService)
        {
            app.UseAuthentication();

            context.Database.EnsureCreated();
            userService.Create(new User {Email = _settings.AdminLogin, Role = UserRole.Administrator},
                _settings.AdminPassword);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());


            app.UseMvc(rb =>
            {
                rb.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}"
            );
            //defaults: new {controller = "Home", action = "Index"});
        });
        }

        private void Initialize(AppSettings settings)
        {
            Console.WriteLine("Введите данные для первого запуска");
            Console.Write("Строка подключения:");
            settings.ConnectionString = Console.ReadLine();
            Console.Write("Email администратора:");
            settings.AdminLogin = Console.ReadLine();
            Console.Write("Пароль администратора:");
            settings.AdminPassword = Console.ReadLine();
            settings.Initializated = true;

            var json = "{\"AppSettings\":" + JsonConvert.SerializeObject(settings) + "}";
            File.WriteAllText($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", json);
        }
    }
}