using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyLab.Db;
using MyLab.Indexer.Common;
using MyLab.Mq.PubSub;

namespace MyLab.Indexer
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
            var configSection = Configuration.GetSection("Indexer");

            var dataProvider = DataProviders.Get<IndexerOptions>(configSection);

            services
                .Configure<IndexerOptions>(configSection)
                .AddMqConsuming(reg =>
                {
                    reg.RegisterConsumerByOptions<IndexerOptions, string>(
                        opts => opts.Queue, 
                        queue => new MqConsumer<IndexingMsg, IndexerConsumerLogic>(queue));
                })
                .AddDbTools(Configuration, dataProvider);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
