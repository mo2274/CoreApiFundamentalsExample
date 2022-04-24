using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace CoreCodeCamp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CampContext>(option => option
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole())));
            services.AddScoped<ICampRepository, CampRepository>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddApiVersioning(opt =>
            {
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(2, 0);
                opt.ReportApiVersions = true;
                //opt.ApiVersionReader = new QueryStringApiVersionReader("ver");
                //opt.ApiVersionReader = new HeaderApiVersionReader("X-Version");
                opt.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("ver", "version"),
                    new HeaderApiVersionReader("X-Version"));
            });
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(cfg =>
            {
                cfg.MapControllers();
            });
        }
    }
}
