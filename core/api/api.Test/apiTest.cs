using api.Helpers;
using api.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Xunit;

namespace api.Test
{
    public class apiTest
    {
        private readonly KubernetesApi api;
        private readonly string name;
        public apiTest(IOptions<CustomSettings> settings)
        {
            api = new KubernetesApi(settings.Value.ApiBaseUrl, settings.Value.Authorization);
            name = $"Minecraft-UnitTest-{DateTime.Now.ToString("yyyyMMddhhmm")}";
        }

        [Fact]
        public void ShouldGet()
        {
            var instances = api.GetAsync("api/v1/namespaces/default/services").Result;
            Assert.IsType<List<Instance>>(instances);
            Assert.True(instances.Count > 0);
        }

        [Fact]
        public void ShouldPost()
        {
            var response = api.PostAsync(name).Result;
        }

        [Fact]
        public void ShouldDelete()
        {
            var response = api.DeleteAsync(name).Result;
        }
    }
}
