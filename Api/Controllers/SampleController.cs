using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldManagementApi.Services;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetAll()
        {
            return Ok(_worldService.GetAll());
        }
    }
}
