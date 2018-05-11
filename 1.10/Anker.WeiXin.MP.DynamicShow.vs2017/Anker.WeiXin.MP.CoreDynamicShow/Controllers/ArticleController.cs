using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Anker.WeiXin.MP.CoreDynamicShow.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Senparc.Weixin.Entities;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Anker.WeiXin.MP.CoreDynamicShow.Models;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;

namespace Anker.WeiXin.MP.CoreDynamicShow.Controllers
{
    public class ArticleController : BaseController
    {
        public ArticleController(DynamicShowContext context, IHostingEnvironment host, IOptions<SenparcWeixinSetting> senparcWeixinSetting, IHttpContextAccessor accessor)
        {
            _host = host;
            _context = context;
            _senparcWeixinSetting = senparcWeixinSetting.Value;
            appId = _senparcWeixinSetting.WeixinAppId;
            appSecret = _senparcWeixinSetting.WeixinAppSecret;
            token = _senparcWeixinSetting.Token;
            encodingAESKey = _senparcWeixinSetting.EncodingAESKey;
            HttpContext = accessor.HttpContext;
            log = LogManager.GetLogger(Startup.repository.Name, typeof(ArticleController));

            uid = Convert.ToInt32(HttpContext.Session.GetString("uid") == null ? "0" : HttpContext.Session.GetString("uid"));
        }
        public ActionResult OAuth()
        {
            return Redirect(OAuthApi.GetAuthorizeUrl(appId,
              "http://www.nbug.xin/Article/MyArticle?returnUrl=" + "".UrlEncode(),
              "", OAuthScope.snsapi_userinfo));
        }
        public async Task<IActionResult> MyArticle(string code, string state)
        {
            WeiXinUserModel user = null;
            if (string.IsNullOrEmpty(code))
            {
                if (uid == 0)
                {
                    return Redirect("/Article/OAuth");
                }

            }
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
            try
            {
                user = await _context.WeiXinUser.FirstOrDefaultAsync(m => m.openid == result.openid);
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
                    await _context.WeiXinUser.AddAsync(user);
                    await _context.SaveChangesAsync();
                    user = await _context.WeiXinUser.FirstOrDefaultAsync(p => p.openid == user.openid);
                }
            }
            catch (ErrorJsonResultException ex)
            {
                return Content(ex.Message);
            }
            HttpContext.Session.SetString("uid", user.ID.ToString());
            user =await _context.WeiXinUser.FirstOrDefaultAsync(u => u.ID == Convert.ToInt32(uid));
            var artlist=await _context.WeiXinArticle.Where(w => w.userID==user).ToListAsync();
            ViewBag.user = user;
            return View(artlist);
        }
        public async Task<IActionResult> updata(int aid)
        {
            if (uid == 0) return Content("Session 错误");
            var art = await _context.WeiXinArticle.Include(i => i.userID).FirstOrDefaultAsync(f => (f.ID == aid&&f.userID.ID==uid));

            var user = await _context.WeiXinUser.FirstOrDefaultAsync(u => u.ID == Convert.ToInt32(uid));
            ViewBag.user = user;
            return View(art);
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            if (uid == 0) return Content("Session 错误");
            var user = await _context.WeiXinUser.FirstOrDefaultAsync(u => u.ID == Convert.ToInt32(uid));
            ViewBag.user = user;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddData(FromDataModel fromData)
        {
            var commentInfo = InsertPicture(fromData);
            var user =await _context.WeiXinUser.FirstOrDefaultAsync(f => f.ID == uid);
            if (user == null) return Content("Session 错误");
            StringBuilder sb = new StringBuilder();
            foreach (var item in commentInfo)
            {
                if (item.str == "titleImg") continue;
                sb.AppendFormat("<p> <img src='{0}' /></p>",item.fileName);
                sb.AppendFormat("<p style='letter - spacing: 1px; padding - left: 0.5em; padding - right: 0.5em; '><span style='font - size: 15px; '>{0}</span></p>",item.str);
            }
            var date = DateTime.Now;
            var art = new WeiXinArticleModel()
            {
                author = fromData.zuozhe,
                Music = "/music/" + fromData.music + ".mp3",
                state = 1,
                qrCode = md5(date.ToString()),
                time = date,
                title = fromData.title,
                titleImg = commentInfo[0].fileName,
                userID = user,
                content = sb.ToString()
            };
            await _context.WeiXinArticle.AddAsync(art);
            await _context.SaveChangesAsync();
            return new JsonResult(new { isSuccess = true, returnMsg = art.qrCode });
        }
        private List<CommentInfo> InsertPicture(FromDataModel fromData)
        {
            var str = fromData.strlist[0].Split('^');
            List<CommentInfo> list = new List<CommentInfo>();
            list.Add(new CommentInfo() { str = "titleImg", fileName = saveImg(fromData.xiaofile) });
            if (fromData.tu1file != null)
                list.Add(new CommentInfo() { str = str[0], fileName = saveImg(fromData.tu1file) });
            if (fromData.tu2file != null)
                list.Add(new CommentInfo() { str = str[1], fileName = saveImg(fromData.tu2file) });
            if (fromData.tu3file != null)
                list.Add(new CommentInfo() { str = str[2], fileName = saveImg(fromData.tu3file) });
            if (fromData.tu4file != null)
                list.Add(new CommentInfo() { str = str[3], fileName = saveImg(fromData.tu4file) });
            if (fromData.tu5file != null)
                list.Add(new CommentInfo() { str = str[4], fileName = saveImg(fromData.tu5file) });
            if (fromData.tu6file != null)
                list.Add(new CommentInfo() { str = str[5], fileName = saveImg(fromData.tu6file) });
            if (fromData.tu7file != null)
                list.Add(new CommentInfo() { str = str[6], fileName = saveImg(fromData.tu7file) });
            if (fromData.tu8file != null)
                list.Add(new CommentInfo() { str = str[7], fileName = saveImg(fromData.tu8file) });
            if (fromData.tu9file != null)
                list.Add(new CommentInfo() { str = str[8], fileName = saveImg(fromData.tu9file) });
            if (fromData.tu10file != null)
                list.Add(new CommentInfo() { str = str[9], fileName = saveImg(fromData.tu10file) });
            return list;
        }

        private string saveImg(IFormFile uploadfile)
        {
            var path = _host.WebRootPath;
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
                    return null;
                }
                if (fileFilt.IndexOf(fileExtension.ToLower(), StringComparison.Ordinal) <= -1)
                {
                    return null;
                }
                //判断文件大小    
                //long length = uploadfile.Length;
                //if (length > 1024 * 1024 * 2) //2M
                //{
                //    return new JsonResultModel { isSucceed = false, resultMsg = "上传的文件不能大于2M" };
                //}
                var strDateTime = DateTime.Now.ToString("yyMMddhhmmssfff"); //取得时间字符串
                var strRan = Convert.ToString(new Random().Next(100, 999)); //生成三位随机数
                var saveName = strDateTime + strRan + fileExtension;
                //插入图片数据
                using (FileStream fs = System.IO.File.Create(path + filePath + saveName))
                {
                    uploadfile.CopyTo(fs);
                    fs.Flush();
                }
                return filePath + saveName;
            }
            return null;
        }

        private string md5(string date)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(date));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }
    }
}