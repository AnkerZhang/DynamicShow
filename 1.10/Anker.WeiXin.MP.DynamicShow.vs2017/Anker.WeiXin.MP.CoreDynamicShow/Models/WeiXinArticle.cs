using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anker.WeiXin.MP.CoreDynamicShow.Models
{
    /// <summary>
    /// 文章
    /// </summary>
    public class WeiXinArticle
    {
        public int ID { get; set; }
        public WeiXinUserInfo UserInfo { get; set; }
        public string Content { get; set; }
    }
}
