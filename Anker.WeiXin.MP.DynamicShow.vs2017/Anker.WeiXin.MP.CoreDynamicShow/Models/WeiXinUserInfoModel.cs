using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anker.WeiXin.MP.CoreDynamicShow.Models
{
    /// <summary>
    /// 微信用户详情
    /// </summary>
    public class WeiXinUserInfoModel
    {
        public int ID { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime logTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remarks { get; set; }

    }
}
