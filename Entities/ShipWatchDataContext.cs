using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
  public partial class ShipWatchDataContext : DbContext
  {
    public ShipWatchDataContext()
    {
    }

    public ShipWatchDataContext(DbContextOptions<ShipWatchDataContext> options)
        : base(options)
    {
    }

        public virtual DbSet<TbVesselBunkerStemsAndSurveysRawDatum> TbVesselBunkerStemsAndSurveysRawData { get; set; } = null!;
        public virtual DbSet<TbVesselBunkerTankRawDatum> TbVesselBunkerTankRawData { get; set; } = null!;
        public virtual DbSet<TbVesselForobAllocationRawDatum> TbVesselForobAllocationRawData { get; set; } = null!;
        public virtual DbSet<TbVesselForobRawDatum> TbVesselForobRawData { get; set; } = null!;
        public virtual DbSet<TbVesselMetaDatum> TbVesselMetaData { get; set; } = null!;
        public virtual DbSet<TbVesselRawDatum> TbVesselRawData { get; set; } = null!;
        public virtual DbSet<TbVesselUpcomingPortRawDatum> TbVesselUpcomingPortRawData { get; set; } = null!;
        public virtual DbSet<TbVesselsAuxilliaryRawDatum> TbVesselsAuxilliaryRawData { get; set; } = null!;
        public virtual DbSet<TbCargoHandlingDatum> TbCargoHandlingData { get; set; } = null!;
        public virtual DbSet<TbStowagesDatum> TbStowagesData { get; set; } = null!;
        public virtual DbSet<TbPortActivities> TbPortActivitiesData { get; set; } = null!;
        public virtual DbSet<TbServiceLogs> TbServiceLogs { get; set; } = null!;
        public virtual DbSet<TbServiceNotifications> TbServiceNotifications { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<TbVesselBunkerStemsAndSurveysRawDatum>(entity =>
      {
        entity.HasOne(d => d.VesselRawData)
                  .WithMany(p => p.TbVesselBunkerStemsAndSurveysRawData)
                  .HasForeignKey(d => d.VesselRawDataId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_tb_Vessel_BunkerStemsAndSurveysRawData_tb_VesselRawData");
      });

      modelBuilder.Entity<TbVesselBunkerTankRawDatum>(entity =>
      {
        entity.HasOne(d => d.VesselDataRaw)
                  .WithMany(p => p.TbVesselBunkerTankRawData)
                  .HasForeignKey(d => d.VesselDataRawId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_tb_Vessel_BunkerTankRawData_tb_VesselRawData");
      });

      modelBuilder.Entity<TbVesselForobAllocationRawDatum>(entity =>
      {
        entity.HasOne(d => d.VesselForobRawData)
                  .WithMany(p => p.TbVesselForobAllocationRawData)
                  .HasForeignKey(d => d.VesselForobRawDataId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_tb_Vessel_Forob_AllocationRawData_tb_Vessel_ForobRawData");
      });

      modelBuilder.Entity<TbVesselForobRawDatum>(entity =>
      {
        entity.HasOne(d => d.VesselDataRaw)
                  .WithMany(p => p.TbVesselForobRawData)
                  .HasForeignKey(d => d.VesselDataRawId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_tb_Vessel_ForobRawData_tb_VesselRawData");
      });

      modelBuilder.Entity<TbVesselMetaDatum>(entity =>
      {
        entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");
      });

      modelBuilder.Entity<TbVesselRawDatum>(entity =>
      {
        entity.Property(e => e.GeographyLocation).HasColumnType("geography");

        entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

        entity.Property(e => e.GeographyLocation).HasColumnType("geography");
        entity.Property(e => e.IsNdrProcessed).HasDefaultValueSql("((0))");

        entity.Property(e => e.IsUnifiedMetricsProcessed).HasDefaultValueSql("((0))");

        entity.Property(e => e.IsVrsProcessed).HasDefaultValueSql("((0))");
      });

      modelBuilder.Entity<TbVesselUpcomingPortRawDatum>(entity =>
      {
        entity.HasOne(d => d.VesselDataRaw)
                  .WithMany(p => p.TbVesselUpcomingPortRawData)
                  .HasForeignKey(d => d.VesselDataRawId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_tb_Vessel_UpcomingPortRawData_tb_VesselRawData");
      });

      modelBuilder.Entity<TbVesselsAuxilliaryRawDatum>(entity =>
      {
        entity.HasOne(d => d.VesselDataRaw)
                  .WithMany(p => p.TbVesselsAuxilliaryRawData)
                  .HasForeignKey(d => d.VesselDataRawId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_tb_Vessels_AuxilliaryRawData_tb_VesselRawData");
      });

            modelBuilder.Entity<TbCargoHandlingDatum>(entity =>
            {
                entity.HasOne(d => d.VesselDataRaw)
                    .WithMany(p => p.TbCargoHandlingData)
                    .HasForeignKey(d => d.VesselDataRawId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tb_CargoHandling_tb_VesselRawData");
            });
            modelBuilder.Entity<TbStowagesDatum>(entity =>
            {
                entity.HasOne(d => d.CargoHandlingData)
                    .WithMany(p => p.TbStowagesData)
                    .HasForeignKey(d => d.CargoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tb_Stowage_tb_VesselRawData");
            });
            modelBuilder.Entity<TbPortActivities>(entity =>
            {
                entity.HasOne(d => d.VesselDataRaw)
                    .WithMany(p => p.TbPortActivitiesData)
                    .HasForeignKey(d => d.VesselDataRawID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tb_PortActivities_tb_VesselRawData");
            });

      OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
  }
}