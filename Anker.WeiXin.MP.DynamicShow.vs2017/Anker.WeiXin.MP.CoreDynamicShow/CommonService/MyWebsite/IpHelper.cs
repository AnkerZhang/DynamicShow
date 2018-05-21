using Anker.WeiXin.MP.CoreDynamicShow.CommonService.MyWebsite.IP2Region;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anker.WeiXin.MP.CoreDynamicShow.CommonService.MyWebsite
{
    public static class IpHelper
    {
        public static string GetAddress(string IP)
        {
            DbSearcher dbSearcher = new DbSearcher(new DbConfig { }, AppContext.BaseDirectory + @"/DBFile/ip2region.db");
            DataBlock result = null;
            result = dbSearcher.BtreeSearch(IP);
            return result.GetRegion();
        }
        public static string GetUserIp(this HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }
        public static string GetAddressInfo(string address, DateTime date)
        {
            string result = date.Month + "月" + date.Day + "日";
            if (date.Hour < 8 || date.Hour > 18)
            {//晚上
                result = result + "晚上在" + address + "说";
            }
            else if (date.Hour > 8 && date.Hour < 12)
            {//上午
                result = result + "上午在" + address + "说";
            }
            else
            {//下午
                result = result + "下午在" + address + "说";
            }
            return result;
        }
    }
}
