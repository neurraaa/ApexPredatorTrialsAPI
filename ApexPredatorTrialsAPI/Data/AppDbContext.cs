using Microsoft.EntityFrameworkCore;
using ApexPredatorTrialsAPI.Models;

namespace ApexPredatorTrialsAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerStats> PlayersStatistics { get; set; }
        public DbSet<GameMap> Maps { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchResults> MatchResults { get; set; }
        public DbSet<GameEvent> Events { get; set; }
        public DbSet<GameEventRegistration> EventRegistrations { get; set; }
        public DbSet<GameEventSchedule> Schedules { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<IpAddress> IpAddresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>()
                .HasOne(p => p.User)
                .WithOne(u => u.Player)
                .HasForeignKey<Player>(p => p.UserId);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.PlayerStats)
                .WithOne(ps => ps.Player)
                .HasForeignKey<Player>(p => p.PlayerStatsId);

            modelBuilder.Entity<GameEvent>()
                .HasOne(e => e.Organizer)
                .WithMany(u => u.OrganizedEvents)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GameEventRegistration>()
                .HasOne(er => er.Event)
                .WithMany(e => e.EventRegistrations)
                .HasForeignKey(er => er.EventId);

            modelBuilder.Entity<GameEventRegistration>()
                .HasOne(er => er.Player)
                .WithMany(p => p.EventRegistrations)
                .HasForeignKey(er => er.PlayerId);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Event)
                .WithMany(e => e.Matches)
                .HasForeignKey(m => m.EventId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Map)
                .WithMany(map => map.Matches)
                .HasForeignKey(m => m.MapId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.HunterPlayer)
                .WithMany(p => p.HunterMatches)
                .HasForeignKey(m => m.HunterPlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.HumanPlayer)
                .WithMany(p => p.HumanMatches)
                .HasForeignKey(m => m.HumanPlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MatchResults>()
                .HasOne(mr => mr.Match)
                .WithOne(m => m.MatchResults)
                .HasForeignKey<MatchResults>(mr => mr.MatchId);

            modelBuilder.Entity<MatchResults>()
                .HasOne(mr => mr.Winner)
                .WithMany()
                .HasForeignKey(mr => mr.WinnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MatchResults>()
                .HasOne(mr => mr.Loser)
                .WithMany()
                .HasForeignKey(mr => mr.LoserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GameEventSchedule>()
                .HasOne(s => s.Event)
                .WithMany(e => e.Schedules)
                .HasForeignKey(s => s.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GameEventSchedule>()
                .HasOne(s => s.Match)
                .WithOne(m => m.Schedule)
                .HasForeignKey<GameEventSchedule>(s => s.MatchId);

            modelBuilder.Entity<GameEventSchedule>()
                .HasOne(s => s.NextSchedule)
                .WithMany(s => s.PreviousSchedules)
                .HasForeignKey(s => s.NextScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Log>()
                .HasIndex(l => new { l.ClientAddress, l.Timestamp });

            modelBuilder.Entity<IpAddress>()
                .HasKey(i => i.Address);
        }
    }
}
