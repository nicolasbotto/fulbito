using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fulbito.UI.Models
{
    public class Match
    {
        public Match()
        {
            Players = new List<Player>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }

        public virtual ICollection<Player> Players { get; set; }
    }
}