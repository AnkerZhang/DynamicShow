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
                        #endregion
                    }
                    if (!context.WeiXinUser.Any())
                    {
                        var user = new Models.WeiXinUserModel()
                        {
                            city = "东城",
                            country = "中国",
                            headimgurl = "http://thirdwx.qlogo.cn/mmopen/UGEBLia3UQ6SBrHOibVHQGVuJYE4mXH30EvpWlGBPGW3YGW0zxpJrAktLSyl5g9eNCnM49aY82aK6cd6DfNzLtkge8WAPsmu3s/132",
                            nickname = "Anker_张",
                            openid = "oCnkb1LaZxV1lcA0DwlvsCtHQs-c",
                            province = "北京",
                            sex = 1,
                            time = DateTime.Now,
                            unionid = ""

                        };
                        var userinfo = new List<Models.WeiXinUserInfoModel>() {
                                new Models.WeiXinUserInfoModel(){
                                    logTime=DateTime.Now,
                                    remarks="初始化1"
                                },
                                 new Models.WeiXinUserInfoModel(){
                                    logTime=DateTime.Now,
                                    remarks="初始化2"
                                }
                            };
                        user.userInfoList = userinfo;
                        context.WeiXinUser.Add(user);
                        context.SaveChanges();
                        var article = new Models.WeiXinArticleModel()
                        {
                            author = "author",
                            content = "content",
                            Music = "Music",
                            qrCode = "qrCode",
                            state = 1,
                            time = DateTime.Now,
                            title = "title",
                            titleImg = "titleImg",
                            userID = user
                        };
                        
                        var weiXinArticleList = new List<Models.WeiXinArticleInfoModel>()
                            {
                                new Models.WeiXinArticleInfoModel()
                                {
                                    amount=10,
                                    endTime =DateTime.Now,
                                    beginTime =DateTime.Now,
                                    opentNumber=1,
                                    spendingDate =20,
                                    user =user
                                }
                            };
                        
                        
                        article.articleInfoList = weiXinArticleList;
                        context.WeiXinArticle.Add(article);
                        var articleInfoLoglist = new Models.WeiXinArticleInfoLogModel()
                        {
                            amount = 10,
                            beginTime = DateTime.Now,
                            endTime = DateTime.Now,
                            remarks = "初始化",
                            articleInfoID = 1
                        };
                        context.WeiXinArticleInfoLog.Add(articleInfoLoglist);
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    if (retryForAvaiability < 20)
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
