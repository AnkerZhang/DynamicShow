using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using log4net;
using Microsoft.Extensions.Options;
using Senparc.Weixin.Entities;
using log4net.Repository;
using log4net.Config;
using System.IO;
using Senparc.Weixin.MP.Containers;
using Anker.WeiXin.MP.CoreDynamicShow.CommonService.Utilities;
using Microsoft.EntityFrameworkCore;
using Anker.WeiXin.MP.CoreDynamicShow.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Anker.WeiXin.MP.CoreDynamicShow
{
    public class Startup
    {
        public static ILoggerRepository repository { get; set; }
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(env.ContentRootPath)
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                 .AddEnvironmentVariables();

            Configuration = builder.Build();
            repository = LogManager.CreateRepository("NETCoreRepository");
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc();
            services.AddSession();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //添加Senparc.Weixin配置文件（内容可以根据需要对应修改）
            services.Configure<SenparcWeixinSetting>(Configuration.GetSection("SenparcWeixinSetting"));
            services.AddDbContext<DynamicShowContext>(options =>
            {
                options.UseMySQL(Configuration.GetConnectionString("MysqlDynamicShow"));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IOptions<SenparcWeixinSetting> senparcWeixinSetting, ILoggerFactory loggerFactory)
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
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=WeiXin}/{action=Index}/{id?}");
            });
            var log = LogManager.GetLogger(repository.Name, typeof(Startup));
            log.Info("Startup");

            ////注册微信
            AccessTokenContainer.Register(senparcWeixinSetting.Value.WeixinAppId, senparcWeixinSetting.Value.WeixinAppSecret);
            //Senparc.Weixin SDK 配置
            Senparc.Weixin.Config.IsDebug = true;
            Senparc.Weixin.Config.DefaultSenparcWeixinSetting = senparcWeixinSetting.Value;
            //提供网站根目录
            if (env.ContentRootPath != null)
            {
                Senparc.Weixin.Config.RootDictionaryPath = env.ContentRootPath;
                Server.AppDomainAppPath = env.ContentRootPath;// env.ContentRootPath;
            }
            Server.WebRootPath = env.WebRootPath;// env.ContentRootPath;
            UserContextSeed.SeedAsync(app, loggerFactory).Wait();
        }
    }
}
