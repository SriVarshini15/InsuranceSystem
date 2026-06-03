using Microsoft.EntityFrameworkCore;
using VehicleInsuranceSystem.Models.Users;
using VehicleInsuranceSystem.Models.Vehicles;
using VehicleInsuranceSystem.Models.Policies;
using VehicleInsuranceSystem.Models.Proposals;
using VehicleInsuranceSystem.Models.Payments;
using VehicleInsuranceSystem.Models.Claims;
using VehicleInsuranceSystem.Models.UserPolicies;
using VehicleInsuranceSystem.Models.Notifications;

namespace VehicleInsuranceSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        #region User Module

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        #endregion

        #region Vehicle Module

        public DbSet<Vehicle> Vehicles { get; set; }

        #endregion

        #region Policy Module

        public DbSet<Policy> Policies { get; set; }

        public DbSet<PolicyCategory> PolicyCategories { get; set; }

        public DbSet<PolicyFeature> PolicyFeatures { get; set; }

        public DbSet<PolicyAddon> PolicyAddons { get; set; }

        #endregion

        #region Proposal Module

        public DbSet<Proposal> Proposals { get; set; }

        public DbSet<ProposalDocument> ProposalDocuments { get; set; }

        #endregion

        #region Payment Module

        public DbSet<Payment> Payments { get; set; }

        #endregion

        #region Active Policy Module

        public DbSet<UserPolicy> UserPolicies { get; set; }

        #endregion

        #region Claim Module

        public DbSet<Claim> Claims { get; set; }

        public DbSet<ClaimDocument> ClaimDocuments { get; set; }

        #endregion

        #region Notification Module

        public DbSet<Notification> Notifications { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureUser(builder);
            ConfigureRefreshToken(builder);
            ConfigureVehicle(builder);
            ConfigurePolicy(builder);
            ConfigureProposal(builder);
            ConfigurePayment(builder);
            ConfigureUserPolicy(builder);
            ConfigureClaim(builder);
            ConfigureNotification(builder);

            SeedRoles(builder);
            SeedPolicyCategories(builder);
        }

        #region User Configuration

        private static void ConfigureUser(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            builder.Entity<User>()
                .HasIndex(x => x.AadhaarNumber)
                .IsUnique();

            builder.Entity<User>()
                .HasIndex(x => x.PANNumber)
                .IsUnique();

            builder.Entity<Role>()
                .HasIndex(x => x.RoleName)
                .IsUnique();

            builder.Entity<RefreshToken>()
                .HasIndex(x => x.Token)
                .IsUnique();

            builder.Entity<User>()
                .HasOne(x => x.Role)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<User>()
                .Property(x => x.Email)
                .IsRequired();

            builder.Entity<User>()
                .Property(x => x.AadhaarNumber)
                .IsRequired();

            builder.Entity<User>()
                .Property(x => x.PANNumber)
                .IsRequired();

            builder.Entity<Role>()
                .Property(x => x.RoleName)
                .IsRequired();
        }
        private static void ConfigureRefreshToken(ModelBuilder builder)
        {
            builder.Entity<RefreshToken>()
                .HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        #endregion

        #region Vehicle Configuration

        private static void ConfigureVehicle(ModelBuilder builder)
        {
            builder.Entity<Vehicle>()
                .HasIndex(x => x.VehicleNumber)
                .IsUnique();

            builder.Entity<Vehicle>()
                .HasIndex(x => x.EngineNumber)
                .IsUnique();

            builder.Entity<Vehicle>()
                .HasIndex(x => x.ChassisNumber)
                .IsUnique();

            builder.Entity<Vehicle>()
                .HasOne(x => x.User)
                .WithMany(x => x.Vehicles)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        #endregion

        #region Policy Configuration

        private static void ConfigurePolicy(ModelBuilder builder)
        {
            builder.Entity<Policy>()
                .HasOne(x => x.PolicyCategory)
                .WithMany(x => x.Policies)
                .HasForeignKey(x => x.PolicyCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PolicyFeature>()
                .HasOne(x => x.Policy)
                .WithMany(x => x.Features)
                .HasForeignKey(x => x.PolicyId);

            builder.Entity<PolicyAddon>()
                .HasOne(x => x.Policy)
                .WithMany(x => x.Addons)
                .HasForeignKey(x => x.PolicyId);

            builder.Entity<PolicyCategory>()
                .Property(x => x.CategoryName)
                .IsRequired();
        }

        #endregion

        #region Proposal Configuration

        private static void ConfigureProposal(ModelBuilder builder)
        {
            builder.Entity<Proposal>()
                .HasIndex(x => x.ProposalNumber)
                .IsUnique();

            builder.Entity<Proposal>()
                .HasOne(x => x.User)
                .WithMany(x => x.Proposals)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Proposal>()
                .HasOne(x => x.Vehicle)
                .WithMany()
                .HasForeignKey(x => x.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Proposal>()
                .HasOne(x => x.Policy)
                .WithMany()
                .HasForeignKey(x => x.PolicyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProposalDocument>()
                .HasOne(x => x.Proposal)
                .WithMany(x => x.ProposalDocuments)
                .HasForeignKey(x => x.ProposalId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Proposal>()
                .Property(x => x.PremiumAmount)
                .HasPrecision(18, 2);

        }

        #endregion

        #region Payment Configuration

        private static void ConfigurePayment(ModelBuilder builder)
        {
            builder.Entity<Payment>()
                .HasIndex(x => x.TransactionNumber)
                .IsUnique();

            builder.Entity<Payment>()
                .HasOne(x => x.Proposal)
                .WithOne()
                .HasForeignKey<Payment>(x => x.ProposalId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payment>()
                .HasIndex(x => x.ProposalId)
                .IsUnique();

            builder.Entity<Payment>()
                .Property(x => x.Amount)
                .HasPrecision(18, 2);
        }

        #endregion

        #region User Policy Configuration

        private static void ConfigureUserPolicy(ModelBuilder builder)
        {
            builder.Entity<UserPolicy>()
                .HasIndex(x => x.PolicyNumber)
                .IsUnique();

            builder.Entity<UserPolicy>()
                .HasOne(x => x.Proposal)
                .WithOne()
                .HasForeignKey<UserPolicy>(x => x.ProposalId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserPolicy>()
                .HasIndex(x => x.ProposalId)
                .IsUnique();

            builder.Entity<Policy>()
                .Property(x => x.BasePremium)
                .HasPrecision(18, 2);

            builder.Entity<Policy>()
                .Property(x => x.CoverageAmount)
                .HasPrecision(18, 2);

            builder.Entity<PolicyAddon>()
                .Property(x => x.AddonCost)
                .HasPrecision(18, 2);
        }

        #endregion

        #region Claim Configuration

        private static void ConfigureClaim(ModelBuilder builder)
        {
            builder.Entity<Claim>()
                .HasIndex(x => x.ClaimNumber)
                .IsUnique();

            builder.Entity<Claim>()
                .HasOne(x => x.UserPolicy)
                .WithMany(x => x.Claims)
                .HasForeignKey(x => x.UserPolicyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ClaimDocument>()
                .HasOne(x => x.Claim)
                .WithMany(x => x.ClaimDocuments)
                .HasForeignKey(x => x.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Claim>()
                .Property(x => x.ClaimAmount)
                .HasPrecision(18, 2);
        }

        #endregion

        #region Notification Configuration

        private static void ConfigureNotification(ModelBuilder builder)
        {
            builder.Entity<Notification>()
                .HasOne(x => x.User)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }


        #endregion

        #region Seed Data

        private static void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<Role>().HasData(
                new Role
                {
                    RoleId = 1,
                    RoleName = "Admin"
                },
                new Role
                {
                    RoleId = 2,
                    RoleName = "Officer"
                },
                new Role
                {
                    RoleId = 3,
                    RoleName = "User"
                });
        }

        private static void SeedPolicyCategories(ModelBuilder builder)
        {
            builder.Entity<PolicyCategory>().HasData(
                new PolicyCategory
                {
                    PolicyCategoryId = 1,
                    CategoryName = "Car",
                    Description = "Car Insurance"
                },
                new PolicyCategory
                {
                    PolicyCategoryId = 2,
                    CategoryName = "Bike",
                    Description = "Bike Insurance"
                },
                new PolicyCategory
                {
                    PolicyCategoryId = 3,
                    CategoryName = "Truck",
                    Description = "Truck Insurance"
                },
                new PolicyCategory
                {
                    PolicyCategoryId = 4,
                    CategoryName = "Camper Van",
                    Description = "Camper Van Insurance"
                });
        }

        #endregion
    }
}