using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Anker.WeiXin.MP.CoreDynamicShow.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfigModel",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Content = table.Column<string>(nullable: true),
                    Titlt = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigModel", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Content = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    IP = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    NickName = table.Column<string>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    Website = table.Column<string>(nullable: true),
                    address = table.Column<string>(nullable: true),
                    city = table.Column<string>(nullable: true),
                    country = table.Column<string>(nullable: true),
                    province = table.Column<string>(nullable: true),
                    ticket = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "WeiXinArticleInfoLog",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    amount = table.Column<int>(nullable: false),
                    articleInfoID = table.Column<int>(nullable: false),
                    beginTime = table.Column<DateTime>(nullable: false),
                    endTime = table.Column<DateTime>(nullable: false),
                    remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeiXinArticleInfoLog", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "WeiXinUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    city = table.Column<string>(nullable: true),
                    country = table.Column<string>(nullable: true),
                    headimgurl = table.Column<string>(nullable: true),
                    nickname = table.Column<string>(nullable: true),
                    openid = table.Column<string>(nullable: true),
                    province = table.Column<string>(nullable: true),
                    sex = table.Column<int>(nullable: false),
                    time = table.Column<DateTime>(nullable: false),
                    unionid = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeiXinUser", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Content = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    IP = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    NickName = table.Column<string>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    Website = table.Column<string>(nullable: true),
                    address = table.Column<string>(nullable: true),
                    city = table.Column<string>(nullable: true),
                    country = table.Column<string>(nullable: true),
                    province = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Message_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeiXinArticle",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Music = table.Column<string>(nullable: true),
                    author = table.Column<string>(nullable: true),
                    content = table.Column<string>(nullable: true),
                    contentTitle = table.Column<string>(nullable: true),
                    qrCode = table.Column<string>(nullable: true),
                    state = table.Column<int>(nullable: false),
                    time = table.Column<DateTime>(nullable: false),
                    title = table.Column<string>(nullable: true),
                    titleImg = table.Column<string>(nullable: true),
                    userIDID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeiXinArticle", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WeiXinArticle_WeiXinUser_userIDID",
                        column: x => x.userIDID,
                        principalTable: "WeiXinUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WeiXinUserInfo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    WeiXinUserModelID = table.Column<int>(nullable: true),
                    logTime = table.Column<DateTime>(nullable: false),
                    remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeiXinUserInfo", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WeiXinUserInfo_WeiXinUser_WeiXinUserModelID",
                        column: x => x.WeiXinUserModelID,
                        principalTable: "WeiXinUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WeiXinArticleInfo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    WeiXinArticleModelID = table.Column<int>(nullable: true),
                    amount = table.Column<int>(nullable: false),
                    beginTime = table.Column<DateTime>(nullable: false),
                    endTime = table.Column<DateTime>(nullable: false),
                    opentNumber = table.Column<int>(nullable: false),
                    spendingDate = table.Column<int>(nullable: false),
                    userID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeiXinArticleInfo", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WeiXinArticleInfo_WeiXinArticle_WeiXinArticleModelID",
                        column: x => x.WeiXinArticleModelID,
                        principalTable: "WeiXinArticle",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WeiXinArticleInfo_WeiXinUser_userID",
                        column: x => x.userID,
                        principalTable: "WeiXinUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Message_UserID",
                table: "Message",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_WeiXinArticle_userIDID",
                table: "WeiXinArticle",
                column: "userIDID");

            migrationBuilder.CreateIndex(
                name: "IX_WeiXinArticleInfo_WeiXinArticleModelID",
                table: "WeiXinArticleInfo",
                column: "WeiXinArticleModelID");

            migrationBuilder.CreateIndex(
                name: "IX_WeiXinArticleInfo_userID",
                table: "WeiXinArticleInfo",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_WeiXinUserInfo_WeiXinUserModelID",
                table: "WeiXinUserInfo",
                column: "WeiXinUserModelID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigModel");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "WeiXinArticleInfo");

            migrationBuilder.DropTable(
                name: "WeiXinArticleInfoLog");

            migrationBuilder.DropTable(
                name: "WeiXinUserInfo");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "WeiXinArticle");

            migrationBuilder.DropTable(
                name: "WeiXinUser");
        }
    }
}
