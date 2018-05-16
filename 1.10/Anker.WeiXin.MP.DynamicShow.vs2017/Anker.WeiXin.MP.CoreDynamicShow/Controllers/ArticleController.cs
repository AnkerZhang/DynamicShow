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
using System.DrawingCore;
//using ImageSharp;
//using System.DrawingCore;

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

            log.Info("/Article/MyArticle/++++++++++++++" + uid);
            WeiXinUserModel user = null;
            if (string.IsNullOrEmpty(code))
            {
                if (uid == 0)
                {
                    return Redirect("/Article/OAuth");
                }
                else
                {
                    user = await _context.WeiXinUser.FirstOrDefaultAsync(u => u.ID == Convert.ToInt32(uid));
                }
            }
            else
            {
                adduser(code, _context);
                uid = Convert.ToInt32(HttpContext.Session.GetString("uid"));
                user = await _context.WeiXinUser.FirstOrDefaultAsync(u => u.ID == Convert.ToInt32(uid));
            }
            var artlist = await _context.WeiXinArticle.Where(w => w.userID == user).ToListAsync();
            ViewBag.user = user;
            return View(artlist);
        }
        public async Task<IActionResult> updata(int aid)
        {
            if (uid == 0) return Content("Session 错误");
            var art = await _context.WeiXinArticle.Include(i => i.userID).FirstOrDefaultAsync(f => (f.ID == aid && f.userID.ID == uid));

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
            var user = await _context.WeiXinUser.FirstOrDefaultAsync(f => f.ID == uid);
            if (user == null) return Content("Session 错误");
            StringBuilder sb = new StringBuilder();
            if (fromData.zuozhe == null || fromData.zuozhe == "")
            {
                sb.Append("[");
                for (int i = 0; i < commentInfo.Count; i++)
                {
                    if (commentInfo[i].str == "titleImg") continue;
                    if (i > 1)
                    { sb.Append(",{"); }
                    else
                    {
                        sb.Append("{");
                    }
                    sb.AppendFormat(@"""name"":""{0}"",""caption"": ""{1}""", commentInfo[i].fileName, commentInfo[i].str);
                    sb.Append("}");
                }
                sb.Append("]");
            }
            else
            {
                foreach (var item in commentInfo)
                {
                    if (item.str == "titleImg") continue;
                    sb.AppendFormat("<p style='padding-left: 0.5em; padding-right: 0.5em; letter-spacing: 1px; line-height: 1.75em; '><span style='font-size: 13px; '>{0}</span></p>", item.str);
                    sb.AppendFormat("<p> <img src='{0}' /></p>", item.fileName);
                }
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
                contentTitle = fromData.contentTitle,
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
            list.Add(new CommentInfo() { str = "titleImg", fileName = saveImg(fromData.xiaofile, false) });
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
            if (fromData.tu11file != null)
                list.Add(new CommentInfo() { str = str[10], fileName = saveImg(fromData.tu11file) });
            if (fromData.tu12file != null)
                list.Add(new CommentInfo() { str = str[11], fileName = saveImg(fromData.tu12file) });
            if (fromData.tu13file != null)
                list.Add(new CommentInfo() { str = str[12], fileName = saveImg(fromData.tu13file) });
            if (fromData.tu14file != null)
                list.Add(new CommentInfo() { str = str[13], fileName = saveImg(fromData.tu14file) });
            if (fromData.tu15file != null)
                list.Add(new CommentInfo() { str = str[14], fileName = saveImg(fromData.tu15file) });
            return list;
        }

        private string saveImg(IFormFile uploadfile, bool b = true)
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
                const string fileFilt = ".gif|.jpg|.php|.jsp|.jpeg|.png|.heic|.|";
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
                var saveName = strDateTime + strRan + fileExtension.Split('.')[0] + ".jpg";
                //插入图片数据
                using (FileStream fs = System.IO.File.Create(path + filePath + saveName))
                {
                    uploadfile.CopyTo(fs);
                    fs.Flush();
                }
                try
                {
                    if (b)
                    {
                        update_picture(path + filePath, saveName, path + filePath + saveName);
                    }
                    else
                    {
                        update_picture(path + filePath, saveName, path + filePath + saveName, 100, 100);
                    }
                    return filePath +"C"+ saveName;
                }
                catch (Exception)
                {
                    return filePath + saveName;
                }
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
        /// <summary>  
        /// 修改指定图片的分辨率  
        /// </summary>  
        /// <param name="fileFoldUrl">文件夹url</param>  
        /// <param name="fileName">文件名</param>  
        /// <param name="filePath">文件路径，带文件名</param>  
        /// <param name="_width">分辨率的宽</param>  
        /// <param name="_height">分辨率的高</param>  
        public string update_picture(string fileFoldUrl, string fileName, string filePath, int _width = 480, int _height = 640)
        {
            byte[] zp = this.load_pictMemory(filePath);

            MemoryStream ms = new MemoryStream(zp);

            Image img = Image.FromStream(ms);

            if (img.Width > _width || img.Height > _height)
            {
                if (img.Width >= img.Height)
                {
                    _height = (int)Math.Floor(Convert.ToDouble(img.Height) * (Convert.ToDouble(_width) / Convert.ToDouble(img.Width)));
                }
                else
                {
                    _width = (int)Math.Floor(Convert.ToDouble(img.Width) * (Convert.ToDouble(_height) / Convert.ToDouble(img.Height)));
                }
            }
            else
            {
                _width = img.Width; //原图宽度
                _height = img.Height; //原图高度
            }

            Bitmap btp = new Bitmap(img, _width, _height);

            DirectoryInfo dti = new DirectoryInfo(fileFoldUrl);

            FileInfo[] fis = dti.GetFiles();

            string fileUrl = fileFoldUrl + "C"+fileName;

            btp.Save(fileUrl);
            return "C" + fileName;
        }
        /// <summary>  
        /// 获取指定文件流的字节大小  
        /// </summary>  
        /// <param name="filePath">文件路径</param>  
        /// <returns>byte[]</returns>  
        public byte[] load_pictMemory(string filePath)
        {
            byte[] pictData = null;

            FileInfo fi = new FileInfo(filePath);

            if (fi.Exists)
            {
                pictData = new byte[fi.Length];

                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);

                BinaryReader br = new BinaryReader(fs);

                br.Read(pictData, 0, Convert.ToInt32(fi.Length));

                fs.Dispose();
            }
            else
            {
                return null;
            }
            return pictData;
        }
    }
}