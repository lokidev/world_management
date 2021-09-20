using WorldManagementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldManagementApi.Repos;
using Microsoft.Extensions.Configuration;

namespace WorldManagementApi.Services
{
    public class WorldService : IWorldService
    {
        private readonly IConfiguration _configuration;

        public WorldService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ICollection<World> GetAll()
        {
            using (var db = new WorldContext(_configuration))
            {
                var t = new WorldRepo(db);
                return t.GetProduts();
            }
        }
    }
}
