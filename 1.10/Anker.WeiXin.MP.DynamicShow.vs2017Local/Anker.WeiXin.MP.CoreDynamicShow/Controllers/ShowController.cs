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
using System.Text;

namespace Anker.WeiXin.MP.CoreDynamicShow.Controllers
{
    public class ShowController : BaseController
    {
        //public new ActionResult  OAuth(string action)
        //{
        //    return Redirect(OAuthApi.GetAuthorizeUrl(appId,
        //      "http://www.nbug.xin/Show/"+ action,
        //      "", OAuthScope.snsapi_userinfo));
        //}
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
            log = LogManager.GetLogger(Startup.repository.Name, typeof(ShowController));
            uid = 1;//  Convert.ToInt32(HttpContext.Session.GetString("uid") == null ? "0" : HttpContext.Session.GetString("uid"));
        }
        public async Task<IActionResult> Index(string art, string code, string state)
        {
            if (art != null && art != "")
            {
                HttpContext.Session.SetString("art", art);
            }
            else
            {
                art = HttpContext.Session.GetString("art");
            }
            log.Info("/Show/Index/++++++++++++++" + uid);
            WeiXinUserModel user = null;
            if (uid == 0)
            {
                if (string.IsNullOrEmpty(code))
                {
                    return Redirect("/Show/OAuth?url=Show/Index");

                }
                else
                {
                    adduser(code, _context);
                    uid = Convert.ToInt32(HttpContext.Session.GetString("uid"));

                }
            }
            user = await _context.WeiXinUser.FirstOrDefaultAsync(u => u.ID == Convert.ToInt32(uid));
            if (user == null)
                return Redirect("/Show/OAuth?url=Show/Index");
            var url = "http://www.nbug.xin/Show/Index?art=" + art;
            var article = await _context.WeiXinArticle.FirstOrDefaultAsync(f => f.qrCode == art);
            if (article == null) return Content("错误");
            ViewBag.user = user;
            //var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(appId, appSecret, url);
            //ViewBag.jssdkUiPackage = jssdkUiPackage;
            ViewBag.url = url;
            return View(article);
        }
        public async Task ArticleApi(string type)
        {
            string art = HttpContext.Session.GetString("art");
            log.Info("/Show/ArticleApi/++++++++++++++" + uid);
            //查到文章 及详情
            var article = await _context.WeiXinArticle.Include(c => c.articleInfoList).FirstOrDefaultAsync(f => f.qrCode == art);
            ///用户
            var user = await _context.WeiXinUser.FirstOrDefaultAsync(f => f.ID == uid);
            var weiXinArticle = await _context.WeiXinArticleInfo.Include(i => i.user).Where(p => article.articleInfoList.Select(s => s.ID).Contains(p.ID)).ToListAsync();
            var articleInfo = weiXinArticle.FirstOrDefault(w => w.user.ID == uid);
            if (type == "s" && articleInfo == null)
                return;
            if (articleInfo == null)
            {
                articleInfo = new WeiXinArticleInfoModel()
                {
                    user = user,
                    beginTime = DateTime.Now,
                    amount = 1,
                    opentNumber = 1,
                    endTime = DateTime.Now,
                    spendingDate = 10 + 1
                };
                article.articleInfoList.Add(articleInfo);
                _context.WeiXinArticle.Update(article);
                _context.SaveChanges();
            }
            else
            {
                switch (type)
                {
                    case "open":
                        articleInfo.opentNumber = articleInfo.opentNumber + 1;
                        articleInfo.endTime = DateTime.Now;
                        articleInfo.spendingDate = articleInfo.spendingDate + 10;
                        break;
                    case "s":
                        articleInfo.amount = articleInfo.amount + 3;
                        articleInfo.endTime = DateTime.Now;
                        articleInfo.spendingDate = articleInfo.spendingDate + 3;
                        break;
                    default:
                        break;
                }
                _context.Update(articleInfo);
                _context.SaveChanges();
            }



        }

        public async Task<IActionResult> Js(string art, string code, string state)
        {
            if (art != null && art != "")
            {
                HttpContext.Session.SetString("art", art);
            }
            else
            {
                art = HttpContext.Session.GetString("art");
            }
            log.Info("/Show/Js/++++++++++++++" + uid);
            WeiXinUserModel user = null;
            if (uid == 0)
            {
                if (string.IsNullOrEmpty(code))
                {
                    return Redirect("/Show/OAuth?url=Show/Js");

                }
                else
                {
                    adduser(code, _context);
                    uid = Convert.ToInt32(HttpContext.Session.GetString("uid"));
                }
            }
            user = await _context.WeiXinUser.FirstOrDefaultAsync(u => u.ID == Convert.ToInt32(uid));
            if (user == null)
                return Redirect("/Show/OAuth?url=Show/Js");
            var url = "http://www.nbug.xin/Show/Js?art=" + art;
            var article = await _context.WeiXinArticle.FirstOrDefaultAsync(f => f.qrCode == art);
            if (article == null) return Content("错误");
            ViewBag.user = user;
            //var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(appId, appSecret, url);
            //ViewBag.jssdkUiPackage = jssdkUiPackage;
            ViewBag.url = url;
            return View(article);
        }
        public async Task<IActionResult> images(string art)
        {
            log.Info("/Show/images/++++++++++++++art:" + art);
            var article = await _context.WeiXinArticle.FirstOrDefaultAsync(f => f.qrCode == art);
            if (article == null) return Content("错误");
            return Content(article.content);
        }

    }
}