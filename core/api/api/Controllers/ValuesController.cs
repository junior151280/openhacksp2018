﻿using System.Threading.Tasks;
using api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace api.Controllers
{
    [Route("api/instances")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly CustomSettings _settings;
        private readonly KubernetesApi _api;
        public ValuesController(IOptions<CustomSettings> settings)
        {
            _settings = settings.Value;
            _api = new KubernetesApi(_settings.ApiBaseUrl, _settings.Authorization);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var instances = await _api.GetAsync("api/v1/namespaces/default/services");
            return Ok(instances);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string name)
        {
            var response = await _api.PostAsync(name);
            return Ok(response);
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete([FromBody] string name)
        {
            var response = await _api.DeleteAsync($"apis/apps/v1/namespaces/default/deployments/{name}");
            return Ok(response);
        }
    }
}