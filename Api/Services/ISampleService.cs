using WorldManagementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorldManagementApi.Services
{
    public interface IWorldService
    {
        public ICollection<World> GetAll();
    }
}
