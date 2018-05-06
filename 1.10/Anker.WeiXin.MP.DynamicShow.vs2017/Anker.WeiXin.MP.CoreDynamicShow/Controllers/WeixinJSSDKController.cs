using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anker.WeiXin.MP.CoreDynamicShow.CommonService.Utilities;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.Helpers;

namespace Anker.WeiXin.MP.CoreDynamicShow.Controllers
{
    public class WeixinJSSDKController : Controller
    {
        private string appId;
        private string appSecret;
        private string token;
        private string encodingAESKey;
        SenparcWeixinSetting _senparcWeixinSetting;
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(WeixinJSSDKController));
        public WeixinJSSDKController(IOptions<SenparcWeixinSetting> senparcWeixinSetting)
        {
            _senparcWeixinSetting = senparcWeixinSetting.Value;
            appId = _senparcWeixinSetting.WeixinAppId;
            appSecret = _senparcWeixinSetting.WeixinAppSecret;
            token = _senparcWeixinSetting.Token;
            encodingAESKey = _senparcWeixinSetting.EncodingAESKey;
        }
        public IActionResult Index()
        {
            var url = Server.GetAbsoluteUri(HttpContext.Request);
            var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(appId, appSecret, url);
            ViewBag.url = url;
            
            var state = "JeffreySu-" + DateTime.Now.Millisecond;//随机数，用于识别请求可靠性
            HttpContext.Session.SetString("State", state);//储存随机数到Session
            ViewBag.OAuthurl = OAuthApi.GetAuthorizeUrl(appId,
                "http://www.nbug.xin/WeixinJSSDK/IndexUserInfo?returnUrl=" + "".UrlEncode(),
                state, OAuthScope.snsapi_userinfo);

            log.Info("OAuthurl:::::::" + ViewBag.OAuthurl);
            return Redirect(OAuthApi.GetAuthorizeUrl(appId,
                "http://www.nbug.xin/WeixinJSSDK/IndexUserInfo?returnUrl=" + "".UrlEncode(),
                state, OAuthScope.snsapi_userinfo));
            
            //return View(jssdkUiPackage);
        }
        public IActionResult Index1(string code, string state, string returnUrl)
        {
            var url = Server.GetAbsoluteUri(HttpContext.Request);
            var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(appId, appSecret, url);
            ViewBag.url = url;
            return View(jssdkUiPackage);
        }
        public IActionResult getUrl(string returnUrl)
        {
            returnUrl = "http://www.nbug.xin/WeixinJSSDK/IndexUserInfo";
            var state = "JeffreySu-" + DateTime.Now.Millisecond;//随机数，用于识别请求可靠性
            HttpContext.Session.SetString("State", state);//储存随机数到Session
            var url= OAuthApi.GetAuthorizeUrl(appId,
                "http://www.nbug.xin/WeixinJSSDK/IndexUserInfo?returnUrl=" + returnUrl.UrlEncode(),
                state, OAuthScope.snsapi_userinfo);
            return Content(url);
        }
        public IActionResult IndexUserInfo(string code, string state, string returnUrl)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Redirect("http://www.nbug.xin/WeixinJSSDK/Index");
            }

            //if (state != HttpContext.Session.GetString("State") as string)
            //{
            //    //这里的state其实是会暴露给客户端的，验证能力很弱，这里只是演示一下，
            //    //建议用完之后就清空，将其一次性使用
            //    //实际上可以存任何想传递的数据，比如用户ID，并且需要结合例如下面的Session["OAuthAccessToken"]进行验证
            //    return Content("验证失败！请从正规途径进入！");
            //}

            OAuthAccessTokenResult result = null;

            //通过，用code换取access_token
            try
            {
                result = OAuthApi.GetAccessToken(appId, appSecret, code);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            if (result.errcode != ReturnCode.请求成功)
            {
                return Content("错误：" + result.errmsg);
            }
            //下面2个数据也可以自己封装成一个类，储存在数据库中（建议结合缓存）
            //如果可以确保安全，可以将access_token存入用户的cookie中，每一个人的access_token是不一样的
            HttpContext.Session.SetString("OAuthAccessTokenStartTime", DateTime.Now.ToString());
            HttpContext.Session.SetString("OAuthAccessToken", result.ToString());//错误
            //因为第一步选择的是OAuthScope.snsapi_userinfo，这里可以进一步获取用户详细信息
            try
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                OAuthUserInfo userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);
                var url = Server.GetAbsoluteUri(HttpContext.Request);
                var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(appId, appSecret, url);
                ViewBag.url = url;
                ViewBag.userInfo = userInfo;
                return View(jssdkUiPackage);
            }
            catch (ErrorJsonResultException ex)
            {
                return Content(ex.Message);
            }


            
        }
    }
}