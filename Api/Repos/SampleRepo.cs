using WorldManagementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldManagementApi.Repos
{
    public class WorldRepo
    {
        private WorldContext db;

        public WorldRepo(WorldContext db)
        {
            this.db = db;
        }

        public List<World> GetProduts()
        {
            if (db != null)
            {
                List<World> employees = new List<World>();

                var result = db.Worlds.OrderByDescending(x => x.WorldName).ToList();

                return result;
            }

            return null;
        }
    }
}
