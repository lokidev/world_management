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
        private WorldContext mWorldContext;

        public PeopleService(IConfiguration configuration, IRabbitMqService rabbitMqService)
        {
            _configuration = configuration;
            mRabbitMqService = rabbitMqService;
            mWorldContext = new WorldContext(_configuration);
        }

        public Person Add(Person person)
        {
            var p = new PeopleRepo(mWorldContext);
            var addedPerson = p.AddPerson(person);
            mRabbitMqService.sendMessage(person, "world_exchange_main.person.created", true);
            return addedPerson;
        }

        public Person Update(Person person)
        {
            var p = new PeopleRepo(mWorldContext);
            var updatedPerson = p.UpdatePerson(person);
            mRabbitMqService.sendMessage(person, "world_exchange_main.person.updated", true);
            return updatedPerson;
        }
    }
}
