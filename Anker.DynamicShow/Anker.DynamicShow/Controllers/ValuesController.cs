using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP;
using System.IO;
using Anker.DynamicShow.Services;
using Senparc.Weixin.MP.MvcExtension;
using log4net;
using Senparc.Weixin.MP.Entities;
using Microsoft.AspNetCore.Http;

namespace Anker.DynamicShow.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        public readonly string Token = "Anker";//与微信公众账号后台的Token设置保持一致，区分大小写。
        public static readonly string EncodingAESKey = "bQc89hLhPYtlSYv9zyAwz3147KEgPpvuXZIsexJ6lMd";//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string AppId = "wx4673f5bfc87da557";//与微信公众账号后台的AppId设置保持一致，区分大小写。
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(ValuesController));
        // GET api/values
        [HttpGet, HttpPost]
        public string Get(PostModel postModel)
        {
            try
            {
                if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
                {
                    return "参数错误";
                }
                log.Info("认证通过");
                postModel.Token = Token;//根据自己后台的设置保持一致  
                postModel.EncodingAESKey = EncodingAESKey;//根据自己后台的设置保持一致  
                postModel.AppId = AppId;//根据自己后台的设置保持一致  
                                        //自定义MessageHandler，对微信请求的详细判断操作都在这里面。  
                                        //获取request的响应  
                var memoryStream = new MemoryStream();
                Request.Body.CopyTo(memoryStream);
                log.Info("memoryStream.Length" + memoryStream.Length);
                CustomMessageHandler messageHandler = new CustomMessageHandler(memoryStream, postModel);//接收消息  
                log.Info("messageHandler");
                messageHandler.Execute();//执行微信处理过程  
                log.Info(ResponseMessage.GetText(messageHandler.ResponseMessage.FromUserName, "wx4673f5bfc87da557", ((ResponseMessageText)messageHandler.ResponseMessage).Content));
                string xml= ResponseMessage.GetText(messageHandler.ResponseMessage.FromUserName, "wx4673f5bfc87da557", ((ResponseMessageText)messageHandler.ResponseMessage).Content);
                return xml;
            }
            catch (Exception ex)
            {
                log.Error("Exception" + ex.Message);
            }
            return "参数错误";
        }
    }
}
