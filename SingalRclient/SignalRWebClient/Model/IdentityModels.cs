using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Data.Entity;

namespace SignalRWebClient.Model
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public int XP { get; set; }

        public string GamerTag { get; set; }
        
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    [Table("Achievement")]
    public class Achievemnent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string Name { get; set; }
    }

    [Table("PlayerAchievement")]
    public class PlayerAchievement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("achievement")]
        public int AchievementID { get; set; }
        [ForeignKey("player")]
        public string PlayerID { get; set; }
        public virtual Achievemnent achievement { get; set; }
        public virtual ApplicationUser player { get; set; }
    }

    [Table("Game")]
    public class Game
    {
        public Game()
        {
            scores = new HashSet<GameScore>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GameID { get; set; }

        public string GameName { get; set; }

        public virtual ICollection<GameScore> scores { get; set; }

    }

    public class GameScore
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScoreID { get; set; }

        [ForeignKey("game")]
        public int GameID { get; set; }

        [ForeignKey("player")]
        public string PlayerID { get; set; }

        public int score { get; set; }

        public virtual Game game { get; set; }
        public virtual ApplicationUser player { get; set; }

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<GameScore> GameScores { get; set; }

        public virtual DbSet<PlayerAchievement> PlayerAchievements { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}