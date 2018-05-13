using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Anker.WeiXin.MP.CoreDynamicShow.Data;
using Microsoft.AspNetCore.Hosting;
using Senparc.Weixin.Entities;
using Microsoft.Extensions.Options;
using log4net;
using Microsoft.EntityFrameworkCore;
using Senparc.Weixin.MP.Helpers;
using Anker.WeiXin.MP.CoreDynamicShow.CommonService.Utilities;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin;
using Anker.WeiXin.MP.CoreDynamicShow.Models;
using Senparc.Weixin.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Anker.WeiXin.MP.CoreDynamicShow.Controllers
{
    public class ShowController : BaseController
    {
        public ActionResult OAuth(string art)
        {
            return Redirect(OAuthApi.GetAuthorizeUrl(appId,
              "http://www.nbug.xin/Show/Index?returnUrl=" + "".UrlEncode(),
              "", OAuthScope.snsapi_userinfo));
        }
        public ShowController(DynamicShowContext context, IHostingEnvironment host, IOptions<SenparcWeixinSetting> senparcWeixinSetting, IHttpContextAccessor accessor)
        {
            _host = host;
            _context = context;
            _senparcWeixinSetting = senparcWeixinSetting.Value;
            appId = _senparcWeixinSetting.WeixinAppId;
            appSecret = _senparcWeixinSetting.WeixinAppSecret;
            token = _senparcWeixinSetting.Token;
            HttpContext = accessor.HttpContext;
            encodingAESKey = _senparcWeixinSetting.EncodingAESKey;
            
            log = LogManager.GetLogger(Startup.repository.Name, typeof(ArticleController));
            uid = Convert.ToInt32(HttpContext.Session.GetString("uid") == null ? "0" : HttpContext.Session.GetString("uid"));
        }
        public async Task<IActionResult> Index(string art,string code, string state)
        {
            if (art != null && art != "")
            {
                HttpContext.Session.SetString("art", art);
            }
            else {
                art= HttpContext.Session.GetString("art");
            }
            log.Info("/Show/Index/++++++++++++++" + uid);
            WeiXinUserModel user = null;
            if (uid == 0)
            {
                if (string.IsNullOrEmpty(code))
                {
                    return Redirect("/Show/OAuth");
                    
                }
                else
                {
                    adduser(code, _context);
                    uid = Convert.ToInt32(HttpContext.Session.GetString("uid"));
                    
                }
            }
            user = await _context.WeiXinUser.FirstOrDefaultAsync(u => u.ID == Convert.ToInt32(uid));
            if (user==null)
                return Redirect("/Show/OAuth?art="+ art);
            var url = Server.GetAbsoluteUri(HttpContext.Request);
            var article= await _context.WeiXinArticle.FirstOrDefaultAsync(f => f.qrCode == art);
            if (article == null) return Content("错误");
            ViewBag.user = user;
            var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(appId, appSecret, url);
            ViewBag.jssdkUiPackage = jssdkUiPackage;
            ViewBag.url = url;
            return View(article);
        }
       public async Task ArticleApi(string type)
        {
           string art = HttpContext.Session.GetString("art");
            log.Info("/Show/ArticleApi/++++++++++++++" + uid);
            try
            {
                var article = await _context.WeiXinArticle.Include(c => c.articleInfoList).Include(c => c.userID).FirstOrDefaultAsync(f => f.qrCode == art);
                var user = await _context.WeiXinUser.FirstOrDefaultAsync(f => f.ID == uid);
                var weiXinArticle = article.articleInfoList.Where(w => w.user.ID == uid).FirstOrDefault();
                if (type == "s" && weiXinArticle == null)
                    return;
                if (weiXinArticle == null)
                {
                    weiXinArticle = new Models.WeiXinArticleInfoModel()
                    {
                        user = user,
                        beginTime = DateTime.Now,
                        amount = 1,
                        opentNumber = 1,
                        endTime = DateTime.Now,
                        spendingDate = 10 + 1
                    };
                    article.articleInfoList.Add(weiXinArticle);
                    _context.WeiXinArticle.Update(article);
                    _context.SaveChanges();
                }
                else
                {
                    switch (type)
                    {
                        case "open":
                            weiXinArticle.opentNumber = weiXinArticle.opentNumber + 1;
                            weiXinArticle.endTime = DateTime.Now;
                            weiXinArticle.spendingDate = weiXinArticle.spendingDate + 10;
                            break;
                        case "s":
                            weiXinArticle.amount = weiXinArticle.amount + 3;
                            weiXinArticle.endTime = DateTime.Now;
                            weiXinArticle.spendingDate = weiXinArticle.spendingDate + 3;
                            break;
                        default:

                            break;
                    }
                    _context.Update(weiXinArticle);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Info("/Show/ArticleApi/++++++++++++++" + ex.Message.ToString());
                return;
            }
            
            
        }
    }
}