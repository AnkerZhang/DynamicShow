using Anker.WeiXin.MP.CoreDynamicShow.Data;
using Anker.WeiXin.MP.CoreDynamicShow.Models;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anker.WeiXin.MP.CoreDynamicShow.Controllers
{
    public class BaseController: Controller
    {
        
        protected string appId;
        protected string appSecret;
        protected string token;
        protected  DynamicShowContext _context;
        protected string encodingAESKey;
        protected SenparcWeixinSetting _senparcWeixinSetting;
        protected ILog log;
        protected IHostingEnvironment _host = null;
        protected int uid = 0;
        protected HttpContext HttpContext;

        public void adduser(string code, DynamicShowContext context)
        {
            OAuthAccessTokenResult result = null;
            try
            {
                result = OAuthApi.GetAccessToken(appId, appSecret, code);
            }
            catch (Exception ex)
            {
                return;
            }
            if (result.errcode != ReturnCode.请求成功)
            {
                return ;
            }
            try
            {
                log.Info("openid++++++++++++++++++++++++++++++++++++++++" + result.openid);
                var user = context.WeiXinUser.FirstOrDefault(m => m.openid == result.openid);
                if (user == null)
                {
                    OAuthUserInfo userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);
                    user = new WeiXinUserModel()
                    {
                        city = userInfo.city,
                        headimgurl = userInfo.headimgurl,
                        country = userInfo.country,
                        nickname = userInfo.nickname,
                        openid = userInfo.openid,
                        province = userInfo.province,
                        sex = userInfo.sex,
                        time = DateTime.Now,
                        userInfoList = new List<WeiXinUserInfoModel>() { new WeiXinUserInfoModel() {
                                logTime=DateTime.Now,
                                remarks="请求/Article/OAuth"
                            } },
                        unionid = userInfo.unionid == null ? "" : userInfo.unionid
                    };
                    context.WeiXinUser.Add(user);
                    context.SaveChanges();
                    user = context.WeiXinUser.FirstOrDefault(p => p.openid == user.openid);
                }
                HttpContext.Session.SetString("uid", user.ID.ToString());
            }
            catch (ErrorJsonResultException ex)
            {
                return;
            }
            
        }
        public ActionResult OAuth(string url)
        {
            return Redirect(OAuthApi.GetAuthorizeUrl(appId,
              "http://www.nbug.xin/"+ url,
              "", OAuthScope.snsapi_userinfo));
        }
    }
}
