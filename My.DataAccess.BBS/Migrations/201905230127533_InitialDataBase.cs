namespace My.DataAccess.BBS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDataBase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.bbs_DareDatails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Pid = c.Int(nullable: false),
                        Score = c.Int(nullable: false),
                        DareUser = c.String(maxLength: 32),
                        DareUserId = c.Guid(nullable: false),
                        DareTime = c.DateTime(nullable: false),
                        HasDareLong = c.Int(nullable: false),
                        LastLong = c.Int(nullable: false),
                        Correcrate = c.Int(nullable: false),
                        IsEnd = c.Boolean(nullable: false),
                        IsQuit = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.bbs_Dares", t => t.Pid, cascadeDelete: true)
                .Index(t => t.Pid);
            
            CreateTable(
                "dbo.bbs_Dares",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DareType = c.String(maxLength: 32),
                        DareLast = c.Int(nullable: false),
                        DareTitle = c.String(maxLength: 100),
                        DareRemark = c.String(maxLength: 500),
                        DareRemad = c.String(maxLength: 500),
                        DareQusNumber = c.Int(nullable: false),
                        QusBankId = c.Int(nullable: false),
                        QusBankName = c.String(maxLength: 100),
                        DareQueRuleId = c.Int(),
                        DareTotalScore = c.Int(nullable: false),
                        AvgRate = c.Int(nullable: false),
                        DareAppNum = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.bbs_DareQues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Pid = c.Int(nullable: false),
                        QueId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.bbs_Dares", t => t.Pid, cascadeDelete: true)
                .Index(t => t.Pid);
            
            CreateTable(
                "dbo.bbs_Genres",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsSystem = c.Boolean(nullable: false),
                        IsNews = c.Boolean(nullable: false),
                        ParentId = c.Int(),
                        Name = c.String(maxLength: 128),
                        TreeCode = c.String(maxLength: 128),
                        Leaf = c.Boolean(nullable: false),
                        Level = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.bbs_Genres", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.bbs_Idustries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Pid = c.Int(nullable: false),
                        TradeName = c.String(maxLength: 100),
                        TradeCode = c.String(maxLength: 30),
                        Elevel = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.bbs_NewDiscusses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Pid = c.Int(nullable: false),
                        DisContent = c.String(maxLength: 500),
                        DisUser = c.String(maxLength: 32),
                        DisUserId = c.Guid(nullable: false),
                        OutRuleId = c.Int(),
                        CheckUser = c.String(maxLength: 32),
                        CheckUserId = c.Guid(),
                        CheckTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.bbs_News", t => t.Pid, cascadeDelete: true)
                .Index(t => t.Pid);
            
            CreateTable(
                "dbo.bbs_News",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NewsName = c.String(maxLength: 500),
                        NewsTitle = c.String(maxLength: 200),
                        NewsListImg = c.String(maxLength: 255),
                        NewsContent = c.String(),
                        NewsFrom = c.String(maxLength: 30),
                        NewsFromUrl = c.String(maxLength: 255),
                        NewsTypeName = c.String(maxLength: 64),
                        NewsTypeId = c.Int(nullable: false),
                        OutRoleId = c.Int(),
                        CheckUser = c.String(maxLength: 32),
                        CheckUserId = c.Guid(),
                        CheckTime = c.DateTime(),
                        IsPopular = c.Boolean(nullable: false),
                        IssueTime = c.DateTime(nullable: false),
                        IssueUser = c.String(maxLength: 32),
                        IssueUserId = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.bbs_NewImgs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Pid = c.Int(nullable: false),
                        PicName = c.String(maxLength: 200),
                        PicFrom = c.String(maxLength: 255),
                        PicRemark = c.String(maxLength: 500),
                        PicPath = c.String(maxLength: 255),
                        OutRoleId = c.Int(),
                        CheckUser = c.String(maxLength: 32),
                        CheckUserId = c.Guid(),
                        CheckTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.bbs_News", t => t.Pid, cascadeDelete: true)
                .Index(t => t.Pid);
            
            CreateTable(
                "dbo.bbs_NewKeeps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Pid = c.Int(nullable: false),
                        KeepUser = c.String(maxLength: 32),
                        KeepUserId = c.Guid(nullable: false),
                        KeepTime = c.DateTime(nullable: false),
                        IsOutKeep = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.bbs_News", t => t.Pid, cascadeDelete: true)
                .Index(t => t.Pid);
            
            CreateTable(
                "dbo.bbs_NewVideoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Pid = c.Int(nullable: false),
                        VioName = c.String(maxLength: 100),
                        VidFrom = c.String(maxLength: 255),
                        VioRemork = c.String(maxLength: 500),
                        VioPath = c.String(maxLength: 255),
                        OutRoleId = c.Int(),
                        CheckUser = c.String(maxLength: 32),
                        CheckUserId = c.Guid(),
                        CheckTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.bbs_News", t => t.Pid, cascadeDelete: true)
                .Index(t => t.Pid);
            
            CreateTable(
                "dbo.bbs_PostKeeps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Pid = c.Int(nullable: false),
                        KeepUser = c.String(maxLength: 32),
                        KeepUserId = c.String(),
                        KeepTime = c.DateTime(nullable: false),
                        IsOutKeep = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.bbs_Posts", t => t.Pid, cascadeDelete: true)
                .Index(t => t.Pid);
            
            CreateTable(
                "dbo.bbs_Posts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PostContent = c.String(),
                        PostTitle = c.String(maxLength: 255),
                        PostTypeName = c.String(maxLength: 64),
                        PostTypeId = c.Int(nullable: false),
                        PostTime = c.DateTime(nullable: false),
                        PostUser = c.String(maxLength: 32),
                        PostUserId = c.Guid(nullable: false),
                        IsMarrow = c.Boolean(nullable: false),
                        IsTop = c.Boolean(nullable: false),
                        OutRoleId = c.Int(),
                        CheckUser = c.String(maxLength: 32),
                        CheckUserId = c.Guid(),
                        CheckTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.bbs_PostReplies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Pid = c.Int(nullable: false),
                        ReplyContent = c.String(),
                        ReplyUser = c.String(maxLength: 32),
                        ReplyUserId = c.Guid(nullable: false),
                        ReplyTime = c.DateTime(nullable: false),
                        OutRuleId = c.Int(),
                        CheckUser = c.String(maxLength: 32),
                        CheckUserId = c.Guid(),
                        CheckTime = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.bbs_Posts", t => t.Pid, cascadeDelete: true)
                .Index(t => t.Pid);
            
            CreateTable(
                "dbo.bbs_QueBanks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QueName = c.String(maxLength: 100),
                        QueRemark = c.String(maxLength: 255),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.bbs_Ques",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Pid = c.Int(nullable: false),
                        QueContent = c.String(maxLength: 1000),
                        TopicType = c.Int(nullable: false),
                        QueAmswer = c.String(maxLength: 50),
                        QueFrom = c.String(maxLength: 255),
                        QueLevel = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.bbs_QueBanks", t => t.Pid, cascadeDelete: true)
                .Index(t => t.Pid);
            
            CreateTable(
                "dbo.bbs_QueOptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Pid = c.Int(nullable: false),
                        OptionCon = c.String(maxLength: 255),
                        RankOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.bbs_Ques", t => t.Pid, cascadeDelete: true)
                .Index(t => t.Pid);
            
            CreateTable(
                "dbo.bbs_Users",
                c => new
                    {
                        Userid = c.Guid(nullable: false),
                        UserCode = c.String(maxLength: 30),
                        NickName = c.String(maxLength: 100),
                        Sex = c.Boolean(nullable: false),
                        MineDesc = c.String(maxLength: 200),
                        MinePresent = c.String(maxLength: 200),
                        TradeId = c.Int(nullable: false),
                        TradeName = c.String(maxLength: 100),
                        Addr = c.String(maxLength: 255),
                        RealName = c.String(maxLength: 30),
                        SdCard = c.String(maxLength: 20),
                        Email = c.String(maxLength: 50),
                        QQ = c.String(maxLength: 20),
                        Elevel = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Userid);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.bbs_QueOptions", "Pid", "dbo.bbs_Ques");
            DropForeignKey("dbo.bbs_Ques", "Pid", "dbo.bbs_QueBanks");
            DropForeignKey("dbo.bbs_PostReplies", "Pid", "dbo.bbs_Posts");
            DropForeignKey("dbo.bbs_PostKeeps", "Pid", "dbo.bbs_Posts");
            DropForeignKey("dbo.bbs_NewVideoes", "Pid", "dbo.bbs_News");
            DropForeignKey("dbo.bbs_NewKeeps", "Pid", "dbo.bbs_News");
            DropForeignKey("dbo.bbs_NewImgs", "Pid", "dbo.bbs_News");
            DropForeignKey("dbo.bbs_NewDiscusses", "Pid", "dbo.bbs_News");
            DropForeignKey("dbo.bbs_Genres", "ParentId", "dbo.bbs_Genres");
            DropForeignKey("dbo.bbs_DareQues", "Pid", "dbo.bbs_Dares");
            DropForeignKey("dbo.bbs_DareDatails", "Pid", "dbo.bbs_Dares");
            DropIndex("dbo.bbs_QueOptions", new[] { "Pid" });
            DropIndex("dbo.bbs_Ques", new[] { "Pid" });
            DropIndex("dbo.bbs_PostReplies", new[] { "Pid" });
            DropIndex("dbo.bbs_PostKeeps", new[] { "Pid" });
            DropIndex("dbo.bbs_NewVideoes", new[] { "Pid" });
            DropIndex("dbo.bbs_NewKeeps", new[] { "Pid" });
            DropIndex("dbo.bbs_NewImgs", new[] { "Pid" });
            DropIndex("dbo.bbs_NewDiscusses", new[] { "Pid" });
            DropIndex("dbo.bbs_Genres", new[] { "ParentId" });
            DropIndex("dbo.bbs_DareQues", new[] { "Pid" });
            DropIndex("dbo.bbs_DareDatails", new[] { "Pid" });
            DropTable("dbo.bbs_Users");
            DropTable("dbo.bbs_QueOptions");
            DropTable("dbo.bbs_Ques");
            DropTable("dbo.bbs_QueBanks");
            DropTable("dbo.bbs_PostReplies");
            DropTable("dbo.bbs_Posts");
            DropTable("dbo.bbs_PostKeeps");
            DropTable("dbo.bbs_NewVideoes");
            DropTable("dbo.bbs_NewKeeps");
            DropTable("dbo.bbs_NewImgs");
            DropTable("dbo.bbs_News");
            DropTable("dbo.bbs_NewDiscusses");
            DropTable("dbo.bbs_Idustries");
            DropTable("dbo.bbs_Genres");
            DropTable("dbo.bbs_DareQues");
            DropTable("dbo.bbs_Dares");
            DropTable("dbo.bbs_DareDatails");
        }
    }
}
