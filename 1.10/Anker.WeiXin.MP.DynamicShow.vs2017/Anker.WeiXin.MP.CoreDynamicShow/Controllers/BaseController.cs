using Anker.WeiXin.MP.CoreDynamicShow.Data;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Senparc.Weixin.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anker.WeiXin.MP.CoreDynamicShow.Controllers
{
    public class BaseController: Controller
    {
        
        protected string appId;
        protected string appSecret;
        protected string token;
        protected  DynamicShowContext _context;
        protected string encodingAESKey;
        protected SenparcWeixinSetting _senparcWeixinSetting;
        protected ILog log = null;
        protected IHostingEnvironment _host = null;
        protected int uid = 0;

    }
}
