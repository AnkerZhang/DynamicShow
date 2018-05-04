using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using log4net.Repository;
using log4net;
using Microsoft.AspNetCore.Http;
using Anker.DynamicShow.Helper;
using System.IO;
using System.Xml;
using Anker.DynamicShow.Model;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MvcExtension;

namespace Anker.DynamicShow.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(ValuesController));
        // GET api/values
        /// <summary>
        /// 微信服务器认证
        /// </summary>
        /// <param name="signature">微信加密签名，signature结合了开发者填写的token参数和请求中的timestamp参数、nonce参数</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonce">随机数</param>
        /// <param name="echostr">随机字符串</param>
        /// <returns></returns>
        [HttpGet,HttpPost]
        public async Task ProcessRequest(PostModel postModel)
        {
            if (Request.Method.ToLower() == "post")
            {
                //回复消息的时候也需要验证消息，这个很多开发者没有注意这个，存在安全隐患  
                //微信中 谁都可以获取信息 所以 关系不大 对于普通用户 但是对于某些涉及到验证信息的开发非常有必要
                if (CheckSignature())
                {
                    log.Info("接收消息");
                    //接收消息
                    //ReceiveXml();
                    // 获取request的响应
                    var memoryStream = new MemoryStream();
                    Request.Body.CopyTo(memoryStream);

                    var messageHandler = new CustomMessageHandler(memoryStream, postModel);//接收消息  
                    messageHandler.Execute();//执行微信处理过程  

                    new FixWeixinBugWeixinResult(messageHandler);//返回结果、、  
                }
                else
                {
                   await Response.WriteAsync("消息并非来自微信");
                }
            }
            else
            {
               await CheckWechat();
            }
        }
        /// <summary>
        /// 返回随机数表示验证成功
        /// </summary>
        private async Task CheckWechat()
        {
            if (string.IsNullOrEmpty(Request.Query["echoStr"]))
            {
                await Response.WriteAsync("消息并非来自微信");
                return;
            }
            string echoStr = Request.Query["echoStr"];
            if (CheckSignature())
            {
                await Response.WriteAsync(echoStr);
                return;
            }
            await Response.WriteAsync("验证签名错误");
            return;
        }
        /// <summary>
        /// 验证微信签名
        /// </summary>
        /// <returns></returns>
        /// * 将token、timestamp、nonce三个参数进行字典序排序
        /// * 将三个参数字符串拼接成一个字符串进行sha1加密
        /// * 开发者获得加密后的字符串可与signature对比，标识该请求来源于微信。
        private bool CheckSignature()
        {
            string access_token = "Anker";
            string signature = Request.Query["signature"].ToString();
            string timestamp = Request.Query["timestamp"].ToString();
            string nonce = Request.Query["nonce"].ToString();
            string[] ArrTmp = { access_token, timestamp, nonce };
            Array.Sort(ArrTmp);     //字典排序
            string tmpStr = string.Join("", ArrTmp);
            tmpStr = ApiHelper.SHA1(tmpStr);
            if (tmpStr.ToLower() == signature)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #region 接收消息
        /// <summary>
        /// 接收微信发送的XML消息并且解析
        /// </summary>
        private void ReceiveXml()
        {
            
            var requestStream = new MemoryStream();
            //messageHandler.Execute();//执行微信处理过程  

            //Request.Body.CopyTo(requestStream);
            if (requestStream == null) return;
            byte[] requestByte = new byte[requestStream.Length];
            requestStream.Read(requestByte, 0, (int)requestStream.Length);
            string requestStr = Encoding.UTF8.GetString(requestByte);
            if (!string.IsNullOrEmpty(requestStr))
            {
                //封装请求类
                XmlDocument requestDocXml = new XmlDocument();
                requestDocXml.LoadXml(requestStr);
                XmlElement rootElement = requestDocXml.DocumentElement;
                WxXmlModel WxXmlModel = new WxXmlModel();
                WxXmlModel.ToUserName = rootElement.SelectSingleNode("ToUserName").InnerText;
                WxXmlModel.FromUserName = rootElement.SelectSingleNode("FromUserName").InnerText;
                WxXmlModel.CreateTime = rootElement.SelectSingleNode("CreateTime").InnerText;
                WxXmlModel.MsgType = rootElement.SelectSingleNode("MsgType").InnerText;
                log.Info("接收消息"+ WxXmlModel.MsgType);
                switch (WxXmlModel.MsgType)
                {
                    case "text"://文本
                        WxXmlModel.Content = rootElement.SelectSingleNode("Content").InnerText;
                        break;
                    case "image"://图片
                        WxXmlModel.PicUrl = rootElement.SelectSingleNode("PicUrl").InnerText;
                        break;
                    case "event"://事件
                        WxXmlModel.Event = rootElement.SelectSingleNode("Event").InnerText;
                        if (WxXmlModel.Event != "TEMPLATESENDJOBFINISH")//关注类型
                        {
                            WxXmlModel.EventKey = rootElement.SelectSingleNode("EventKey").InnerText;
                        }
                        break;
                    default:
                        break;
                }

                ResponseXML(WxXmlModel);//回复消息
            }
        }
        #endregion
        #region 回复消息
        private void ResponseXML(WxXmlModel WxXmlModel)
        {
            //QrCodeApi QrCodeApi = new QrCodeApi();
            string XML = "";
            switch (WxXmlModel.MsgType)
            {
                case "text"://文本回复
                    XML = ResponseMessage.GetText(WxXmlModel.FromUserName, WxXmlModel.ToUserName, WxXmlModel.Content);
                    break;
                case "event":
                    switch (WxXmlModel.Event)
                    {
                        case "subscribe":
                            if (string.IsNullOrEmpty(WxXmlModel.EventKey))
                            {
                                XML = ResponseMessage.GetText(WxXmlModel.FromUserName, WxXmlModel.ToUserName, "关注成功");
                            }
                            else
                            {
                                XML = ResponseMessage.SubScanQrcode(WxXmlModel.FromUserName, WxXmlModel.ToUserName, WxXmlModel.EventKey);//扫描带参数二维码先关注后推送事件
                            }
                            break;
                        case "SCAN":
                            XML = ResponseMessage.ScanQrcode(WxXmlModel.FromUserName, WxXmlModel.ToUserName, WxXmlModel.EventKey);//扫描带参数二维码已关注 直接推送事件
                            break;
                    }
                    break;
                default://默认回复
                    break;
            }
            Response.WriteAsync(XML);
        }
        #endregion
    }
}
