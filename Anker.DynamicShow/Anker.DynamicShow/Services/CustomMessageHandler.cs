using System.Collections.Generic;
using System.IO;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.Context;
using log4net;

namespace Anker.DynamicShow.Services
{
    public partial class CustomMessageHandler : MessageHandler<MessageContext<IRequestMessageBase, IResponseMessageBase>>
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(CustomMessageHandler));

        public CustomMessageHandler(Stream inputStream, PostModel postModel) : base(inputStream, postModel)
        {
        }

        public CustomMessageHandler(RequestMessageBase requestMessage) : base(requestMessage)
        {
        }

        public override IResponseMessageBase OnEventRequest(IRequestMessageEventBase requestMessage)
        {
            return base.OnEventRequest(requestMessage);
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这条消息来自于DefaultResponseMessage";
            return responseMessage;
        }

        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            log.Info("您的OpenID是：" + responseMessage.FromUserName + ".\r\n您发送的文字是：" + requestMessage.Content+" msgID"+requestMessage.MsgId);
            responseMessage.Content = "您的OpenID是：" + responseMessage.FromUserName+".\r\n您发送的文字是："+requestMessage.Content;  
            if (requestMessage.Content == "ID")
                responseMessage.Content = "您的OpenID是：" + responseMessage.FromUserName;
            if (requestMessage.Content == "天气")
                responseMessage.Content = "抱歉，还未开通此功能！";
            return responseMessage;
        }
    }
}
