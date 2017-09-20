using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebAPI.Model
{
    public partial class AppSecUSAContext : DbContext
    {
        public virtual DbSet<ServiceLoginLog> ServiceLoginLog { get; set; }
        public virtual DbSet<ServiceRole> ServiceRole { get; set; }
        public virtual DbSet<ServiceUser> ServiceUser { get; set; }
        public virtual DbSet<ServiceUserRoles> ServiceUserRoles { get; set; }
        public virtual DbSet<User> User { get; set; }


        public AppSecUSAContext(DbContextOptions<AppSecUSAContext> options) : base(options) { }
//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer(@"Data Source=localhost;Initial Catalog=AppSecUSA;Trusted_Connection=True;");
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServiceLoginLog>(entity =>
            {
                entity.HasKey(e => e.LogId);

                entity.ToTable("SERVICE_LOGIN_LOG");

                entity.Property(e => e.LogId).HasColumnName("LogID");

                entity.Property(e => e.Pass)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ServiceRole>(entity =>
            {
                entity.HasKey(e => e.RoleId);

                entity.ToTable("SERVICE_ROLE");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ServiceUser>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("SERVICE_USER");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FailedAttempts).HasDefaultValueSql("((0))");

                entity.Property(e => e.LockedOut).HasDefaultValueSql("((0))");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UserPassword)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ServiceUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.ToTable("SERVICE_USER_ROLES");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.ServiceUserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_service_user_roles_roleid");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ServiceUserRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_service_user_roles_userid");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USER");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FailedAttempts).HasDefaultValueSql("((0))");

                entity.Property(e => e.LockedOut).HasDefaultValueSql("((0))");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UserPassword)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);
            });
        }
    }
}
