using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace Dalisama.Extensions.Configuration.Consul.Example
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {

            Configuration = configuration;
            Configuration = (IConfiguration)new ConfigurationBuilder().AddApiConfiguration(options =>
            {
                options.ApiUri = "http://localhost:8500/v1/kv/";
                options.KeyPath = @"Example";
                options.ReloadOnChange = true;
                options.Headers = new Dictionary<string, string>
                {   // for authentication you can use one of those two but not both, don't be creedy
                    // ["X-Consul-Token"]="token"
                    // [Authorization]="Bearer <token>"
                };
                options.IgnoreSSL = true;
            }).Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<Keys>(Configuration.GetSection("MyKeys"));
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
