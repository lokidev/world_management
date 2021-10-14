using WorldManagementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorldManagementApi.Services.Interfaces
{
    public interface IWorldService
    {
        ICollection<Person> GetPeople();
        int GetPeopleCount();
        DateTime GetCurrentDate();
        DateTime StartClock();
        DateTime StopClock();
    }
}
