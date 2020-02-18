using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notes2021Blazor.Shared;

namespace Notes2021Blazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AboutController : ControllerBase
    {
        [HttpGet]
        public async Task<AboutModel> Get()
        {
            return new AboutModel
            {
                PrimeAdminName = Globals.PrimeAdminName,
                PrimeAdminEmail = Globals.PrimeAdminEmail,
                StartupDateTime = Globals.StartupDateTime
            };
        }
    }
}