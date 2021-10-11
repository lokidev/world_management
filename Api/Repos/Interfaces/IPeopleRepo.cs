using WorldManagementApi.Models;
using System;
using System.Collections.Generic;

namespace WorldManagementApi.Repos
{
    public interface IPeopleRepo
    {
        Person GetPerson(int id);
        List<Person> GetPeople();
        Person AddPerson(Person person);
        Person UpdatePerson(Person person);
    }
}
