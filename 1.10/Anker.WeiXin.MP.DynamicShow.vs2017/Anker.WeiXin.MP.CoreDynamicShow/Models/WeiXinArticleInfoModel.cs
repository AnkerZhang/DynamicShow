using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anker.WeiXin.MP.CoreDynamicShow.Models
{
    /// <summary>
    /// 文章详情
    /// </summary>
    public class WeiXinArticleInfoModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 微信用户外键
        /// </summary>
        public WeiXinUserModel user { get; set; }
        /// <summary>
        /// 打开次数
        /// </summary>
        public int opentNumber { get; set; }
        /// <summary>
        /// 第一次打开时间
        /// </summary>
        public DateTime beginTime { get; set; }
        /// <summary>
        /// 最后一次打开时间
        /// </summary>
        public DateTime endTime { get; set; }
        /// <summary>
        /// 系数
        /// </summary>
        public int spendingDate { get; set; }
        /// <summary>
        /// 总共访问时间 秒
        /// </summary>
        public int amount { get; set; }


    }
}
