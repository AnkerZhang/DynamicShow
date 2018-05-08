using Senparc.Weixin.MP.MessageHandlers;
using System;
using System.Collections.Generic;
using System.Text;
using Senparc.Weixin.MP.Entities;
using System.IO;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.Entities.Request;
using System.Threading;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.Helpers;
using Senparc.Weixin.MP;
using System.Threading.Tasks;
using Anker.WeiXin.MP.CoreDynamicShow.CommonService.Utilities;
using log4net;
using Anker.WeiXin.MP.CoreDynamicShow;

namespace Anker.Weixin.MP.CoreDynamicShow.CommonService.MessageHandlers.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public partial class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        private string appId = "wx4673f5bfc87da557";
        private string appSecret = "42f119bb1f2cf8e201362d938aa11c9b";
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(CustomMessageHandler));
        /*
         * 重要提示：v1.5起，MessageHandler提供了一个DefaultResponseMessage的抽象方法，
         * DefaultResponseMessage必须在子类中重写，用于返回没有处理过的消息类型（也可以用于默认消息，如帮助信息等）；
         * 其中所有原OnXX的抽象方法已经都改为虚方法，可以不必每个都重写。若不重写，默认返回DefaultResponseMessage方法中的结果。
         */
        public CustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0)
           : base(inputStream, postModel, maxRecordCount)
        {
            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalWeixinContext.ExpireMinutes = 3。
            WeixinContext.ExpireMinutes = 3;

            if (!string.IsNullOrEmpty(postModel.AppId))
            {
                appId = postModel.AppId;//通过第三方开放平台发送过来的请求
            }

            //在指定条件下，不使用消息去重
            base.OmitRepeatedMessageFunc = requestMessage =>
            {
                var textRequestMessage = requestMessage as RequestMessageText;
                if (textRequestMessage != null && textRequestMessage.Content == "容错")
                {
                    return false;
                }
                return true;
            };
        }
        public CustomMessageHandler(RequestMessageBase requestMessage)
            : base(requestMessage)
        {
        }
        /// <summary>
        /// 订阅（关注）事件
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_Submit_Membercard_User_InfoRequest(RequestMessageEvent_Submit_Membercard_User_Info requestMessage)
        {
            log.Info("触发了 欢迎关注动态秀！全新动态交互方式的奇妙之旅，即刻起航！[奸笑]+++++++++++");
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseMessage.Content = "欢迎关注动态秀！全新动态交互方式的奇妙之旅，即刻起航！[奸笑]";
            return responseMessage;
        }
        public override void OnExecuting()
        {
            //测试MessageContext.StorageData
            if (CurrentMessageContext.StorageData == null)
            {
                CurrentMessageContext.StorageData = 0;
            }
            base.OnExecuting();
        }
        public override void OnExecuted()
        {
            base.OnExecuted();
            CurrentMessageContext.StorageData = ((int)CurrentMessageContext.StorageData) + 1;
        }
        /// <summary>
        /// 处理文字请求
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            
            //说明：实际项目中这里的逻辑可以交给Service处理具体信息，参考OnLocationRequest方法或/Service/LocationSercice.cs
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            log.Info(string.Format("日志记录:----处理文字请求", responseMessage.Content));
            var requestHandler =
                requestMessage.StartHandler()
                 .Keyword("关键字", () =>
                 {
                     responseMessage.Content = WeiXin.MP.CoreDynamicShow.CommonService.MessageHandlers.CustomMessageHandler.Service.LocationSercice.GetText("关键字");
                     return responseMessage;
                 }).Keywords(new[] { "我", "你" }, () =>
                 {
                     responseMessage.Content = WeiXin.MP.CoreDynamicShow.CommonService.MessageHandlers.CustomMessageHandler.Service.LocationSercice.GetText("我");
                     return responseMessage;
                 }).Keywords(new[] { "测试" }, () =>
                 {
                     responseMessage.Content = WeiXin.MP.CoreDynamicShow.CommonService.MessageHandlers.CustomMessageHandler.Service.LocationSercice.GetText("测试");
                     return responseMessage;
                 }).Keyword("root", () =>
                 {
                     responseMessage.Content = "http://www.nbug.xin/Root/Index?openid="+ requestMessage.FromUserName;
                     return responseMessage;
                 }).Keyword("容错", () =>
                 {
                     Thread.Sleep(4900);//故意延时1.5秒，让微信多次发送消息过来，观察返回结果
                     var faultTolerantResponseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
                     faultTolerantResponseMessage.Content = string.Format("测试容错，MsgId：{0}，Ticks：{1}", requestMessage.MsgId,
                         DateTime.Now.Ticks);
                     return faultTolerantResponseMessage;
                 }).Keywords(new[] { "我的信息", "信息" }, () =>
                 {
                     var openId = requestMessage.FromUserName;//获取OpenId
                     var userInfo = UserApi.Info(appId, openId, Language.zh_CN);
                     responseMessage.Content = string.Format(
                         "您的OpenID为：{0}\r\n头像：{9}\r\n昵称：{1}\r\n性别：{2}\r\n地区（国家/省/市）：{3}/{4}/{5}\r\n关注时间：{6}\r\n关注状态：{7}\r\n您发送的消息是{8}",
                         requestMessage.FromUserName, userInfo.nickname, (Sex)userInfo.sex, userInfo.country, userInfo.province, userInfo.city, DateTimeHelper.GetDateTimeFromXml(userInfo.subscribe_time), userInfo.subscribe, requestMessage.Content,userInfo.headimgurl);
                     return responseMessage;
                 }).Keyword("MUTE", () => //不回复任何消息
                 {
                     //方案一：
                     return new SuccessResponseMessage();
                 }).Keyword("Anker", () =>
                 {
                     responseMessage.Content = "点击打开：http://www.nbug.xin/WeiXin/DemoHtml";
                     return responseMessage;
                 }).Default(() =>
                 {
                     var result = new StringBuilder();
                     result.AppendFormat("您刚才发送了文字信息：{0}\r\n\r\n", requestMessage.Content);
                     if (CurrentMessageContext.RequestMessages.Count > 1)
                     {
                         result.AppendFormat("您刚才还发送了如下消息（{0}/{1}）：\r\n", CurrentMessageContext.RequestMessages.Count,
                             CurrentMessageContext.StorageData);
                         for (int i = CurrentMessageContext.RequestMessages.Count - 2; i >= 0; i--)
                         {
                             var historyMessage = CurrentMessageContext.RequestMessages[i];
                             result.AppendFormat("{0} 【{1}】{2}\r\n",
                                 historyMessage.CreateTime.ToString("HH:mm:ss"),
                                 historyMessage.MsgType.ToString(),
                                 (historyMessage is RequestMessageText)
                                     ? (historyMessage as RequestMessageText).Content
                                     : "[非文字类型]"
                                 );
                         }
                         result.AppendLine("\r\n");
                     }
                     result.AppendFormat("如果您在{0}分钟内连续发送消息，记录将被自动保留（当前设置：最多记录{1}条）。过期后记录将会自动清除。\r\n",
                         WeixinContext.ExpireMinutes, WeixinContext.MaxRecordCount);
                     result.AppendLine("\r\n");
                 result.AppendLine(
                     "您还可以发送【位置】【图片】【语音】【视频】等类型的信息（注意是这几种类型，不是这几个文字），查看不同格式的回复。);");
                     responseMessage.Content = result.ToString();
                     return responseMessage;
                 });
            return requestHandler.GetResponseMessage() as IResponseMessageBase;
        }
        /// <summary>
        /// 处理位置请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        //public override IResponseMessageBase OnLocationRequest(RequestMessageLocation requestMessage)
        //{
        //    var locationService = new LocationService();
        //    var responseMessage = locationService.GetResponseMessage(requestMessage as RequestMessageLocation);
        //    return responseMessage;
        //}
        public override IResponseMessageBase OnShortVideoRequest(RequestMessageShortVideo requestMessage)
        {
            log.Info("日志记录:----您刚才发送的是小视频");
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您刚才发送的是小视频";
            return responseMessage;
        }
        /// <summary>
        /// 处理图片请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        {
            //一隔一返回News或Image格式
            if (base.WeixinContext.GetMessageContext(requestMessage).RequestMessages.Count % 2 == 0)
            {
                log.Info("日志记录:----处理多张图片请求");
                var responseMessage = CreateResponseMessage<ResponseMessageNews>();

                responseMessage.Articles.Add(new Article()
                {
                    Title = "您刚才发送了图片信息",
                    Description = "您发送的图片将会显示在边上",
                    PicUrl = requestMessage.PicUrl,
                    Url = "http://www.nbug.xin:5000/"
                });
                responseMessage.Articles.Add(new Article()
                {
                    Title = "第二条",
                    Description = "第二条带连接的内容",
                    PicUrl = requestMessage.PicUrl,
                    Url = "http://www.nbug.xin:5000/"
                });
                log.Info("日志记录:----处理多张图片请求完毕");
                return responseMessage;
            }
            else
            {
                log.Info("日志记录:----处理单张图片请求");
                var responseMessage = CreateResponseMessage<ResponseMessageImage>();
                responseMessage.Image.MediaId = requestMessage.MediaId;
                log.Info("日志记录:----处理单张图片完毕");
                return responseMessage;
            }
        }
        /// <summary>
        /// 处理语音请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnVoiceRequest(RequestMessageVoice requestMessage)
        {
            log.Info("日志记录:----处理语音请求");
            var responseMessage = CreateResponseMessage<ResponseMessageMusic>();
            //上传缩略图
            //var accessToken = Containers.AccessTokenContainer.TryGetAccessToken(appId, appSecret);
            var uploadResult = Senparc.Weixin.MP.AdvancedAPIs.MediaApi.UploadTemporaryMedia(appId, UploadMediaFileType.image,
                                                         Server.GetMapPath("~/Images/Logo.jpg"));
            
            //设置音乐信息
            responseMessage.Music.Title = "天籁之音";
            responseMessage.Music.Description = "播放您上传的语音";
            responseMessage.Music.MusicUrl = "http://sdk.weixin.senparc.com/Media/GetVoice?mediaId=" + requestMessage.MediaId;
            responseMessage.Music.HQMusicUrl = "http://sdk.weixin.senparc.com/Media/GetVoice?mediaId=" + requestMessage.MediaId;
            responseMessage.Music.ThumbMediaId = uploadResult.media_id;

            //推送一条客服消息
            try
            {
                CustomApi.SendText(appId, WeixinOpenId, "本次上传的音频MediaId：" + requestMessage.MediaId);

            }
            catch
            {
            }
            log.Info("日志记录:----处理语音请求完毕");
            return responseMessage;
        }
        /// <summary>
        /// 处理视频请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnVideoRequest(RequestMessageVideo requestMessage)
        {
            log.Info("日志记录:----处理视频请求");
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您发送了一条视频信息，ID：" + requestMessage.MediaId;

            #region 上传素材并推送到客户端

            Task.Factory.StartNew(async () =>
            {
                //上传素材
                var dir = Server.GetMapPath("~/App_Data/TempVideo/");
                var file = await MediaApi.GetAsync(appId, requestMessage.MediaId, dir);
                var uploadResult = await MediaApi.UploadTemporaryMediaAsync(appId, UploadMediaFileType.video, file, 50000);
                await CustomApi.SendVideoAsync(appId, base.WeixinOpenId, uploadResult.media_id, "这是您刚才发送的视频", "这是一条视频消息");
            }).ContinueWith(async task =>
            {
                if (task.Exception != null)
                {
                    //WeixinTrace.Log("OnVideoRequest()储存Video过程发生错误：", task.Exception.Message);
                    log.Info("日志记录:----OnVideoRequest()储存Video过程发生错误"+ task.Exception.Message);
                    var msg = string.Format("上传素材出错：{0}\r\n{1}",
                               task.Exception.Message,
                               task.Exception.InnerException != null
                                   ? task.Exception.InnerException.Message
                                   : null);
                    await CustomApi.SendTextAsync(appId, base.WeixinOpenId, msg);
                }
            });

            #endregion
            log.Info("日志记录:----处理视频请求完毕");
            return responseMessage;
        }

        /// <summary>
        /// 模板消息集合（Key：checkCode，Value：OpenId）
        /// </summary>
        public static Dictionary<string, string> TemplateMessageCollection = new Dictionary<string, string>();

        /// <summary>
        /// 所有没有被处理的消息会默认返回这里的结果
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            /* 所有没有被处理的消息会默认返回这里的结果，
            * 因此，如果想把整个微信请求委托出去（例如需要使用分布式或从其他服务器获取请求），
            * 只需要在这里统一发出委托请求，如：
            * var responseMessage = MessageAgent.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
            * return responseMessage;
            */
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "欢迎关注动态秀！全新动态交互方式的奇妙之旅，即刻起航！[奸笑]";
            return responseMessage;
        }
    }
}
