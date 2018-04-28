using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Json;
using Microsoft.EntityFrameworkCore;
using AardvarkREST.Models;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;

namespace AardvarkREST
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
                Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddMvc();
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            if (connectionString == null)
            {
                var JSON = System.IO.File.ReadAllText( "appsettings.json");
                JsonObject jsonDoc = (JsonObject)JsonObject.Parse(JSON);
                string strLine = jsonDoc["ConnectionStrings"]["DefaultConnection"];
                connectionString = strLine;
            }
           // var connection = @"Server=LIVINGROOM\SQLEXPRESS;Database=OIMOpenWF;Trusted_Connection=True;ConnectRetryCount=0";
            services.AddDbContext<WFContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);
            services.AddScoped<IWFChartRepository, WFChartRepository>();
            services.AddScoped<IWFTaskRepository, WFTaskRepository>();
            services.AddScoped<IWFRouteRepository, WFRouteRepository>();
            services.AddScoped<IWFItemRepository, WFItemRepository>();
            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "AardvarkREST.xml");
                c.IncludeXmlComments(xmlPath);
            });    
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseStatusCodePages();
            app.UseMvc();
        }
    }
}
