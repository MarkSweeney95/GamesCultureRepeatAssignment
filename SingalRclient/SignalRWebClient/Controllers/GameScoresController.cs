using SignalRWebClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Game2;


namespace SignalRWebClient.Controllers
{
    [RoutePrefix("api/GameScores")]
    public class GameScoresController : ApiController
    {
        [Route("getTops/Count/{Count:int}/Game/{gameName}")]
        public dynamic getTop(int Count, string gameName )
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Game g = db.Games.FirstOrDefault(game => game.GameName == gameName);
                if (g == null)
                    return BadRequest("Game does not exist");

                return 
                    (from scores in db.GameScores
                     join players in db.Users
                     on scores.PlayerID equals players.Id
                     where scores.GameID == g.GameID
                     orderby scores.score descending
                     select new { players.GamerTag, scores.score })
                     .Take(Count).ToList();
            }

        }
    }
}
