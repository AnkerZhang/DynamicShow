using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Anker.WeiXin.MP.CoreDynamicShow.Models;
using Anker.WeiXin.MP.CoreDynamicShow.Data;
using Anker.WeiXin.MP.CoreDynamicShow.CommonService.MyWebsite.HandlersServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Anker.WeiXin.MP.CoreDynamicShow.CommonService.MyWebsite;
using System.IO;
using Senparc.Weixin.Entities;
using log4net;
using Microsoft.Extensions.Options;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP;
using Microsoft.AspNetCore.Http;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin;
using Anker.WeiXin.MP.CoreDynamicShow.CommonService.Utilities;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.Exceptions;
using Newtonsoft.Json;

namespace Anker.WeiXin.MP.CoreDynamicShow.Controllers
{
    public class HomeController : Controller
    {
        private string appId;
        private string appSecret;
        private string token;
        private string encodingAESKey;
        SenparcWeixinSetting _senparcWeixinSetting;
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(WeixinJSSDKController));
        private readonly DynamicShowContext _context;
        private IHostingEnvironment _host = null;
        public HomeController(DynamicShowContext context, IHostingEnvironment host, IOptions<SenparcWeixinSetting> senparcWeixinSetting)
        {
            _host = host;
            _context = context;
            _senparcWeixinSetting = senparcWeixinSetting.Value;
            appId = _senparcWeixinSetting.WeixinAppId;
            appSecret = _senparcWeixinSetting.WeixinAppSecret;
            token = _senparcWeixinSetting.Token;
            encodingAESKey = _senparcWeixinSetting.EncodingAESKey;
        }
        public ActionResult OAuth()
        {
            HttpContext.Session.SetString("AID", "1");
            return Redirect(OAuthApi.GetAuthorizeUrl(appId,
              "http://www.nbug.xin/Home/Index?returnUrl="+"".UrlEncode(),
              "", OAuthScope.snsapi_userinfo));
        }
        public async Task<IActionResult> Index(string code, string state)
        {
            var url = Server.GetAbsoluteUri(HttpContext.Request);
            ViewBag.url = url;
            string openid= HttpContext.Session.GetString("openid");
            HttpContext.Session.SetString("AID", "1");
            WeiXinUserInfo weiXinUserInfo = null;
            if (string.IsNullOrEmpty(code))
            {
                if (string.IsNullOrEmpty(openid))
                {
                    return Redirect("/Home/OAuth");
                }
                var user= await _context.WeiXinUserInfo.FirstOrDefaultAsync(m => m.openid == openid);
                if (user==null)
                {
                    return Redirect("/Home/OAuth");
                }
                else {
                    weiXinUserInfo = user;
                }
            }
            else
            {
                OAuthAccessTokenResult result = null;
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
                try
                {
                    var user = await _context.WeiXinUserInfo.FirstOrDefaultAsync(m => m.openid == result.openid);
                    if (user!=null)
                    {
                        weiXinUserInfo = user;
                    }
                    else
                    {
                        OAuthUserInfo userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);
                        weiXinUserInfo = new WeiXinUserInfo()
                        {
                            city = userInfo.city,
                            headimgurl = userInfo.headimgurl,
                            country = userInfo.country,
                            nickname = userInfo.nickname,
                            openid = userInfo.openid,
                            province = userInfo.province,
                            sex = userInfo.sex,
                            unionid = userInfo.unionid == null ? "" : userInfo.unionid
                        };
                        await _context.WeiXinUserInfo.AddAsync(weiXinUserInfo);
                    }
                   
                        
                }
                catch (ErrorJsonResultException ex)
                {
                    return Content(ex.Message);
                }
            }
            HttpContext.Session.SetString("openid", weiXinUserInfo.openid);
            ViewBag.t1 = await GetConfig(1, "t");
            ViewBag.c1 = await GetConfig(1, "c");
            ViewBag.t2 = await GetConfig(2, "t");
            ViewBag.c2 = await GetConfig(2, "c");
            ViewBag.t3 = await GetConfig(3, "t");
            ViewBag.c3 = await GetConfig(3, "c");
            ViewBag.t4 = await GetConfig(4, "t");
            ViewBag.c4 = await GetConfig(4, "c");
            ViewBag.t5 = await GetConfig(5, "t");
            ViewBag.c5 = await GetConfig(5, "c");
            ViewBag.t6 = await GetConfig(6, "t");
            ViewBag.c6 = await GetConfig(6, "c");
            string num = await GetConfig(7, "c");
            _context.Update(new ConfigModel { ID = 7, Titlt = "number", Content = (Convert.ToInt32(num) + 1).ToString() });
            await _context.SaveChangesAsync();
            ViewBag.images = weiXinUserInfo.headimgurl;
            ViewBag.name = weiXinUserInfo.nickname;
            ViewBag.c7 = num;
            ViewBag.c10 = await GetConfig(10, "c");
            var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(appId, appSecret, url);
            return View(jssdkUiPackage);
        }
        public async Task<IActionResult> Board(int page = 1)
        {
            var url = Server.GetAbsoluteUri(HttpContext.Request);
            ViewBag.url = url;
            string openid = HttpContext.Session.GetString("openid");
            if (string.IsNullOrEmpty(openid))
            {
                return Redirect("/Home/OAuth");
            }
            WeiXinUserInfo weiXinUserInfo = null;
            weiXinUserInfo = await _context.WeiXinUserInfo.Where(u => u.openid == openid).FirstAsync();
            if (weiXinUserInfo == null)
            {
                return Redirect("/Home/OAuth");
            }
            var user = await _context.Users
                .Include(p => p.Meaages)//包含导航属性Meaages表
                .Where(m => m.State == 1)
                .OrderByDescending(u => u.ID)
                .Skip(10 * (page - 1))
                .Take(10)
                 .AsNoTracking()//优化只进行查询不修改
                .ToListAsync();
            ViewBag.Page = page;
            int count = await _context.Users
                .Where(m => m.State == 1).CountAsync();
            ViewBag.count = (count / 10) + (count % 10 > 0 ? 1 : 0);
            ViewBag.total = count;
            ViewBag.t8 = await GetConfig(8, "t");
            ViewBag.c8 = await GetConfig(8, "c");
            ViewBag.t9 = await GetConfig(9, "t");
            ViewBag.c9 = await GetConfig(9, "c");
            ViewBag.images = weiXinUserInfo.headimgurl;
            ViewBag.name = weiXinUserInfo.nickname;
            ViewBag.ListUser = user;
            var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(appId, appSecret, url);
            return View(jssdkUiPackage);
        }
        public async Task<IActionResult> Resume()
        {
            var url = Server.GetAbsoluteUri(HttpContext.Request);
            ViewBag.url = url;
            ViewBag.t8 = await GetConfig(8, "t");
            ViewBag.c8 = await GetConfig(8, "c");
            ViewBag.t9 = await GetConfig(9, "t");
            ViewBag.c9 = await GetConfig(9, "c");
            ViewBag.t11 = await GetConfig(11, "t");
            ViewBag.c11 = await GetConfig(11, "c");
            var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(appId, appSecret, url);
            return View(jssdkUiPackage);
        }
        public async Task<IActionResult> Changelogs()
        {
            ViewBag.t8 = await GetConfig(8, "t");
            ViewBag.c8 = await GetConfig(8, "c");
            ViewBag.t9 = await GetConfig(9, "t");
            ViewBag.c9 = await GetConfig(9, "c");
            return View();

        }
        private async Task<string> GetConfig(int id, string title)
        {
            var config = _context.ConfigModel
                .Where(w => w.ID == id)
                .Select(s => title == "t" ? s.Titlt : s.Content).FirstAsync();
            return await config;

        }
        public ActionResult GetAddress()
        {
            string ip = HttpContext.GetUserIp();
            string address = IpHelper.GetAddress(ip);
            return Json(address.Split('|'));
        }
        public async Task<ActionResult> GetTopMeage()
        {
            var mage = await _context.Users
                 .Where(m => m.State == 1)
                 .OrderByDescending(o => o.ID)
                 .Take(5)
                 .Select(s => new { s.Image, s.Content, s.NickName, mages = IpHelper.GetAddressInfo(s.address, s.Time) })
                 .ToListAsync();
            return Json(mage);
        }
        public async Task<JsonResult> Send(SendDataModel data)
        {
            if (data.author == null || data.content == null )
            {
                return new JsonResult(new { isSuccess = false, returnMsg = "错误" });
            }
            if (data.images == null)
                data.images = "/Images/default.png";
            if (data.url != "" && data.url != null)
            {
                data.url = data.url.Contains("http") ? data.url : "http://" + data.url;
            }
            string ip = HttpContext.GetUserIp();
            var arr = IpHelper.GetAddress(ip).Split('|');
            if (data.comment_parent != null && data.comment_parent != "" && data.comment_parent != "0")
            {
                var message = new Message()
                {
                    UserID = Convert.ToInt32(data.comment_parent),
                    NickName = data.author,
                    Content = data.content,
                    IP = ip,
                    country = arr[0],
                    province = arr[1],
                    city = arr[2],
                    address = arr[3],
                    Email = data.email,
                    Website = data.url,
                    Image = data.images,
                    Time = DateTime.Now,
                    State = 1
                };
                await _context.Messages.AddAsync(message);
            }
            else
            {
                var user = new User()
                {
                    NickName = data.author,
                    Content = data.content,
                    IP = ip,
                    country = arr[0],
                    province = arr[1],
                    city = arr[2],
                    address = arr[3],
                    Email = data.email==null?"": data.email,
                    Website = data.url,
                    Image = data.images,
                    Time = DateTime.Now,
                    State = 1
                };
                await _context.Users.AddAsync(user);
            }
            await _context.SaveChangesAsync();
            return new JsonResult(new { isSuccess = true, returnMsg = "提交成功" });
        }
        public JsonResultModel InsertPicture()
        {
            var path = _host.WebRootPath;
            var uploadfile = Request.Form.Files["file"];
            var filePath = string.Format("/Uploads/Images/");
            if (!Directory.Exists(path + filePath))
            {
                Directory.CreateDirectory(path + filePath);
            }
            if (uploadfile != null)
            {
                //文件后缀
                var fileExtension = Path.GetExtension(uploadfile.FileName);
                // 判断后缀是否是图片
                const string fileFilt = ".gif|.jpg|.php|.jsp|.jpeg|.png|......";
                if (fileExtension == null)
                {
                    return new JsonResultModel { isSucceed = false, resultMsg = "上传的文件没有后缀" };
                }
                if (fileFilt.IndexOf(fileExtension.ToLower(), StringComparison.Ordinal) <= -1)
                {
                    return new JsonResultModel { isSucceed = false, resultMsg = "上传的文件不是图片" };
                }
                //判断文件大小    
                long length = uploadfile.Length;
                if (length > 1024 * 1024 * 2) //2M
                {
                    return new JsonResultModel { isSucceed = false, resultMsg = "上传的文件不能大于2M" };
                }
                var strDateTime = DateTime.Now.ToString("yyMMddhhmmssfff"); //取得时间字符串
                var strRan = Convert.ToString(new Random().Next(100, 999)); //生成三位随机数
                var saveName = strDateTime + strRan + fileExtension;
                //插入图片数据
                using (FileStream fs = System.IO.File.Create(path + filePath + saveName))
                {
                    uploadfile.CopyTo(fs);
                    fs.Flush();
                }
                return new JsonResultModel { isSucceed = true, resultMsg = filePath + saveName };
            }
            else
            {
                return new JsonResultModel { isSucceed = true, resultMsg = "/Images/default.png" };
            }

        }
        public async Task<ActionResult> Evaluate(string action, int id)
        {
            var ip = HttpContext.GetUserIp();
            var u = await _context.Users.Where(w => w.ID == id).FirstAsync();
            if (action == "dislike")
            {
                u.ticket = u.ticket - 1;
            }
            else
            {
                u.ticket = u.ticket + 1;
            }
            _context.Update(u);
            await _context.SaveChangesAsync();
            return Content(u.ticket.ToString());
            //}
        }

        public void ArticleApi(string type)
        {
            var aid= HttpContext.Session.GetString("AID");
            var openid = HttpContext.Session.GetString("openid");
            var user = _context.WeiXinUserInfo.FirstOrDefault(f => f.openid == openid);
            var article=_context.WeiXinArticle.FirstOrDefault(f => f.ID ==Convert.ToInt32(aid));
            var articleInfo =_context.WeiXinArticleInfo.FirstOrDefault(f => f.AID ==article&&f.UID== user);
            if (articleInfo == null)
            {

                articleInfo = new WeiXinArticleInfo()
                {
                    AID = article,
                    UID = user,
                    ClockStart = DateTime.Now,
                    Amount = 1,
                    OpentNumber = 1,
                    ClockEnd = DateTime.Now,
                    SpendingDate = 10 + 1
                };
                _context.WeiXinArticleInfo.AddAsync(articleInfo);
                if (_context.WeiXinArticleInfo.FirstOrDefault(f => f.AID == article && f.UID == user) == null)
                {
                    _context.SaveChanges();
                }
            }
            else
            {
                switch (type)
                {
                    case "open":
                        articleInfo.OpentNumber = articleInfo.OpentNumber + 1;
                        articleInfo.ClockEnd = DateTime.Now;
                        articleInfo.SpendingDate = articleInfo.SpendingDate + 10;
                        break;
                    case "s":
                        articleInfo.Amount = articleInfo.Amount + 2;
                        articleInfo.ClockEnd = DateTime.Now;
                        articleInfo.SpendingDate = articleInfo.SpendingDate + 2;
                        break;
                    default:

                        break;
                }
                _context.Update(articleInfo);
                _context.SaveChanges();
            }
        }

    }
}
