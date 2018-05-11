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
    public class RootController : BaseController
    {
        
        public RootController(DynamicShowContext context, IOptions<SenparcWeixinSetting> senparcWeixinSetting)
        {
            _context = context;
            _senparcWeixinSetting = senparcWeixinSetting.Value;
            appId = _senparcWeixinSetting.WeixinAppId;
            appSecret = _senparcWeixinSetting.WeixinAppSecret;
            token = _senparcWeixinSetting.Token;
            encodingAESKey = _senparcWeixinSetting.EncodingAESKey;

        }
        public async Task<IActionResult> Index(int ID)
        {
            if (uid == 0) return Content("Session 错误");
            var user = await _context.WeiXinUser.FirstOrDefaultAsync(u => u.ID == Convert.ToInt32(uid));
            var artlist = await _context.WeiXinArticle.Include(p => p.articleInfoList).FirstOrDefaultAsync(w => w.ID == ID && w.userID == user);
            if (artlist == null)
            {
                return Content("非法操作");
            }
            if (artlist.articleInfoList.Count <= 0)
            {
                return Content("无用户记录");
            }
            ViewBag.user = user;
            ViewBag.artlist = artlist;
            return View(artlist.articleInfoList);
        
            //var user = await _context.WeiXinUser.FirstOrDefaultAsync(f => f.ID == Convert.ToInt32(uid));
            //if (user == null)
            //    return Content("找不到用户");
            //var article = await _context.WeiXinArticle.FirstOrDefaultAsync(c => c.userInfo == user);
            //if (article == null)
            //    return Content("找不到用户文章");
            //var articleInfoList = await _context.WeiXinArticleInfo.Include(c => c.userInfoID).Include(c => c.articleInfoLogModel).Where(c => c.articleID == article.ID).OrderByDescending(c => c.spendingDate).ToListAsync();

            //return View(articleInfoList);
        }
    }
}