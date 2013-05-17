using MvcWebRole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcWebRole.Repositories
{
    public interface IFootballRepository
    {
        IEnumerable<Match> GetMatches();
        IEnumerable<Player> GetPlayers();
        Match GetMatch(int id);
        void AddMatch(Match match);
        void AddPlayer(Player player);
        void AddMatchPlayer(Match match, Player player);
    }
}