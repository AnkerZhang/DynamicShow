using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anker.WeiXin.MP.CoreDynamicShow.CommonService.MyWebsite.HandlersServer.Models
{
    public class SendDataModel
    {
        public string author { get; set; }
        public string content { get; set; }
        public string email { get; set; }
        public string url { get; set; }
        public string comment_parent { get; set; }
        public string images { get; set; }
    }
}
