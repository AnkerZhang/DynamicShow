using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Anker.WeiXin.MP.CoreDynamicShow.Data;
using log4net;
using Microsoft.EntityFrameworkCore;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.Entities;
using Microsoft.Extensions.Options;
using Anker.WeiXin.MP.CoreDynamicShow.CommonService.Utilities;

namespace Anker.WeiXin.MP.CoreDynamicShow.Controllers
{
    public class RootController : Controller
    {
        private string appId;
        private string appSecret;
        private string token;
        private string encodingAESKey;
        SenparcWeixinSetting _senparcWeixinSetting;
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(WeixinJSSDKController));
        private readonly DynamicShowContext _context;
        public RootController(DynamicShowContext context, IOptions<SenparcWeixinSetting> senparcWeixinSetting)
        {
            _context = context;
            _senparcWeixinSetting = senparcWeixinSetting.Value;
            appId = _senparcWeixinSetting.WeixinAppId;
            appSecret = _senparcWeixinSetting.WeixinAppSecret;
            token = _senparcWeixinSetting.Token;
            encodingAESKey = _senparcWeixinSetting.EncodingAESKey;

        }
        public async Task<IActionResult> Index(string openid)
        {
            var url = Server.GetAbsoluteUri(HttpContext.Request);
            var jssdkUiPackage=JSSDKHelper.GetJsSdkUiPackage(appId, appSecret, url);
            log.Info("openid" + openid);
            var user = await _context.WeiXinUserInfo.FirstOrDefaultAsync(f => f.openid == openid);
            if (user == null)
                return Content("找不到用户");
            var article = await _context.WeiXinArticle.FirstOrDefaultAsync(c => c.UserInfo == user);
            if(article==null)
                return Content("找不到用户文章");
            var articleInfoList = await _context.WeiXinArticleInfo.Include(c => c.UID).Where(c => c.AID == article).OrderByDescending(c=>c.SpendingDate).ToListAsync();
            ViewBag.AppId = jssdkUiPackage.AppId;
            ViewBag.Timestamp = jssdkUiPackage.Timestamp;
            ViewBag.NonceStr = jssdkUiPackage.NonceStr;
            ViewBag.Signature = jssdkUiPackage.Signature;
            ViewBag.url = url;
            return View(articleInfoList);
        }
    }
}