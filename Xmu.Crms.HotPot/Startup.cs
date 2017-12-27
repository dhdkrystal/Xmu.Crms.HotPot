using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Xmu.Crms.Shared.Models;
using System;
using Microsoft.IdentityModel.Tokens;

namespace Xmu.Crms.HotPot
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        //private SymmetricSecurityKey _signingKey;
       // private TokenValidationParameters _tokenValidationParameters;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //JWT参数
            //_signingKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(_configuration["Keys:ServerSecretKey"]));
            //_tokenValidationParameters = new TokenValidationParameters
            //{
            //    ValidateIssuerSigningKey = true,
            //    IssuerSigningKey = _signingKey,
            //    RequireExpirationTime = true,
            //    ValidateLifetime = true,
            //    ValidateAudience = false,
            //    ValidateActor = false,
            //    ValidateIssuer = false

            //};

            var connStr = _configuration.GetConnectionString("MYSQL57");
            System.Diagnostics.Debug.WriteLine(connStr+" hhhhh");
            services.AddDbContext<CrmsContext>(options => options.UseMySql(connStr));
            
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
