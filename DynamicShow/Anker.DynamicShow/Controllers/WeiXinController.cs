using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Anker.DynamicShow.Controllers
{
    using Anker.DynamicShow.Helper;
    using Senparc.Weixin.MP;
    using Senparc.Weixin.MP.Entities.Request;
    using Senparc.Weixin.MP.MvcExtension;
    using System.IO;

    [Produces("application/json")]
    [Route("api/WeiXin")]
    public class WeiXinController : Controller
    {
        public readonly string Token = "Anker";//与微信公众账号后台的Token设置保持一致，区分大小写。
        public static readonly string EncodingAESKey = "bQc89hLhPYtlSYv9zyAwz3147KEgPpvuXZIsexJ6lMd";//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string AppId = "wx4673f5bfc87da557";//与微信公众账号后台的AppId设置保持一致，区分大小写。

        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(string signature, string timestamp, string nonce, string echostr)
        {
            if (CheckSignature.Check(signature, timestamp, nonce, Token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + signature + "," + Senparc.Weixin.MP.CheckSignature.GetSignature(timestamp, nonce, Token) + "。如果您在浏览器中看到这条信息，表明此Url可以填入微信后台。");
            }
        }
        /// <summary>
        /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        /// PS：此方法为简化方法，效果与OldPost一致。
        /// v0.8之后的版本可以结合Senparc.Weixin.MP.MvcExtension扩展包，使用WeixinResult，见MiniPost方法。
        /// </summary>
        [HttpPost]
        [ActionName("Index")]
        public ActionResult Post(PostModel postModel)
        {
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content("参数错误！");
            }
            postModel.Token = Token;//根据自己后台的设置保持一致
            postModel.EncodingAESKey = EncodingAESKey;//根据自己后台的设置保持一致
            postModel.AppId = AppId;//根据自己后台的设置保持一致
            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var memoryStream = new MemoryStream();
            Request.Body.CopyTo(memoryStream);
            var messageHandler = new CustomMessageHandler(memoryStream, postModel);//接收消息
            messageHandler.Execute();//执行微信处理过程
            return new FixWeixinBugWeixinResult(messageHandler);//返回结果

        }
    }
}