using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcWebRole.Models
{
    public class Player
    {
        public int Id { get; set; }
        public long FacebookId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Match> Matches { get; set; }
    }
}