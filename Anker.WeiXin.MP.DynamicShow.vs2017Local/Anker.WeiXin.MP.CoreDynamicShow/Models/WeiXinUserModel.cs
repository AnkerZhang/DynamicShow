using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anker.WeiXin.MP.CoreDynamicShow.Models
{
    /// <summary>
    /// 微信基本信息
    /// </summary>
    public class WeiXinUserModel
    {
        public int ID { get; set; }
        public string openid { get; set; }
        //
        // 摘要:
        //     用户昵称
        public string nickname { get; set; }
        //
        // 摘要:
        //     用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
        public int sex { get; set; }
        //
        // 摘要:
        //     用户个人资料填写的省份
        public string province { get; set; }
        //
        // 摘要:
        //     普通用户个人资料填写的城市
        public string city { get; set; }
        //
        // 摘要:
        //     国家，如中国为CN
        public string country { get; set; }
        //
        // 摘要:
        //     用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空
        public string headimgurl { get; set; }
        //
        // 摘要:
        //     用户特权信息，json 数组，如微信沃卡用户为（chinaunicom） 作者注：其实这个格式称不上JSON，只是个单纯数组。

        public string unionid { get; set; }
        /// <summary>
        /// 录取时间
        /// </summary>
        public DateTime time { get; set; }
        /// <summary>
        /// 微信用户详情
        /// </summary>

        public List<WeiXinUserInfoModel> userInfoList { get; set; }
    }
}
