using MvcWebRole.Models;
using MvcWebRole.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MvcWebRole.Controllers
{
    public class MatchController : ApiController
    {
        private IFootballRepository repository;

        public MatchController() :
            this(new AzureFootballRepository())
        {

        }

        public MatchController(IFootballRepository repository)
        {
            this.repository = repository;
        }
        // GET api/match
        public IEnumerable<Match> GetMatches()
        {
            return repository.GetMatches();
        }

        // GET api/match/5
        public Match Get(int id)
        {
            return repository.GetMatch(id);
        }

        // POST api/match
        public HttpResponseMessage Post(Match match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }

            repository.AddMatch(match);

            var response = Request.CreateResponse<Match>(HttpStatusCode.Created, match);

            string uri = Url.Link("DefaultApi", new { id = match.Id });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        // PUT api/match/5
        public void Put(int id, Player player)
        {
            repository.AddMatchPlayer(new Match() { Id = id }, player);
        }

        // DELETE api/match/5
        public void Delete(int id)
        {
        }
    }
}
