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
                name: "WeiXinUserInfo",
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
                    unionid = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeiXinUserInfo", x => x.ID);
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
                    Content = table.Column<string>(nullable: true),
                    UserInfoID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeiXinArticle", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WeiXinArticle_WeiXinUserInfo_UserInfoID",
                        column: x => x.UserInfoID,
                        principalTable: "WeiXinUserInfo",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WeiXinArticleInfo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    AIDID = table.Column<int>(nullable: true),
                    Amount = table.Column<int>(nullable: false),
                    ClockEnd = table.Column<DateTime>(nullable: false),
                    ClockStart = table.Column<DateTime>(nullable: false),
                    OpentNumber = table.Column<int>(nullable: false),
                    SpendingDate = table.Column<int>(nullable: false),
                    UIDID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeiXinArticleInfo", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WeiXinArticleInfo_WeiXinArticle_AIDID",
                        column: x => x.AIDID,
                        principalTable: "WeiXinArticle",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WeiXinArticleInfo_WeiXinUserInfo_UIDID",
                        column: x => x.UIDID,
                        principalTable: "WeiXinUserInfo",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Message_UserID",
                table: "Message",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_WeiXinArticle_UserInfoID",
                table: "WeiXinArticle",
                column: "UserInfoID");

            migrationBuilder.CreateIndex(
                name: "IX_WeiXinArticleInfo_AIDID",
                table: "WeiXinArticleInfo",
                column: "AIDID");

            migrationBuilder.CreateIndex(
                name: "IX_WeiXinArticleInfo_UIDID",
                table: "WeiXinArticleInfo",
                column: "UIDID");
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
                name: "User");

            migrationBuilder.DropTable(
                name: "WeiXinArticle");

            migrationBuilder.DropTable(
                name: "WeiXinUserInfo");
        }
    }
}
