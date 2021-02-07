using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MegoTest.Service;
using MegoTest.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using MegoTest.DAL;
using MegoTest.DAL.Interfaces;

namespace MegoTest.WEB
{
    public class Startup
    {
        public delegate IServiceCollection ServiceResolver(string key);

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<DbContext, MegoDbContext>(options => options.UseSqlServer(connection));

            services.AddScoped<IExternalA, ExternalA>();
            services.AddScoped<IExternalB, ExternalB>();
            services.AddScoped<IExternalC, ExternalC>();
            services.AddScoped<IExternalD, ExternalD>();

            services.AddScoped<IMetricsService, MetricsService>();


            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSwaggerGen();

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

            app.UseSwagger();

            app.UseSwaggerUI();
        }
    }
}
