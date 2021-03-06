﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using static Dalisama.Extensions.Configuration.Consul.Example.Startup;

namespace Dalisama.Extensions.Configuration.Consul.Example.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExampleRawController : ControllerBase
    {

        [HttpGet]
        public List<Keys> Get([FromServices] IOptionsSnapshot<RawKeys> option1, [FromServices] IOptions<RawKeys> option2)
        {
            return new List<Keys> { option1.Value, option2.Value };
        }
    }
}
