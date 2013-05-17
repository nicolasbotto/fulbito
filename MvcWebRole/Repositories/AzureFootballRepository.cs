using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcWebRole.Repositories
{
    public class AzureFootballRepository : IFootballRepository
    {
        private FootballPlannerContext context;
        
        public AzureFootballRepository()
        {
            context = new FootballPlannerContext();
        }

        public IEnumerable<Models.Match> GetMatches()
        {
            return context.Matches;
        }

        public IEnumerable<Models.Player> GetPlayers()
        {
            return context.Players;
        }

        public Models.Match GetMatch(int id)
        {
            var match = context.Matches.SingleOrDefault(x => x.Id == id);

            if (match == null)
            {
                throw new ArgumentException("invalid id");
            }

            return match;
        }

        public void AddMatch(Models.Match match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }

            context.Matches.Add(match);
            context.SaveChanges();
        }

        public void AddPlayer(Models.Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }

            if (!context.Players.Any(x => x.FacebookId == player.FacebookId))
            {
                context.Players.Add(player);
                context.SaveChanges();
            }
        }

        public void AddMatchPlayer(Models.Match match, Models.Player player)
        {
            var matchToAdd = context.Matches.SingleOrDefault(x => x.Id == match.Id);

            if (matchToAdd == null)
            {
                throw new ArgumentException("invalid match");
            }

            var playerToAdd = context.Players.SingleOrDefault(x => x.FacebookId == player.FacebookId);

            matchToAdd.Players.Add(playerToAdd);
            context.SaveChanges();
        }
    }
}