﻿// <auto-generated />
using Anker.WeiXin.MP.CoreDynamicShow.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Storage.Internal;
using System;

namespace Anker.WeiXin.MP.CoreDynamicShow.Migrations
{
    [DbContext(typeof(DynamicShowContext))]
    [Migration("20180518085430_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("Anker.WeiXin.MP.CoreDynamicShow.CommonService.MyWebsite.HandlersServer.Models.ConfigModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<string>("Titlt");

                    b.HasKey("ID");

                    b.ToTable("ConfigModel");
                });

            modelBuilder.Entity("Anker.WeiXin.MP.CoreDynamicShow.CommonService.MyWebsite.HandlersServer.Models.Message", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<string>("Email");

                    b.Property<string>("IP");

                    b.Property<string>("Image");

                    b.Property<string>("NickName");

                    b.Property<int>("State");

                    b.Property<DateTime>("Time");

                    b.Property<int>("UserID");

                    b.Property<string>("Website");

                    b.Property<string>("address");

                    b.Property<string>("city");

                    b.Property<string>("country");

                    b.Property<string>("province");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Message");
                });

            modelBuilder.Entity("Anker.WeiXin.MP.CoreDynamicShow.CommonService.MyWebsite.HandlersServer.Models.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<string>("Email");

                    b.Property<string>("IP");

                    b.Property<string>("Image");

                    b.Property<string>("NickName");

                    b.Property<int>("State");

                    b.Property<DateTime>("Time");

                    b.Property<string>("Website");

                    b.Property<string>("address");

                    b.Property<string>("city");

                    b.Property<string>("country");

                    b.Property<string>("province");

                    b.Property<int>("ticket");

                    b.HasKey("ID");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Anker.WeiXin.MP.CoreDynamicShow.Models.WeiXinArticleInfoLogModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("amount");

                    b.Property<int>("articleInfoID");

                    b.Property<DateTime>("beginTime");

                    b.Property<DateTime>("endTime");

                    b.Property<string>("remarks");

                    b.HasKey("ID");

                    b.ToTable("WeiXinArticleInfoLog");
                });

            modelBuilder.Entity("Anker.WeiXin.MP.CoreDynamicShow.Models.WeiXinArticleInfoModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("WeiXinArticleModelID");

                    b.Property<int>("amount");

                    b.Property<DateTime>("beginTime");

                    b.Property<DateTime>("endTime");

                    b.Property<int>("opentNumber");

                    b.Property<int>("spendingDate");

                    b.Property<int?>("userID");

                    b.HasKey("ID");

                    b.HasIndex("WeiXinArticleModelID");

                    b.HasIndex("userID");

                    b.ToTable("WeiXinArticleInfo");
                });

            modelBuilder.Entity("Anker.WeiXin.MP.CoreDynamicShow.Models.WeiXinArticleModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Music");

                    b.Property<string>("author");

                    b.Property<string>("content");

                    b.Property<string>("contentTitle");

                    b.Property<string>("qrCode");

                    b.Property<int>("state");

                    b.Property<DateTime>("time");

                    b.Property<string>("title");

                    b.Property<string>("titleImg");

                    b.Property<int?>("userIDID");

                    b.HasKey("ID");

                    b.HasIndex("userIDID");

                    b.ToTable("WeiXinArticle");
                });

            modelBuilder.Entity("Anker.WeiXin.MP.CoreDynamicShow.Models.WeiXinUserInfoModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("WeiXinUserModelID");

                    b.Property<DateTime>("logTime");

                    b.Property<string>("remarks");

                    b.HasKey("ID");

                    b.HasIndex("WeiXinUserModelID");

                    b.ToTable("WeiXinUserInfo");
                });

            modelBuilder.Entity("Anker.WeiXin.MP.CoreDynamicShow.Models.WeiXinUserModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("city");

                    b.Property<string>("country");

                    b.Property<string>("headimgurl");

                    b.Property<string>("nickname");

                    b.Property<string>("openid");

                    b.Property<string>("province");

                    b.Property<int>("sex");

                    b.Property<DateTime>("time");

                    b.Property<string>("unionid");

                    b.HasKey("ID");

                    b.ToTable("WeiXinUser");
                });

            modelBuilder.Entity("Anker.WeiXin.MP.CoreDynamicShow.CommonService.MyWebsite.HandlersServer.Models.Message", b =>
                {
                    b.HasOne("Anker.WeiXin.MP.CoreDynamicShow.CommonService.MyWebsite.HandlersServer.Models.User")
                        .WithMany("Meaages")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Anker.WeiXin.MP.CoreDynamicShow.Models.WeiXinArticleInfoModel", b =>
                {
                    b.HasOne("Anker.WeiXin.MP.CoreDynamicShow.Models.WeiXinArticleModel")
                        .WithMany("articleInfoList")
                        .HasForeignKey("WeiXinArticleModelID");

                    b.HasOne("Anker.WeiXin.MP.CoreDynamicShow.Models.WeiXinUserModel", "user")
                        .WithMany()
                        .HasForeignKey("userID");
                });

            modelBuilder.Entity("Anker.WeiXin.MP.CoreDynamicShow.Models.WeiXinArticleModel", b =>
                {
                    b.HasOne("Anker.WeiXin.MP.CoreDynamicShow.Models.WeiXinUserModel", "userID")
                        .WithMany()
                        .HasForeignKey("userIDID");
                });

            modelBuilder.Entity("Anker.WeiXin.MP.CoreDynamicShow.Models.WeiXinUserInfoModel", b =>
                {
                    b.HasOne("Anker.WeiXin.MP.CoreDynamicShow.Models.WeiXinUserModel")
                        .WithMany("userInfoList")
                        .HasForeignKey("WeiXinUserModelID");
                });
#pragma warning restore 612, 618
        }
    }
}
