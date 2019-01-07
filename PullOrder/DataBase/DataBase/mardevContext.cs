using System;
using DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataBase
{
    public partial class mardevContext : DbContext
    {
        public mardevContext()
        {
        }

        public mardevContext(DbContextOptions<mardevContext> options)
            : base(options)
        {
        }
        public virtual DbSet<ApiUserPlatformToken> ApiUserPlatformToken { get; set; }
        public virtual DbSet<LogPullOrder> LogPullOrder { get; set; }
        public virtual DbSet<QueuePullOrder> QueuePullOrder { get; set; }
        public virtual DbSet<QueuePullOrderUserToken> QueuePullOrderUserToken { get; set; }

        // Unable to generate entity type for table 'LogApiRequest'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("Server=team-dev-mysqldb.cxkbhs9nvapa.ap-southeast-1.rds.amazonaws.com;User Id=dev-db-admin;Password=55zCPN52UhrcmkEhH5wtFahbTv2FS8Nv;Database=mar-dev");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApiUserPlatformToken>(entity =>
            {
                entity.HasIndex(e => new { e.ApiId, e.ApiUserId })
                    .HasName("IX_ApiUserPlatformToken_apiId_userId");

                entity.HasIndex(e => new { e.ApiId, e.PlatformCode, e.PlatformUserKey })
                    .HasName("UQ_ApiUserPlatformToken_apiId_platformCode_PlatformUserkey")
                    .IsUnique();

                entity.HasIndex(e => new { e.ApiId, e.ApiUserId, e.ApiUserPlatformTokenId, e.IsDeleted })
                    .HasName("IX_ApiUserPlatformToken_apiId_userId_tokenId_isDeleted");

                entity.Property(e => e.ApiUserPlatformTokenId)
                    .HasColumnName("api_user_platform_token_id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.AccessToken)
                    .IsRequired()
                    .HasColumnName("access_token")
                    .HasColumnType("text");

                entity.Property(e => e.ApiId)
                    .IsRequired()
                    .HasColumnName("api_id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.ApiUserId)
                    .IsRequired()
                    .HasColumnName("api_user_id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.CreateOn).HasColumnName("create_on");

                entity.Property(e => e.ExpirationTime).HasColumnName("expiration_time");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("is_deleted")
                    .HasColumnType("bit(1)");

                entity.Property(e => e.ModifyOn).HasColumnName("modify_on");

                entity.Property(e => e.PlatformCode)
                    .IsRequired()
                    .HasColumnName("platform_code")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.PlatformUserKey)
                    .IsRequired()
                    .HasColumnName("platform_user_key")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.PlatformUsername)
                    .IsRequired()
                    .HasColumnName("platform_username")
                    .HasColumnType("text");
            });

            modelBuilder.Entity<LogPullOrder>(entity =>
            {
                entity.HasKey(e => e.PullOrderBgplogId);

                entity.Property(e => e.PullOrderBgplogId)
                    .HasColumnName("pullorder_log_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ApiUserPlatformTokenId)
                    .HasColumnName("api_user_platform_token_id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.ProcessEndTime).HasColumnName("process_end_time").HasColumnType("datetime");

                entity.Property(e => e.ProcessMessage).HasColumnName("Process_log_message").HasColumnType("text");

                entity.Property(e => e.ProcessStartTime).HasColumnName("process_beg_time").HasColumnType("datetime");

                entity.Property(e => e.ProcessStatus).HasColumnName("process_status").HasColumnType("int(11)");
            });

            modelBuilder.Entity<QueuePullOrder>(entity =>
            {
                entity.HasKey(e => e.PullOrderQueueId);

                entity.Property(e => e.PullOrderQueueId)
                    .HasColumnName("pullorder_queue_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ApiUserPlatformTokenId)
                    .HasColumnName("apiuser_platform_token_Id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.PullOrderFromID)
                    .HasColumnName("pullorder_from_id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.OrderId)
                    .IsRequired()
                    .HasColumnName("order_Id")
                    .HasColumnType("text");

                entity.Property(e => e.SiteCode)
                    .IsRequired()
                    .HasColumnName("site_code")
                    .HasColumnType("text");
                entity.Property(e => e.IsUsed)
                    .IsRequired()
                    .HasColumnName("is_used")
                    .HasColumnType("bit(1)");
            });

            modelBuilder.Entity<QueuePullOrderUserToken>(entity =>
            {
                entity.HasKey(e => e.LoginUserTokenQueueId);

                entity.Property(e => e.LoginUserTokenQueueId)
                    .HasColumnName("user_token_queue_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ApiUserPlatformTokenId)
                    .HasColumnName("apiuser_platform_token_Id")
                    .HasColumnType("bigint(11)");

                entity.Property(e => e.NumberOfDays)
                    .HasColumnName("number_days")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IsUsed)
                   .IsRequired()
                   .HasColumnName("is_used")
                   .HasColumnType("bit(1)");

                entity.Property(e => e.OrderTotal)
                   .HasColumnName("order_total")
                   .HasColumnType("int(11)");
            });
        }
    }
}
