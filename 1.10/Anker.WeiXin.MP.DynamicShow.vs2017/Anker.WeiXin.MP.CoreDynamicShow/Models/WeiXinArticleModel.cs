using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anker.WeiXin.MP.CoreDynamicShow.Models
{
    /// <summary>
    /// 文章
    /// </summary>13552847894
    public class WeiXinArticleModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 微信基本信息
        /// </summary>
        public WeiXinUserModel userID { get; set; }
        /// <summary>
        /// 文章详情
        /// </summary>
        public List<WeiXinArticleInfoModel> articleInfoList { get; set; }
        /// <summary>
        /// 发表时间
        /// </summary>
        public DateTime time { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int state { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string author { get; set; }
        /// <summary>
        /// 音乐
        /// </summary>
        public string Music { get; set; }
        /// <summary>
        /// 标题图片
        /// </summary>
        public string titleImg { get; set; }
        /// <summary>
        /// 二维码
        /// </summary>
        public string qrCode { get; set; }
    }
}
