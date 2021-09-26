using WorldManagementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorldManagementApi.Services.Interfaces
{
    public interface IPeopleService
    {
        public Person Add(Person person);
        public Person Update(Person person);
    }
}
