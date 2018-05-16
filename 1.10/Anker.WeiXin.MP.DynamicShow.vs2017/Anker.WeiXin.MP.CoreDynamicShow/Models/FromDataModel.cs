using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anker.WeiXin.MP.CoreDynamicShow.Models
{
    public class FromDataModel
    {
        public string title { get; set; }
        public string zuozhe { get; set; }
        public string music { get; set; }
        public string contentTitle { get; set; }
        public IFormFile xiaofile { get; set; }
        public List<string> strlist { get; set; }
        public IFormFile tu1file { get; set; }
        public IFormFile tu2file { get; set; }
        public IFormFile tu3file { get; set; }
        public IFormFile tu4file { get; set; }
        public IFormFile tu5file { get; set; }
        public IFormFile tu6file { get; set; }
        public IFormFile tu7file { get; set; }
        public IFormFile tu8file { get; set; }
        public IFormFile tu9file { get; set; }
        public IFormFile tu10file { get; set; }
        public IFormFile tu11file { get; set; }
        public IFormFile tu12file { get; set; }
        public IFormFile tu13file { get; set; }
        public IFormFile tu14file { get; set; }
        public IFormFile tu15file { get; set; }

    }
    public class CommentInfo
    {
        public string str { get; set; }
        public string fileName { get; set; }
    }

}
