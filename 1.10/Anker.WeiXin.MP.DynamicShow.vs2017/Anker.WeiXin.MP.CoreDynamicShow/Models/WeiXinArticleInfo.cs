using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anker.WeiXin.MP.CoreDynamicShow.Models
{
    public class WeiXinArticleInfo
    {
        public int ID { get; set; }
        public WeiXinArticle AID { get; set; }
        public WeiXinUserInfo UID { get; set; }
        public int OpentNumber { get; set; }
        public DateTime ClockStart { get; set; }
        public DateTime ClockEnd { get; set; }
        public int SpendingDate { get; set; }
        public int Amount { get; set; }
        
    }
}
