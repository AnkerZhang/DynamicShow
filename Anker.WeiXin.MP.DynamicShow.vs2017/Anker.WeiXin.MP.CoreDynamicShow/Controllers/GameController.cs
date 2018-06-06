using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Anker.WeiXin.MP.CoreDynamicShow.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Senparc.Weixin.Entities;
using Microsoft.AspNetCore.Http;
using log4net;
using Senparc.Weixin.MP.Helpers;
using Microsoft.EntityFrameworkCore;
using Anker.WeiXin.MP.CoreDynamicShow.Models;

namespace Anker.WeiXin.MP.CoreDynamicShow.Controllers
{
    public class GameController : BaseController
    {
        public GameController(DynamicShowContext context, IHostingEnvironment host, IOptions<SenparcWeixinSetting> senparcWeixinSetting, IHttpContextAccessor accessor)
        {
            _host = host;
            _context = context;
            _senparcWeixinSetting = senparcWeixinSetting.Value;
            appId = _senparcWeixinSetting.WeixinAppId;
            appSecret = _senparcWeixinSetting.WeixinAppSecret;
            token = _senparcWeixinSetting.Token;
            HttpContext = accessor.HttpContext;
            encodingAESKey = _senparcWeixinSetting.EncodingAESKey;
            log = LogManager.GetLogger(Startup.repository.Name, typeof(GameController));
            uid =   Convert.ToInt32(HttpContext.Session.GetString("uid") == null ? "0" : HttpContext.Session.GetString("uid"));
        }
        public IActionResult Index(string code)
        {

            if (uid == 0)
            {
                log.Info("uid==0");
                if (string.IsNullOrEmpty(code))
                {
                    log.Info("code== ");
                    return Redirect("/Game/OAuth?url=Game/Index");

                }
                else
                {
                    log.Info("code==" + code);
                    adduser(code, _context);

                }
                uid = Convert.ToInt32(HttpContext.Session.GetString("uid") == null ? "0" : HttpContext.Session.GetString("uid"));
            }
            if (uid == 0)
            {
                return Redirect("/Game/OAuth?url=Game/Index");
            }
            log.Info("1");
            var url = "http://www.nbug.xin/Game/Index";
            var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(appId, appSecret, url);
            log.Info("2");
            return View(jssdkUiPackage);
            //return View();
        }
        public async Task ArticleApi(int n,int b)
        {
            int art = 79;
            n = n / 1000;
            b = b / 1000;
            var article = await _context.WeiXinArticle.Include(c => c.articleInfoList).FirstOrDefaultAsync(f => f.ID == art);
            ///用户
            var user = await _context.WeiXinUser.FirstOrDefaultAsync(f => f.ID == uid);
            var weiXinArticle = await _context.WeiXinArticleInfo.Include(i => i.user).Where(p => article.articleInfoList.Select(s => s.ID).Contains(p.ID)).ToListAsync();
            var articleInfo = weiXinArticle.FirstOrDefault(w => w.user.ID == uid);

            if (articleInfo == null)
            {
                articleInfo = new WeiXinArticleInfoModel()
                {
                    user = user,
                    beginTime = DateTime.Now,
                    amount = n,
                    opentNumber = 1,
                    endTime = DateTime.Now,
                    spendingDate = b
                };
                article.articleInfoList.Add(articleInfo);
                _context.WeiXinArticle.Update(article);
                _context.SaveChanges();
            }
            else
            {
                articleInfo.opentNumber = articleInfo.opentNumber + 1;
                articleInfo.endTime = DateTime.Now;
                articleInfo.amount = n;
                articleInfo.spendingDate = b;
                _context.Update(articleInfo);
                _context.SaveChanges();
            }
        }
    }
}