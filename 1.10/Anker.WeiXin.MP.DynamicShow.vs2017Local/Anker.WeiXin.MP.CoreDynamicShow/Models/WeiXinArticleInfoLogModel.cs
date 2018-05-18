using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anker.WeiXin.MP.CoreDynamicShow.Models
{
    /// <summary>
    /// 文章详情每次打开记录日志
    /// </summary>
    public class WeiXinArticleInfoLogModel
    {
        public int ID { get; set; }
        public int articleInfoID { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime beginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime endTime { get; set; }
        /// <summary>
        /// 访问时间 秒
        /// </summary>
        public int amount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remarks { get; set; }

    }
}
