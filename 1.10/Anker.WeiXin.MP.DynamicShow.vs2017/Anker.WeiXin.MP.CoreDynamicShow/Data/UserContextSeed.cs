using Anker.WeiXin.MP.CoreDynamicShow.CommonService.MyWebsite.HandlersServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anker.WeiXin.MP.CoreDynamicShow.Data
{
    public class UserContextSeed
    {
        private ILogger<UserContextSeed> _logger;
        public UserContextSeed(ILogger<UserContextSeed> logger)
        {
            _logger = logger;
        }
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder, ILoggerFactory loggerFactory, int? retry = 0)
        {
            var retryForAvaiability = retry.Value;
            using (var scope = applicationBuilder.ApplicationServices.CreateScope())
            {
                try
                {
                    var context = (DynamicShowContext)scope.ServiceProvider.GetService(typeof(DynamicShowContext));
                    var logger = (ILogger<UserContextSeed>)scope.ServiceProvider.GetService(typeof(ILogger<UserContextSeed>));
                    logger.LogDebug("开始UserContextSeed SeedAsync方法");
                    context.Database.Migrate();
                    if (!context.Users.Any())
                    {

                        #region
                        context.Users.Add(
                            new User()
                            {
                                NickName = "Anker",
                                address = "北京",
                                State = 1,
                                country = "中国",
                                province = "华北",
                                city = "北京",
                                IP = "124.65.149.194",
                                Image = "/Images/default.png",
                                Meaages = new List<Message>()
                                {
                                    new Message()
                                    {
                                        NickName = "张龙",
                                        country="中国",
                                        province="华北",
                                        city="北京",
                                        IP= "124.65.149.194",
                                        Content = "您好Anker",
                                        Email = "1515@qq.com",
                                        Image= "/Images/default.png",
                                        Time = Convert.ToDateTime("2018-3-26")
                                    }
                                },

                                Content = "this me",
                                Email = "793087382@qq.com",
                                Time = Convert.ToDateTime("2018-3-01")
                            });
                        context.Users.Add(
                            new User()
                            {
                                NickName = "嘎~嘎~嘎~)",
                                address = "上海",
                                Content = "你好 me",
                                State = 1,
                                Email = "67382@qq.com",
                                country = "中国",
                                province = "华北",
                                Image = "/Images/default.png",
                                IP = "124.65.149.194",
                                city = "北京",
                                Time = Convert.ToDateTime("2018-3-02")
                            });
                        context.Users.Add(
                            new User()
                            {
                                NickName = "bibili",
                                address = "长沙",
                                Content = "我在长沙",
                                Email = "32748923@qq.com",
                                State = 1,
                                Image = "/Images/default.png",
                                country = "中国",
                                province = "华北",
                                city = "北京",
                                IP = "124.65.149.194",
                                Time = Convert.ToDateTime("2018-3-06"),
                                Meaages = new List<Message>()
                                {
                                    new Message()
                                    {
                                        NickName = "张龙",
                                        country="中国",
                                        province="华北",
                                        city="北京",
                                        IP= "124.65.149.194",
                                        Image= "/Images/default.png",
                                        Content = "您好Anker",
                                        Email = "1515@qq.com",
                                        Time = Convert.ToDateTime("2018-3-26")
                                    },
                                    new Message()
                                    {
                                        NickName = "赵虎",
                                        country="中国",
                                        province="华北",
                                        city="北京",
                                        Image= "/Images/default.png",
                                        IP= "124.65.149.194",
                                        Content = "您好bibili",
                                        Email = "151155@qq.com",
                                        Time = Convert.ToDateTime("2018-3-26")
                                    }
                                },
                            });
                        context.Users.Add(
                            new User()
                            {
                                NickName = "数据库",
                                address = "高碑店",
                                Content = "高碑店是我家",
                                State = 1,
                                Image = "/Images/default.png",
                                country = "中国",
                                province = "华北",
                                city = "北京",
                                IP = "124.65.149.194",
                                Email = "119344925682@qq.com",
                                Time = Convert.ToDateTime("2018-3-01")
                            });
                        context.Users.Add(
                            new User()
                            {
                                NickName = "屌丝",
                                address = "华山",
                                Image = "/Images/default.png",
                                State = 1,
                                Content = "我在华山吃烤鸡",
                                Email = "2554@qq.com",
                                country = "中国",
                                province = "华北",
                                city = "北京",
                                IP = "124.65.149.194",
                                Time = Convert.ToDateTime("2018-3-26")
                            });
                        context.SaveChanges();
                        for (int i = 0; i < 100; i++)
                        {
                            context.Users.Add(
                            new User()
                            {
                                NickName = "User" + i.ToString("000"),
                                address = "Address" + i.ToString("000"),
                                State = 1,
                                country = "中国",
                                province = "华北",
                                city = "北京",
                                Image = "/Images/default.png",
                                IP = "124.65.149.194",
                                Content = "我在华山吃烤鸡" + i.ToString("000"),
                                Email = i.ToString("000") + "@qq.com",
                                Time = Convert.ToDateTime("2018-3-26")
                            });
                            context.SaveChanges();
                        }
                        #endregion

                    }
                }
                catch (Exception ex)
                {
                    if (retryForAvaiability < 10)
                    {
                        retryForAvaiability++;
                        var logger = loggerFactory.CreateLogger(typeof(UserContextSeed));
                        logger.LogError(ex.Message);
                        await SeedAsync(applicationBuilder, loggerFactory, retryForAvaiability);
                    }
                }

            }
        }
    }
}
