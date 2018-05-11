using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.MP.Entities.Menu;
using Senparc.Weixin.MP.CommonAPIs;
using Microsoft.Extensions.Options;
using Senparc.Weixin.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP;
using Senparc.Weixin;
using Senparc.Weixin.HttpUtility;
using System.Text;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.Exceptions;
using Anker.WeiXin.MP.CoreDynamicShow.Data;
using Microsoft.AspNetCore.Hosting;
using log4net;
using Microsoft.AspNetCore.Http;

namespace Anker.WeiXin.MP.CoreDynamicShow.Controllers
{
   
    public class MenuController : BaseController
    {
       
        public MenuController(DynamicShowContext context, IHostingEnvironment host, IOptions<SenparcWeixinSetting> senparcWeixinSetting)
        {
            _host = host;
            _context = context;
            _senparcWeixinSetting = senparcWeixinSetting.Value;
            appId = _senparcWeixinSetting.WeixinAppId;
            appSecret = _senparcWeixinSetting.WeixinAppSecret;
            token = _senparcWeixinSetting.Token;
            encodingAESKey = _senparcWeixinSetting.EncodingAESKey;
            log = LogManager.GetLogger(Startup.repository.Name, typeof(MenuController));
            uid = Convert.ToInt32(HttpContext.Session.GetString("uid") == null ? "0" : HttpContext.Session.GetString("uid"));
        }
        public IActionResult CreateMenu()
        {
            ButtonGroup bg = new ButtonGroup();

            //二级菜单
            var subButton = new SingleViewButton()
            {
                name = "我要制作",
                url = "http://www.nbug.xin/Article/MyArticle",
            };
            bg.button.Add(subButton);
            var subButton3 = new SubButton()
            {
                name = "关于我",
            };
            bg.button.Add(subButton3);
            subButton3.sub_button.Add(new SingleViewButton()
            {
                url = "http://www.nbug.xin/5000",
                name = "Anker主页"
            });
            subButton3.sub_button.Add(new SingleViewButton()
            {
                url = "http://www.nbug.xin/5000/Home/Board",
                name = "反馈信息"
            });

            var result = CommonApi.CreateMenu(_senparcWeixinSetting.WeixinAppId, bg);
            return Json(result);
        }
        public IActionResult DeleteMenu()
        {
            string token = AccessTokenContainer.GetAccessToken(appId);
            try
            {
                var result = CommonApi.DeleteMenu(token);
                var json = new
                {
                    Success = result.errmsg == "ok",
                    Message = result.errmsg
                };
                return Json(json, new JsonSerializerSettings() { ContractResolver = new DefaultContractResolver() });
            }
            catch (Exception ex)
            {
                var json = new { Success = false, Message = ex.Message };
                return Json(json, new JsonSerializerSettings() { ContractResolver = new DefaultContractResolver() });
            }


        }
        public  IActionResult GetMenuTest()
        {
            var token = AccessTokenContainer.GetAccessToken(appId);
            var url = string.Format(Config.ApiMpHost + "/cgi-bin/menu/get?access_token={0}", token);
            var jsonString = RequestUtility.HttpGet(url, Encoding.UTF8);
            return Content(jsonString);


        }
    }
}