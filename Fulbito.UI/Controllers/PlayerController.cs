using Fulbito.UI.Models;
using Fulbito.UI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Fulbito.UI.Controllers
{
    public class PlayerController : ApiController
    {
        private IFootballRepository repository;

        public PlayerController() :
            this(new AzureFootballRepository())
        {

        }

        public PlayerController(IFootballRepository repository)
        {
            this.repository = repository;
        }

        // GET api/player
        public IEnumerable<Player> GetPlayers()
        {
            return repository.GetPlayers();
        }

        // GET api/player/5
        public Player Get(int id)
        {
            var player = repository.GetPlayers().SingleOrDefault(x => x.FacebookId == id);

            if (player == null)
            {
                throw new HttpException("Player not found");
            }

            return player;
        }

        // POST api/player
        public HttpResponseMessage Post(Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }

            repository.AddPlayer(player);

            var response = Request.CreateResponse<Player>(HttpStatusCode.Created, player);

            string uri = Url.Link("DefaultApi", new { id = player.Id });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        // PUT api/player/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/player/5
        public void Delete(int id)
        {
        }
    }
}
