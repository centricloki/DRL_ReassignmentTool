using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DRL.Model.Models
{
    public partial class DRLNewContext : DbContext
    {
        //public DRLNewContext()
        //{
        //}

        public DRLNewContext(DbContextOptions<DRLNewContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccountClassificationTypeMaster> AccountClassificationTypeMaster { get; set; }
        public virtual DbSet<AppVersion> AppVersion { get; set; }
        public virtual DbSet<AuditCallActivity> AuditCallActivity { get; set; }
        public virtual DbSet<AuditContact> AuditContact { get; set; }
        public virtual DbSet<AuditCustomer> AuditCustomer { get; set; }
        public virtual DbSet<AuditCustomerDocument> AuditCustomerDocument { get; set; }
        public virtual DbSet<AuditLogin> AuditLogin { get; set; }
        public virtual DbSet<AuditOrder> AuditOrder { get; set; }
        public virtual DbSet<AuditRoute> AuditRoute { get; set; }
        public virtual DbSet<AuditUserTax> AuditUserTax { get; set; }
        public virtual DbSet<BrandStyleMaster> BrandStyleMaster { get; set; }
        public virtual DbSet<CallActivity> CallActivity { get; set; }
        public virtual DbSet<CategoryMaster> CategoryMaster { get; set; }
        public virtual DbSet<CategoryProduct> CategoryProduct { get; set; }
        public virtual DbSet<CityMaster> CityMaster { get; set; }
        public virtual DbSet<Configuration> Configuration { get; set; }
        public virtual DbSet<ContactMaster> ContactMaster { get; set; }
        public virtual DbSet<ContactTypeMaster> ContactTypeMaster { get; set; }
        public virtual DbSet<ContractProgram> ContractProgram { get; set; }
        public virtual DbSet<CorporateProgram> CorporateProgram { get; set; }
        public virtual DbSet<CustomerDistributor> CustomerDistributor { get; set; }
        public virtual DbSet<CustomerDocument> CustomerDocument { get; set; }
        public virtual DbSet<CustomerMaster> CustomerMaster { get; set; }
        public virtual DbSet<CustomerProduct> CustomerProduct { get; set; }
        public virtual DbSet<DocumentEmail> DocumentEmail { get; set; }
        public virtual DbSet<IpadsyncData> IpadsyncData { get; set; }
        public virtual DbSet<LnkPopitems> LnkPopitems { get; set; }
        public virtual DbSet<LnkRackItems> LnkRackItems { get; set; }
        public virtual DbSet<NoteMaster> NoteMaster { get; set; }
        public virtual DbSet<OrderDetail> OrderDetail { get; set; }
        public virtual DbSet<OrderHistoryEmail> OrderHistoryEmail { get; set; }
        public virtual DbSet<OrderMaster> OrderMaster { get; set; }
        public virtual DbSet<PositionMaster> PositionMaster { get; set; }
        public virtual DbSet<ProductAdditionalDocument> ProductAdditionalDocument { get; set; }
        public virtual DbSet<ProductMaster> ProductMaster { get; set; }
        public virtual DbSet<ProductRoleLink> ProductRoleLink { get; set; }
        public virtual DbSet<Promotion> Promotion { get; set; }
        public virtual DbSet<RackCategoryDetail> RackCategoryDetail { get; set; }
        public virtual DbSet<RackCategoryMaster> RackCategoryMaster { get; set; }
        public virtual DbSet<RackImages> RackImages { get; set; }
        public virtual DbSet<RankMaster> RankMaster { get; set; }
        public virtual DbSet<RecordResourceType> RecordResourceType { get; set; }
        public virtual DbSet<RegionMaster> RegionMaster { get; set; }
        public virtual DbSet<RoleMaster> RoleMaster { get; set; }
        public virtual DbSet<RouteStations> RouteStations { get; set; }
        public virtual DbSet<SalesDocument> SalesDocument { get; set; }
        public virtual DbSet<ScheduleRoutes> ScheduleRoutes { get; set; }
        public virtual DbSet<StateMaster> StateMaster { get; set; }
        public virtual DbSet<StyleMaster> StyleMaster { get; set; }
        public virtual DbSet<SugarUpdated> SugarUpdated { get; set; }
        public virtual DbSet<SupplyChain> SupplyChain { get; set; }
        public virtual DbSet<TerritoryMaster> TerritoryMaster { get; set; }
        public virtual DbSet<UserApplicationDetails> UserApplicationDetails { get; set; }
        public virtual DbSet<UserCustomer> UserCustomer { get; set; }
        public virtual DbSet<UserMaster> UserMaster { get; set; }
        public virtual DbSet<UserMasterwithSugar> UserMasterwithSugar { get; set; }
        public virtual DbSet<UserTaxStatement> UserTaxStatement { get; set; }
        public virtual DbSet<ZoneMaster> ZoneMaster { get; set; }
        public virtual DbSet<AVPMaster> AVPMaster { get; set; }
        public virtual DbSet<BDMaster> BDMaster { get; set; }

        // Unable to generate entity type for table 'dbo.Travel'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.RepublicSalesSpiffTarget'. Please see the warning messages.

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseLazyLoadingProxies();
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("Server=PC74\\SQL2017EXP;initial catalog=DRLNew;user id=sa;password=softweb#123;MultipleActiveResultSets=True;");
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<AccountClassificationTypeMaster>(entity =>
            {
                entity.HasKey(e => e.AccountClassificationId);

                entity.Property(e => e.AccountClassificationId).HasColumnName("AccountClassificationID");

                entity.Property(e => e.AccountClassificationName)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerType).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");
            });

            modelBuilder.Entity<AppVersion>(entity =>
            {
                entity.Property(e => e.AppVersionId).HasColumnName("AppVersionID");

                entity.Property(e => e.DownloadUrl)
                    .HasColumnName("DownloadURL")
                    .HasColumnType("text");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsProduction).HasDefaultValueSql("((0))");

                entity.Property(e => e.Version)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.WhatsNew).HasColumnType("text");
            });

            modelBuilder.Entity<AuditCallActivity>(entity =>
            {
                entity.Property(e => e.AuditCallActivityId).HasColumnName("AuditCallActivityID");

                entity.Property(e => e.AuditDateTime).HasColumnType("datetime");

                entity.Property(e => e.CallActivityId).HasColumnName("CallActivityID");

                entity.Property(e => e.ErrorMsg)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.LoginPassword)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.LoginUserName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Mode).HasDefaultValueSql("((1))");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<AuditContact>(entity =>
            {
                entity.Property(e => e.AuditContactId).HasColumnName("AuditContactID");

                entity.Property(e => e.AuditDateTime).HasColumnType("datetime");

                entity.Property(e => e.ContactId)
                    .HasColumnName("ContactID")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ErrorMsg)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.LoginPassword)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.LoginUserName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<AuditCustomer>(entity =>
            {
                entity.Property(e => e.AuditCustomerId).HasColumnName("AuditCustomerID");

                entity.Property(e => e.AuditDateTime).HasColumnType("datetime");

                entity.Property(e => e.CustomerId)
                    .IsRequired()
                    .HasColumnName("CustomerID")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ErrorMsg)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.LoginPassword)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.LoginUserName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Mode).HasDefaultValueSql("((1))");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<AuditCustomerDocument>(entity =>
            {
                entity.Property(e => e.AuditCustomerDocumentId).HasColumnName("AuditCustomerDocumentID");

                entity.Property(e => e.AuditDateTime).HasColumnType("datetime");

                entity.Property(e => e.CustomerDocumentId).HasColumnName("CustomerDocumentID");

                entity.Property(e => e.ErrorMsg)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.LoginPassword)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.LoginUserName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<AuditLogin>(entity =>
            {
                entity.Property(e => e.AuditLoginId).HasColumnName("AuditLoginID");

                entity.Property(e => e.AuditDateTime).HasColumnType("datetime");

                entity.Property(e => e.DbfileName)
                    .HasColumnName("DBFileName")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ErrorMsg)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.LastSyncUtcdate)
                    .HasColumnName("lastSyncUTCDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.LoginPassword)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.LoginUserName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.NewPassword)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.OldPassword)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.SupplyChainsCount).HasColumnName("Supply_ChainsCount");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.VersionNumber)
                    .HasMaxLength(150)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AuditOrder>(entity =>
            {
                entity.Property(e => e.AuditOrderId).HasColumnName("AuditOrderID");

                entity.Property(e => e.AuditDateTime).HasColumnType("datetime");

                entity.Property(e => e.ErrorMsg)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.LoginPassword)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.LoginUserName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<AuditRoute>(entity =>
            {
                entity.Property(e => e.AuditRouteId).HasColumnName("AuditRouteID");

                entity.Property(e => e.AuditDateTime).HasColumnType("datetime");

                entity.Property(e => e.ErrorMsg)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.LoginPassword)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.LoginUserName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Mode).HasDefaultValueSql("((1))");

                entity.Property(e => e.RouteId).HasColumnName("RouteID");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<AuditUserTax>(entity =>
            {
                entity.Property(e => e.AuditUserTaxId).HasColumnName("AuditUserTaxID");

                entity.Property(e => e.AuditDateTime).HasColumnType("datetime");

                entity.Property(e => e.ErrorMsg)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.LoginPassword)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.LoginUserName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.UserTaxStatementId)
                    .HasColumnName("UserTaxStatementID")
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<BrandStyleMaster>(entity =>
            {
                entity.HasKey(e => e.BrandIstyleId);

                entity.Property(e => e.BrandIstyleId).HasColumnName("BrandIStyleID");

                entity.Property(e => e.BrandStyleName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ImageFilePath)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.IsPopOrder).HasDefaultValueSql("((0))");

                entity.Property(e => e.LangId).HasColumnName("LangID");

                entity.Property(e => e.ParentId).HasColumnName("ParentID");

                entity.Property(e => e.PromoId).HasColumnName("PromoID");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");
            });

            modelBuilder.Entity<CallActivity>(entity =>
            {
                entity.HasIndex(e => e.CustomerId)
                    .HasName("idx_nclstr_CallActivity_CustomerId");

                entity.HasIndex(e => e.OrderId)
                    .HasName("idx_nclstr_CallActivity_orderid");

                entity.HasIndex(e => e.UpdateDate)
                    .HasName("idx_nclstr_CallActivity_UpdateDate");

                entity.HasIndex(e => e.UserId)
                    .HasName("idx_nclstr_CallActivity_userid");

                entity.HasIndex(e => new { e.CallDate, e.CustomerId, e.UserId })
                    .HasName("CallDate_CustID_UserID");

                entity.Property(e => e.ActivityType)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CallDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CarSalesSold)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Comments).HasColumnType("text");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.DeviceCallActivityId)
                    .HasColumnName("DeviceCallActivityID")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.DeviceCustomerId)
                    .HasColumnName("DeviceCustomerID")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.GratisProductUsed)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Hours).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsThisFromYourList)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Objective)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Result).IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('CLOSED')");

                entity.Property(e => e.SugarCallActivityId)
                    .HasColumnName("SugarCallActivityID")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.WholesaleInvoiceFilePath)
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CategoryMaster>(entity =>
            {
                entity.HasKey(e => e.CategoryId);

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.CategoryStatus).HasDefaultValueSql("((1))");

                entity.Property(e => e.ErpcategoryId)
                    .HasColumnName("ERPCategoryID")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ImageFilePath)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.LangId).HasColumnName("LangID");

                entity.Property(e => e.ParentCategoryId).HasColumnName("ParentCategoryID");

                entity.Property(e => e.PromoId).HasColumnName("PromoID");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");
            });

            modelBuilder.Entity<CategoryProduct>(entity =>
            {
                entity.Property(e => e.CategoryProductId).HasColumnName("CategoryProductID");

                entity.Property(e => e.BrandIstyleId).HasColumnName("BrandIStyleID");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");
            });

            modelBuilder.Entity<CityMaster>(entity =>
            {
                entity.HasKey(e => e.CityId);

                entity.Property(e => e.CityId).HasColumnName("CityID");

                entity.Property(e => e.CityName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.StateId).HasColumnName("StateID");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");
            });

            modelBuilder.Entity<Configuration>(entity =>
            {
                entity.HasKey(e => e.ConfigId);

                entity.Property(e => e.ConfigId).HasColumnName("ConfigID");

                entity.Property(e => e.Keyname)
                    .IsRequired()
                    .HasColumnName("KEYName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Keyvalue)
                    .IsRequired()
                    .HasColumnName("KEYValue")
                    .HasMaxLength(300)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ContactMaster>(entity =>
            {
                entity.HasKey(e => e.ContactId);

                entity.Property(e => e.ContactId).HasColumnName("ContactID");

                entity.Property(e => e.ContactCellPhone)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ContactEmail)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.ContactFax)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ContactName)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ContactNote)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.ContactPhone)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ContactRole)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ContactTypeId).HasColumnName("ContactTypeID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.DeviceContactId)
                    .HasColumnName("DeviceContactID")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PositionId)
                    .HasColumnName("PositionID")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.RankId)
                    .HasColumnName("RankID")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.SugarContactId)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");
            });

            modelBuilder.Entity<ContactTypeMaster>(entity =>
            {
                entity.HasKey(e => e.ContactTypeId);

                entity.Property(e => e.ContactTypeId).HasColumnName("ContactTypeID");

                entity.Property(e => e.ContactType)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ContractProgram>(entity =>
            {
                entity.Property(e => e.ContractProgramId).HasColumnName("ContractProgramID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Effective).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Payment1Amount)
                    .HasColumnName("Payment1_Amount")
                    .HasColumnType("decimal(26, 6)");

                entity.Property(e => e.Payment1CheckNo).HasColumnName("Payment1_CheckNo");

                entity.Property(e => e.Payment1Date)
                    .HasColumnName("Payment1_Date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Payment1Request).HasColumnName("Payment1_Request");

                entity.Property(e => e.Payment2Amount)
                    .HasColumnName("Payment2_Amount")
                    .HasColumnType("decimal(26, 6)");

                entity.Property(e => e.Payment2CheckNo).HasColumnName("Payment2_CheckNo");

                entity.Property(e => e.Payment2Date)
                    .HasColumnName("Payment2_Date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Payment2Request).HasColumnName("Payment2_Request");

                entity.Property(e => e.Plan)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.SugarId)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.Year)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CorporateProgram>(entity =>
            {
                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.Awards)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.BonusPoint).HasColumnName("Bonus_Point");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ContactId).HasColumnName("ContactID");

                entity.Property(e => e.CsLyr).HasColumnName("Cs_Lyr");

                entity.Property(e => e.CsNeeded).HasColumnName("Cs_Needed");

                entity.Property(e => e.CsYtd).HasColumnName("Cs_Ytd");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.EarnedPoint).HasColumnName("Earned_Point");

                entity.Property(e => e.Effective).HasColumnType("datetime");

                entity.Property(e => e.Level)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.MarketArea)
                    .IsRequired()
                    .HasColumnName("Market_Area")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Meeting)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NeededPoint).HasColumnName("Needed_point");

                entity.Property(e => e.NetPoints).HasColumnName("Net_Points");

                entity.Property(e => e.Next)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Payment1Amount)
                    .HasColumnName("Payment1_Amount")
                    .HasColumnType("decimal(26, 6)");

                entity.Property(e => e.Payment1CheckNo).HasColumnName("Payment1_Check_No");

                entity.Property(e => e.Payment1Date)
                    .HasColumnName("Payment1_Date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Payment1Request).HasColumnName("Payment1_Request");

                entity.Property(e => e.Payment2Amount)
                    .HasColumnName("Payment2_Amount")
                    .HasColumnType("decimal(26, 6)");

                entity.Property(e => e.Payment2CheckNo).HasColumnName("Payment2_Check_No");

                entity.Property(e => e.Payment2Date)
                    .HasColumnName("Payment2_Date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Payment2Request).HasColumnName("Payment2_Request");

                entity.Property(e => e.Pd).HasColumnName("PD");

                entity.Property(e => e.Plan)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Qualified)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.R)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Rebate).HasColumnType("decimal(26, 6)");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.StrretAddress)
                    .IsRequired()
                    .HasColumnName("Strret_Address")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.SugarCorporateProgramId)
                    .HasColumnName("SugarCorporateProgramID")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Tp)
                    .IsRequired()
                    .HasColumnName("TP")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.TpLevel)
                    .IsRequired()
                    .HasColumnName("TP_level")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.TravelGoal)
                    .IsRequired()
                    .HasColumnName("Travel_Goal")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.TraveleresTitle)
                    .IsRequired()
                    .HasColumnName("Traveleres_Title")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.TravelersName)
                    .IsRequired()
                    .HasColumnName("Travelers_Name")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.Vrip)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.VripYear)
                    .IsRequired()
                    .HasColumnName("Vrip_Year")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Year)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CustomerDistributor>(entity =>
            {
                entity.Property(e => e.CustomerDistributorId).HasColumnName("CustomerDistributorID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.DeviceCustomerId)
                    .HasColumnName("DeviceCustomerID")
                    .HasMaxLength(400)
                    .IsUnicode(false);

                entity.Property(e => e.DistributorId).HasColumnName("DistributorID");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");
            });

            modelBuilder.Entity<CustomerDocument>(entity =>
            {
                entity.Property(e => e.CustomerDocumentId).HasColumnName("CustomerDocumentID");

                entity.Property(e => e.CreateDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.CustomerDocumentPath)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.DeviceDocumentId)
                    .HasColumnName("DeviceDocumentID")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.DocumentType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OriginalFileName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.SugarCustomerDocId)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");
            });

            modelBuilder.Entity<CustomerMaster>(entity =>
            {
                entity.HasKey(e => e.CustomerId);

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.AccountResponsibility)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.AccountType).HasDefaultValueSql("((1))");

                entity.Property(e => e.AssignUserId)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.AssociatedInternalSalesGuy)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Broker)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.BuyType)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Buyer)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerNumber).HasMaxLength(10);

                entity.Property(e => e.DeviceCustomerId)
                    .HasColumnName("DeviceCustomerID")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DeviceDistributorCustomerId)
                    .HasColumnName("DeviceDistributorCustomerID")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DistributorId)
                    .IsRequired()
                    .HasColumnName("DistributorID")
                    .HasMaxLength(10)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.EmailId)
                    .HasColumnName("EmailID")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Fax)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.IsCreatePermanent).HasColumnName("isCreatePermanent");

                entity.Property(e => e.MailingAddress)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.MailingAddressCity)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.MailingAddressStateId).HasColumnName("MailingAddressStateID");

                entity.Property(e => e.MailingAddressZipCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ManagerName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PhysicalAddress)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.PhysicalAddressCity)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.PhysicalAddressStateId).HasColumnName("PhysicalAddressStateID");

                entity.Property(e => e.PhysicalAddressZipCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Ranking)
                    .HasColumnName("ranking")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.RegionId).HasColumnName("RegionID");

                entity.Property(e => e.RetailerLicense)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.RetailerSalesTaxCertificate)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ShippingAddress)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.ShippingAddressCity)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ShippingAddressStateId).HasColumnName("ShippingAddressStateID");

                entity.Property(e => e.ShippingAddressZipCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StateTobaccoLicense)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.StoreCount).HasDefaultValueSql("((0))");

                entity.Property(e => e.StoreType)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SugarCustomerId)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.SupplyChainId)
                    .HasColumnName("SupplyChainID")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.TaxStatement).HasMaxLength(500);

                entity.Property(e => e.Team).HasDefaultValueSql("((0))");

                entity.Property(e => e.TerritoryId).HasColumnName("TerritoryID");

                entity.Property(e => e.TradeType)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.VarianceMtd)
                    .HasColumnName("VarianceMTD")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.VarianceYtd)
                    .HasColumnName("VarianceYTD")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.YtdcasesCurrentYear).HasColumnName("YTDCasesCurrentYear");

                entity.Property(e => e.YtdcasesLastYear).HasColumnName("YTDCasesLastYear");

                entity.Property(e => e.YtdcurrentYear)
                    .HasColumnName("YTDCurrentYear")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.YtdlastYear)
                    .HasColumnName("YTDLastYear")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ZoneId).HasColumnName("ZoneID");
            });

            modelBuilder.Entity<CustomerProduct>(entity =>
            {
                entity.Property(e => e.CustomerProductId).HasColumnName("CustomerProductID");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.LastDistributionRecordDate).HasColumnType("datetime");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");
            });

            modelBuilder.Entity<DocumentEmail>(entity =>
            {
                entity.Property(e => e.DocumentEmailId).HasColumnName("DocumentEmailID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.EmailTo)
                    .HasMaxLength(2500)
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<IpadsyncData>(entity =>
            {
                entity.ToTable("IPADSyncData");

                entity.Property(e => e.AddCustomerDocument).HasDefaultValueSql("((0))");

                entity.Property(e => e.AddEditCallActivity).HasDefaultValueSql("((0))");

                entity.Property(e => e.AddEditCustomer).HasDefaultValueSql("((0))");

                entity.Property(e => e.AddEditScheduleRoute).HasDefaultValueSql("((0))");

                entity.Property(e => e.AddOrder).HasDefaultValueSql("((0))");

                entity.Property(e => e.DbfileName)
                    .HasColumnName("DBFileName")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DeleteCallActivity).HasDefaultValueSql("((0))");

                entity.Property(e => e.DeleteCustomerDocument).HasDefaultValueSql("((0))");

                entity.Property(e => e.DeleteScheduleRoutes).HasDefaultValueSql("((0))");

                entity.Property(e => e.ErrorMsg).IsUnicode(false);

                entity.Property(e => e.IsForgotPassword).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsPartialSync).HasDefaultValueSql("((0))");

                entity.Property(e => e.LastSyncUtcdate)
                    .HasColumnName("lastSyncUTCDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.LoginAccountClassification).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginBrand).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginCallActivity).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginCategory).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginCity).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginCustomer).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginCustomerDocument).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginCustomerProduct).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginOrder).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginOrderDetails).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginPassword)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.LoginProduct).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginProductDocument).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginRecordResourceType).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginRegion).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginRole).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginRouteStations).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginScheduleRoutes).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginState).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginStyle).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginSupplyChain).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginTerritory).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoginUserName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.LoginZone).HasDefaultValueSql("((0))");

                entity.Property(e => e.PartialSyncFromDateTimeSendByIpad)
                    .HasColumnName("PartialSyncFromDateTimeSendByIPAD")
                    .HasColumnType("datetime");

                entity.Property(e => e.SuccessMsg).IsUnicode(false);

                entity.Property(e => e.SyncCurrentDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.VersionNumber)
                    .HasMaxLength(150)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LnkPopitems>(entity =>
            {
                entity.ToTable("LnkPOPItems");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BrandIstyleId).HasColumnName("BrandIStyleID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<LnkRackItems>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BrandIstyleId).HasColumnName("BrandIStyleID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.IsMainRack).HasDefaultValueSql("((0))");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.Status).HasDefaultValueSql("((0))");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<NoteMaster>(entity =>
            {
                entity.HasKey(e => e.NoteId);

                entity.Property(e => e.NoteId).HasColumnName("NoteID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.DeviceNoteId)
                    .HasColumnName("DeviceNoteID")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.NoteDescription).HasColumnType("text");

                entity.Property(e => e.Subject)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.SugarNoteId)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasIndex(e => e.OrderId)
                    .HasName("OrderID");

                entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");

                entity.Property(e => e.BrandIstyleId).HasColumnName("BrandIStyleID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.CreditRequest)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.StyleId).HasColumnName("StyleID");

                entity.Property(e => e.Units)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");
            });

            modelBuilder.Entity<OrderHistoryEmail>(entity =>
            {
                entity.Property(e => e.DeviceOrderEmailId)
                    .HasColumnName("DeviceOrderEmailID")
                    .HasMaxLength(400)
                    .IsUnicode(false);

                entity.Property(e => e.DeviceOrderId)
                    .HasColumnName("DeviceOrderID")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.EmailId)
                    .HasColumnName("EmailID")
                    .HasMaxLength(400)
                    .IsUnicode(false);

                entity.Property(e => e.MemoField)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<OrderMaster>(entity =>
            {
                entity.HasKey(e => e.OrderId);

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.CustomStatement)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerComment)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerDeviceDistributorId)
                    .HasColumnName("CustomerDeviceDistributorID")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerDistributorId)
                    .HasColumnName("CustomerDistributorID")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerShippingCityId)
                    .HasColumnName("CustomerShippingCityID")
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CustomerShippingStateId).HasColumnName("CustomerShippingStateID");

                entity.Property(e => e.CustomerShippingZipCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerSignatureFilePath)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.DeviceCustomerId)
                    .HasColumnName("DeviceCustomerID")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DeviceOrderId)
                    .HasColumnName("DeviceOrderID")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.EmailRecipients)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.GrandTotal).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.InvoiceNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OpenOrderstatus).HasDefaultValueSql("((0))");

                entity.Property(e => e.OrderAddress)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.OrderCityId)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.OrderNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OrderZipCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PrebookShipDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.PrintName)
                    .HasMaxLength(400)
                    .IsUnicode(false);

                entity.Property(e => e.PurchaseOrderNumber)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.RegionId).HasColumnName("RegionID");

                entity.Property(e => e.RepublicSalesRepository)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.RetailerLicense)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.RetailerSalesTaxCertificate)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.SellerName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.SellerRepTobaccoPermitNo)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.StateTobaccoLicense)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.SugarCrmorderId)
                    .HasColumnName("SugarCRMOrderID")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.TerritoryId).HasColumnName("TerritoryID");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.ZoneId).HasColumnName("ZoneID");
            });

            modelBuilder.Entity<PositionMaster>(entity =>
            {
                entity.HasKey(e => e.PositionId);

                entity.Property(e => e.PositionId).HasColumnName("PositionID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Position)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProductAdditionalDocument>(entity =>
            {
                entity.Property(e => e.ProductAdditionalDocumentId).HasColumnName("ProductAdditionalDocumentID");

                entity.Property(e => e.DocumentFilePath)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.LangId).HasColumnName("LangID");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductAdditionalDocument)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductAdditionalDocument_ProductMaster");
            });

            modelBuilder.Entity<ProductMaster>(entity =>
            {
                entity.HasKey(e => e.ProductId);

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.BrandId).HasColumnName("BrandID");

                entity.Property(e => e.CatId).HasColumnName("CatID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.DistributionRecordedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ImportedFrom).HasDefaultValueSql("((1))");

                entity.Property(e => e.LangId).HasColumnName("LangID");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.StyleId)
                    .HasColumnName("StyleID")
                    .HasDefaultValueSql("((-1))");

                entity.Property(e => e.Uom)
                    .HasColumnName("UOM")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.HasOne(d => d.ImportedFromNavigation)
                    .WithMany(p => p.ProductMaster)
                    .HasForeignKey(d => d.ImportedFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductMaster_RecordResourceType");
            });

            modelBuilder.Entity<ProductRoleLink>(entity =>
            {
                entity.Property(e => e.ProductRoleLinkId).HasColumnName("ProductRoleLinkID");

                entity.Property(e => e.CatBrandProductId).HasColumnName("CatBrandProductID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.TerritoryId)
                    .IsRequired()
                    .HasColumnName("TerritoryID")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Type).HasDefaultValueSql("((3))");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");
            });

            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.Property(e => e.PromotionId).HasColumnName("PromotionID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Effective).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Payment1Amount)
                    .HasColumnName("Payment1_Amount")
                    .HasColumnType("decimal(26, 6)");

                entity.Property(e => e.Payment1CheckNo).HasColumnName("Payment1_CheckNo");

                entity.Property(e => e.Payment1Date)
                    .HasColumnName("Payment1_Date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Payment1Request).HasColumnName("Payment1_Request");

                entity.Property(e => e.Payment2Amount)
                    .HasColumnName("Payment2_Amount")
                    .HasColumnType("decimal(26, 6)");

                entity.Property(e => e.Payment2CheckNo).HasColumnName("Payment2_CheckNo");

                entity.Property(e => e.Payment2Date)
                    .HasColumnName("Payment2_Date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Payment2Request).HasColumnName("Payment2_Request");

                entity.Property(e => e.Plan)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.SugarId)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Year)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RackCategoryDetail>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.ItemNumber)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<RackCategoryMaster>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<RackImages>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<RankMaster>(entity =>
            {
                entity.HasKey(e => e.RankId);

                entity.Property(e => e.RankId).HasColumnName("RankID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Rank)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<RecordResourceType>(entity =>
            {
                entity.HasKey(e => e.ResourceTypeId);

                entity.Property(e => e.ResourceTypeId).HasColumnName("ResourceTypeID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.ResourceName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");
            });

            modelBuilder.Entity<RegionMaster>(entity =>
            {
                entity.HasKey(e => e.RegionId);

                entity.Property(e => e.RegionId).HasColumnName("RegionID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ImportedFrom).HasDefaultValueSql("((2))");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Regioname)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.SugarRegionId)
                    .HasColumnName("Sugar_RegionId")
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Sugar_RegionId')");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.ZoneId).HasColumnName("ZoneID");
            });

            modelBuilder.Entity<RoleMaster>(entity =>
            {
                entity.HasKey(e => e.RoleId);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.Description)
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.RoleMasterCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.RoleMasterUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy);
            });

            modelBuilder.Entity<RouteStations>(entity =>
            {
                entity.HasKey(e => e.RouteStationId);

                entity.Property(e => e.RouteStationId).HasColumnName("RouteStationID");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<SalesDocument>(entity =>
            {
                entity.Property(e => e.SalesDocumentId).HasColumnName("SalesDocumentID");

                entity.Property(e => e.BrandId).HasColumnName("BrandID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.ItemDescription).IsUnicode(false);

                entity.Property(e => e.ItemName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.OriginalFileName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.SalesDocumentPath)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.StyleId).HasColumnName("StyleID");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");
            });

            modelBuilder.Entity<ScheduleRoutes>(entity =>
            {
                entity.HasKey(e => e.RouteId);

                entity.Property(e => e.RouteId).HasColumnName("RouteID");

                entity.Property(e => e.AddressName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.AssignTsmid).HasColumnName("AssignTSMID");

                entity.Property(e => e.City)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.DeviceRouteId)
                    .HasColumnName("DeviceRouteID")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.HouseNumber)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Latitude)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Longitude)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PlannedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RouteDescription)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.RouteName)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.RouteType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.StreetName)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.SugarIds)
                    .HasColumnName("Sugar_Ids")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.ZipCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<StateMaster>(entity =>
            {
                entity.HasKey(e => e.StateId);

                entity.Property(e => e.StateId).HasColumnName("StateID");

                entity.Property(e => e.StateCode).IsUnicode(false);

                entity.Property(e => e.StateName)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.TaxStatement).HasMaxLength(500);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");
            });

            modelBuilder.Entity<StyleMaster>(entity =>
            {
                entity.HasKey(e => e.StyleId);

                entity.Property(e => e.StyleId).HasColumnName("StyleID");

                entity.Property(e => e.ImageFilePath)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.LangId).HasColumnName("LangID");

                entity.Property(e => e.ParentId).HasColumnName("ParentID");

                entity.Property(e => e.PromoId).HasColumnName("PromoID");

                entity.Property(e => e.StyleName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");
            });

            modelBuilder.Entity<SugarUpdated>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.ServiceName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SupplyChain>(entity =>
            {
                entity.ToTable("Supply_Chain");

                entity.Property(e => e.SupplyChainId).HasColumnName("SupplyChainID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.CustomerParentId).HasColumnName("CustomerParentID");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");
            });

            modelBuilder.Entity<TerritoryMaster>(entity =>
            {
                entity.HasKey(e => e.TerritoryId);

                entity.Property(e => e.TerritoryId).HasColumnName("TerritoryID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.ImportedFrom).HasDefaultValueSql("((2))");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.RegionId).HasColumnName("RegionID");

                entity.Property(e => e.SugarTerritoryId)
                    .HasColumnName("Sugar_TerritoryId")
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Sugar_TerritoryId')");

                entity.Property(e => e.TerritoryName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Type).HasMaxLength(100);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");
            });

            modelBuilder.Entity<UserApplicationDetails>(entity =>
            {
                entity.HasKey(e => e.UserApplicationDetailId);

                entity.Property(e => e.UserApplicationDetailId).HasColumnName("UserApplicationDetailID");

                entity.Property(e => e.ApplicationVersion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.DeviceToken).HasMaxLength(200);

                entity.Property(e => e.DeviceUniqueId).HasMaxLength(200);

                entity.Property(e => e.Iosversion)
                    .HasColumnName("IOSVersion")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastLoginDate).HasColumnType("datetime");

                entity.Property(e => e.LastSyncDate).HasColumnType("datetime");

                entity.Property(e => e.NotificationCode).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<UserCustomer>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__UserCust__1788CC4C75727316");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.CustomerId)
                    .IsRequired()
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserMaster>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.ContactNo)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.CustomerTerritoryChangeDate).HasColumnType("datetime");

                entity.Property(e => e.DocumentZipfilePath)
                    .HasColumnName("DocumentZIPFilePath")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.EmailId)
                    .HasColumnName("EmailID")
                    .HasMaxLength(400)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.LastTerritoryChangeDate).HasColumnType("datetime");

                entity.Property(e => e.Location)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.ManagerId).HasColumnName("ManagerID");

                entity.Property(e => e.ParentTerritoryChangeDate).HasColumnType("datetime");

                entity.Property(e => e.Pin)
                    .HasColumnName("PIN")
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(CONVERT([int],left(CONVERT([nvarchar](15),CONVERT([bigint],rand()*(1000000000000.),(0)),(0)),(4)),(0)))");

                entity.Property(e => e.RegionId).HasColumnName("RegionID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.SellerRepresentativeTobaccoPermitNo)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Seller')");

                entity.Property(e => e.SugarUserId)
                    .HasColumnName("Sugar_UserId")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.TaxStatement).HasMaxLength(500);

                entity.Property(e => e.TerritoryId)
                    .IsRequired()
                    .HasColumnName("TerritoryID")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.UfileNameLastSyncDate)
                    .HasColumnName("UFileNameLastSyncDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.UserFileName)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ZoneId).HasColumnName("ZoneID");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserMaster)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserMaster_RoleMaster");
            });

            modelBuilder.Entity<UserMasterwithSugar>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.ContactNo)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.EmailId)
                    .HasColumnName("EmailID")
                    .HasMaxLength(400)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Location)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.ManagerId).HasColumnName("ManagerID");

                entity.Property(e => e.Pin)
                    .HasColumnName("PIN")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.RegionId).HasColumnName("RegionID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.SellerRepresentativeTobaccoPermitNo)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Seller')");

                entity.Property(e => e.SugarUserId)
                    .IsRequired()
                    .HasColumnName("Sugar_UserId")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.TerritoryId)
                    .IsRequired()
                    .HasColumnName("TerritoryID")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.UserName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ZoneId).HasColumnName("ZoneID");
            });

            modelBuilder.Entity<UserTaxStatement>(entity =>
            {
                entity.Property(e => e.UserTaxStatementId).HasColumnName("UserTaxStatementID");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DeviceUserTaxStatementId)
                    .HasColumnName("DeviceUserTaxStatementID")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<ZoneMaster>(entity =>
            {
                entity.HasKey(e => e.ZoneId);

                entity.Property(e => e.ZoneId).HasColumnName("ZoneID");

                entity.Property(e => e.ImportedFrom).HasDefaultValueSql("((2))");

                entity.Property(e => e.SugarZoneId)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("([dbo].[GetCDate]())");

                entity.Property(e => e.ZoneName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AVPMaster>(entity =>
            {
                entity.HasKey(e => e.AVPID);

                entity.Property(e => e.AVPName)
                        .IsRequired()
                        .HasMaxLength(200);

                entity.Property(e => e.UpdateDate)
                    .HasDefaultValueSql("([dbo].[GetCDate]())")
                    .IsRequired();

                entity.Property(e => e.IsDeleted)
                    .HasDefaultValue(false)
                    .IsRequired();

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true)
                    .IsRequired();
            });

            modelBuilder.Entity<BDMaster>(entity =>
            {
                entity.HasKey(e => e.BDID);

                entity.Property(e => e.BDName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.UpdateDate)
                    .HasDefaultValueSql("([dbo].[GetCDate]())")
                    .IsRequired();

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("((0))")
                    .IsRequired();

                entity.Property(e => e.CreatedBy)
                    .IsRequired();

                entity.Property(e => e.UpdatedBy)
                    .IsRequired(false);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true)
                    .IsRequired();

                entity.Property(e => e.IsDeleted)
                    .HasDefaultValue(false)
                    .IsRequired();

                entity.Property(e => e.Approver)
                    .HasDefaultValue(false)
                    .IsRequired();

                entity.ToTable("BDMaster");
            });
        }
    }
}
