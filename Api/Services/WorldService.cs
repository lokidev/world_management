using WorldManagementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldManagementApi.Repos;
using Microsoft.Extensions.Configuration;
using WorldManagementApi.Services.Interfaces;
using System.Threading;
using WorldManagementApi.Messaging.Interfaces;

namespace WorldManagementApi.Services
{
    public class WorldService : IWorldService
    {
        private readonly IConfiguration _configuration;
        private IRabbitMqService mRabbitMqService;
        private bool clockRunning;
        private DateTime curDate;
        private WorldContext mWorldContext;

        public WorldService(IConfiguration configuration, IRabbitMqService rabbitMqService)
        {
            _configuration = configuration;
            mRabbitMqService = rabbitMqService;
            clockRunning = false;
            curDate = DateTime.Now;
            mWorldContext = new WorldContext(_configuration);
        }

        public ICollection<World> GetAll()
        {
            var t = new WorldRepo(mWorldContext);
            return t.GetProduts();
        }

        public async Task ChangeDateAndWait()
        {
            if (clockRunning)
            {
                curDate = curDate.AddYears(1);
                mRabbitMqService.sendMessage(curDate, "world_exchange_main.time.newDay", true);
                Thread.Sleep(6000);
                _ = Task.Factory.StartNew(() => ChangeDateAndWait());
            }
        }

        public async Task StartClock()
        {
            clockRunning = true;
            _ = Task.Factory.StartNew(() => ChangeDateAndWait());
        }

        public async Task StopClock()
        {
            clockRunning = false;
        }
    }
}
