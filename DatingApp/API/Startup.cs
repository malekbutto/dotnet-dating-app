using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 3 types of services lifetimes after we start it:
            //AdSingleton: dies (disposed) with the application (too long for service we need on the call level) 
            //AddScoped: dies (disposed) with the http request (in this case its scoped to the request , we create it on http call (injected to the controller), most useful in Web Apps)
            //AddTransient: dies (disposed) on method finishing, creates every time they are injected or requested.

            services.AddScoped<ITokenService, TokenService>();
            
            services.AddDbContext<DataContext>(optiones => {
                optiones.UseSqlite(_config.GetConnectionString("DefaultConnection"));
            });

            services.AddControllers(); 
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(policy => 
            policy
            .AllowAnyHeader() // allow any header (like authentication related headers)
            .AllowAnyMethod() // allow any method (HTTP Verb) (like GET, POST, PUT, DELETE)
            .WithOrigins("https://localhost:4200")  // our frontend
            );

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
