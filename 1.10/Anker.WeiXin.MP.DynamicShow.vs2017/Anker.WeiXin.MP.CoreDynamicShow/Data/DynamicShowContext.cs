using Anker.WeiXin.MP.CoreDynamicShow.CommonService.MyWebsite.HandlersServer.Models;
using Anker.WeiXin.MP.CoreDynamicShow.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anker.WeiXin.MP.CoreDynamicShow.Data
{
    public class DynamicShowContext : DbContext
    {
        public DynamicShowContext(DbContextOptions<DynamicShowContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User").HasKey(u => u.ID);
            modelBuilder.Entity<Message>().ToTable("Message").HasKey(u => u.ID);
            modelBuilder.Entity<ConfigModel>().ToTable("ConfigModel").HasKey(u => u.ID);
            modelBuilder.Entity<WeiXinArticle>().ToTable("WeiXinArticle").HasKey(u => u.ID);
            modelBuilder.Entity<WeiXinUserInfo>().ToTable("WeiXinUserInfo").HasKey(u => u.ID);
            modelBuilder.Entity<WeiXinArticleInfo>().ToTable("WeiXinArticleInfo").HasKey(u => u.ID);
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<WeiXinArticle> WeiXinArticle { get; set; }
        public DbSet<WeiXinUserInfo> WeiXinUserInfo { get; set; }
        public DbSet<WeiXinArticleInfo> WeiXinArticleInfo { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ConfigModel> ConfigModel { get; set; }
    }
}
