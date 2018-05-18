using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anker.WeiXin.MP.CoreDynamicShow.CommonService.MyWebsite.HandlersServer.Models
{
    /// <summary>
    /// 用户表
    /// </summary>
    public class User
    {
        public int ID { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        public string IP { get; set; }
        public string country { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 电子邮件
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 网址
        /// </summary>
        public string Website { get; set; }
        /// <summary>
        /// 头像图片
        /// </summary>
        public string Image { get; set; }
        public DateTime Time { get; set; }
        /// <summary>
        /// 状态1：正常0：不可回复，-1：不可见
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 票数
        /// </summary>
        public int ticket { get; set; }
        public List<Message> Meaages{ get; set; }
    }
}
