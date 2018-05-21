using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.Entities;
using log4net;
using Microsoft.Extensions.Options;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP;
using System.IO;
using System.Text;
using Anker.Weixin.MP.CoreDynamicShow.CommonService.MessageHandlers.CustomMessageHandler;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.Containers;
using Anker.WeiXin.MP.CoreDynamicShow.CommonService.Utilities;

namespace Anker.WeiXin.MP.CoreDynamicShow.Controllers
{
    public class WeiXinController : Controller
    {
        private string appId;
        private string appSecret;
        private string token;
        private string encodingAESKey;
        SenparcWeixinSetting _senparcWeixinSetting;
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(WeiXinController));
        public WeiXinController(IOptions<SenparcWeixinSetting> senparcWeixinSetting)
        {
            _senparcWeixinSetting = senparcWeixinSetting.Value;
            appId = _senparcWeixinSetting.WeixinAppId;
            appSecret = _senparcWeixinSetting.WeixinAppSecret;
            token = _senparcWeixinSetting.Token;
            encodingAESKey = _senparcWeixinSetting.EncodingAESKey;
        }
        /// <summary>
        /// 微信后台验证地址（使用Get），微信后台的“接口配置信息”的Url填写如：http://sdk.weixin.senparc.com/weixin
        /// </summary>
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(PostModel postModel, string echostr)
        {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + postModel.Signature + "," + CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, token) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
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
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, token))
            {
                return Content("参数错误！");
            }
            #region 打包 PostModel 信息

            postModel.Token = token;//根据自己后台的设置保持一致
            postModel.EncodingAESKey = encodingAESKey;//根据自己后台的设置保持一致
            postModel.AppId = appId;//根据自己后台的设置保持一致

            #endregion
            //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
            var maxRecordCount = 10;
            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            string body = new StreamReader(Request.Body).ReadToEnd();
            byte[] requestData = Encoding.UTF8.GetBytes(body);
            Stream inputStream = new MemoryStream(requestData);
            var messageHandler = new CustomMessageHandler(inputStream, postModel, maxRecordCount);
            /* 如果需要添加消息去重功能，只需打开OmitRepeatedMessage功能，SDK会自动处理。
                 * 收到重复消息通常是因为微信服务器没有及时收到响应，会持续发送2-5条不等的相同内容的RequestMessage*/
            messageHandler.OmitRepeatedMessage = true;
            //执行微信处理过程
            messageHandler.Execute();
            //日志记录
            log.Info(string.Format("日志记录:----FromUserName:{0} APPID:{1}", messageHandler.ResponseMessage.FromUserName, "wx4673f5bfc87da557"));
            return new FixWeixinBugWeixinResult(messageHandler);//为了解决官方微信5.0软件换行bug暂时添加的方法，平时用下面一个方法即可
        }
        [HttpGet]
        [ActionName("DemoHtml")]
        public IActionResult DemoHtml()
        {
            var Models=GetJsSdkUiPackage(appId, appSecret, "www.nbug.xin");
            return View(Models);
        }
        /// <summary>
        /// 获取给UI使用的JSSDK信息包
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public  JsSdkUiPackage GetJsSdkUiPackage(string appId, string appSecret, string url)
        {
            //获取时间戳
            var timestamp = JSSDKHelper.GetTimestamp();
            //获取随机码
            string nonceStr = JSSDKHelper.GetNoncestr();
            string ticket = JsApiTicketContainer.TryGetJsApiTicket(appId, appSecret);
            //获取签名
            string signature = JSSDKHelper.GetSignature(ticket, nonceStr, timestamp, url);
            //返回信息包
            return new JsSdkUiPackage(appId, timestamp, nonceStr, signature);
        }
    }
}