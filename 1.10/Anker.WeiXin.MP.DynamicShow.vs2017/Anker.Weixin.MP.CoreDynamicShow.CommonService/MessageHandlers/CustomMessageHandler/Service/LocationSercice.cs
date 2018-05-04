using System;
using System.Collections.Generic;
using System.Text;

namespace Anker.Weixin.MP.CoreDynamicShow.CommonService.MessageHandlers.CustomMessageHandler.Service
{
    public class LocationSercice
    {
        /// <summary>
        /// 接收文本
        /// </summary>
        /// <param name="FromUserName"></param>
        /// <param name="ToUserName"></param>
        /// <param name="Content"></param>
        /// <returns></returns>
        public static string GetText(string Content)
        {
            // Common.CommonMethod.WriteTxt(Content);//接收的文本消息
            string result = "";
            switch (Content)
            {
                case "关键字":
                    result = "关键词回复测试";
                    break;
                case "我":
                    result = "我的个人主页 http://www.nbug.xin:5000/";
                    break;
                case "测试":
                    result = "您已经进入的测试程序，请发送任意信息进行测试。";
                    break;
                default:
                    result = "你发送的消息是" + Content;
                    break;
            }
            return result;
        }
    }
}
