using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notes2021Blazor.Server.Services;
using Notes2021Blazor.Shared;

namespace Notes2021Blazor.Server.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class LinkTestController : ControllerBase
    {

        [HttpPost]
        public async Task<bool> Post(Stringy uri)
        {
            LinkProcessor lp = new LinkProcessor(null);
            return await lp.Test(uri.value);
        }

        [HttpPut]
        public async Task<bool> Put(Stringy uri)
        {
            LinkProcessor lp = new LinkProcessor(null);
            bool test = await lp.Test2(uri.value);
            return test;
        }

    }
}
