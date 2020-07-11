using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RobicServer.Models;
using RobicServer.Services;

namespace RobicServer
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
            // Configures database using settings defined in the appsettings config
            services.Configure<ExerciseDatabaseSettings>(Configuration.GetSection(nameof(ExerciseDatabaseSettings)));
            // Database interface is registered with DI for a singleton service lifetime
            services.AddSingleton<IExerciseDatabaseSettings>(
                serviceProvider => serviceProvider.GetRequiredService<IOptions<ExerciseDatabaseSettings>>().Value
            );
            services.AddSingleton<ExerciseService>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
