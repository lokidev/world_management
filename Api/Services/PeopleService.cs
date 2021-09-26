using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldManagementApi.Repos;
using WorldManagementApi.Messaging.Interfaces;
using Microsoft.Extensions.Configuration;
using WorldManagementApi.Messaging.Configurations;
using WorldManagementApi.Services.Interfaces;
using WorldManagementApi.Models;

namespace WorldManagementApi.Services
{
    public class PeopleService : IPeopleService
    {
        private IRabbitMqService mRabbitMqService;
        private readonly IConfiguration _configuration;

        public PeopleService(IConfiguration configuration, IRabbitMqService rabbitMqService)
        {
            _configuration = configuration;
            mRabbitMqService = rabbitMqService;
        }

        public Person Add(Person person)
        {
            using (var db = new WorldContext(_configuration))
            {
                var p = new PeopleRepo(db);
                var addedPerson = p.AddPerson(person);
                mRabbitMqService.sendMessage(person, "people_exchange_main.person.created", true);
                return addedPerson;
            }
        }

        public Person Update(Person person)
        {
            using (var db = new WorldContext(_configuration))
            {
                var p = new PeopleRepo(db);
                var updatedPerson = p.UpdatePerson(person);
                mRabbitMqService.sendMessage(person, "people_exchange_main.person.updated", true);
                return updatedPerson;
            }
        }
    }
}
