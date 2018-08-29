﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Extensions.Options;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly CustomSettings _settings;
        public ValuesController(IOptions<CustomSettings> settings)
        {
            _settings = settings.Value;
        }
        
        // GET api/values
        [HttpGet]
        public ActionResult Get()
        {
            //String fileStr = System.IO.File.ReadAllText(@"C:\Users\wadmin\Documents\Visual Studio 2017\Projects\minecraftK8sAdminAPI\minecraftK8sAdminAPI\services.json");

            //JObject o = JObject.Parse(fileStr);

            //foreach (var itemJson in o.SelectTokens("$.items[*]"))
            //{
            //    string tenant = itemJson.SelectToken("$.metadata.name").ToString();
            //    if (tenant != "kubernetes")
            //    {
            //        JToken ipJson = itemJson.SelectToken("$.status.loadBalancer.ingress[0].ip");
            //        if (ipJson != null)
            //        {
            //            string ip = ipJson.ToString();
            //        }
            //    }

            //}


            return Ok();
        }
        
        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            return Ok();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
