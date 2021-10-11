using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldManagementApi.Services;
using Microsoft.AspNetCore.Mvc;
using WorldManagementApi.Services.Interfaces;

namespace WorldManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorldController : ControllerBase
    {
        private IWorldService _worldService;

        public WorldController(IWorldService worldService)
        {
            _worldService = worldService;
        }

        [HttpGet]
        [Route("people")]
        public IActionResult GetPeople()
        {
            return Ok(_worldService.GetPeople());
        }

        [HttpGet]
        [Route("peopleCount")]
        public IActionResult GetPeopleCount()
        {
            return Ok(_worldService.GetPeopleCount());
        }

        [HttpGet]
        [Route("currentDate")]
        public IActionResult GetCurrentDate()
        {
            return Ok(_worldService.GetCurrentDate());
        }

        [HttpGet]
        [Route("startClock")]
        public IActionResult StartClock()
        {
            return Ok(_worldService.StartClock());
        }

        [HttpGet]
        [Route("stopClock")]
        public IActionResult StopClock()
        {
            return Ok(_worldService.StopClock());
        }
    }
}
