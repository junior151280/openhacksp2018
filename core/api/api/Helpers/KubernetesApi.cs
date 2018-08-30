using api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace api.Helpers
{
    public class KubernetesApi
    {
        private HttpClient _client;
        private HttpClientHandler _handler;
        private string _urlBase;
        private string _auth;

        public KubernetesApi(string urlBase, string auth)
        {
            _urlBase = urlBase;
            _auth = auth;
        }

        public async Task<List<Instance>> GetAsync(string method)
        {
            using (_handler = new HttpClientHandler())
            {
                _handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                using (_client = new HttpClient(_handler))
                {
                    _client.DefaultRequestHeaders.Accept.Clear();
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    _client.DefaultRequestHeaders.Add("Authorization", _auth);

                    string fullApiCall = $"{_urlBase}{method}";

                    var apiReturn = await _client.GetStringAsync(fullApiCall);

                    return Parse(apiReturn);
                }
            }
        }

        private List<Instance> Parse(string json)
        {
            var jsonReturn = JObject.Parse(json);
            var instances = new List<Instance>();

            foreach (var itemJson in jsonReturn.SelectTokens("$.items[*]"))
            {
                var instance = new Instance
                {
                    Name = itemJson.SelectToken("$.metadata.name").ToString()
                };
                if (instance.Name != "kubernetes")
                {
                    JToken ipJson = itemJson.SelectToken("$.status.loadBalancer.ingress[0].ip");
                    if (ipJson != null)
                    {
                        instance.Endpoints.Minecraft = $"{ipJson.ToString()}:25565";
                        instance.Endpoints.Rcon = $"{ipJson.ToString()}:25575";
                        instances.Add(instance);
                    }
                }
            }
            return instances;
        }

        public async Task<string> PostAsync(string method, string name)
        {
            using (_handler = new HttpClientHandler())
            {
                _handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                using (_client = new HttpClient(_handler))
                {
                    _client.DefaultRequestHeaders.Accept.Clear();
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/yaml"));
                    _client.DefaultRequestHeaders.Add("Authorization", _auth);

                    var fullApiCall = $"{_urlBase}{method}";
                    var label = name.Replace("deployment-", "");
                    var yamlBody = $"apiVersion: apps/v1\nkind: Deployment\nmetadata:\n  name: {name}\n  labels:\n    app: {label}\nspec:\n  replicas: 1\n  selector:\n    matchLabels:\n      app: {label}\n  template:\n    metadata:\n      labels:\n        app: {label}\n    spec:\n      containers:\n      - name: {label}\n        image: openhack/minecraft-server:1.0\n        env:\n        - name: EULA\n          value: \"TRUE\"\n        ports:\n        - containerPort: 25565\n        - containerPort: 25575\n        volumeMounts:\n        - mountPath: /data\n          name: minecraft-volume\n      volumes:\n      - name: minecraft-volume\n        hostPath:\n          # directory location on host\n          path: /minecraft/data\n          # this field is optional\n          type: DirectoryOrCreate";
                    var content = new StringContent(yamlBody, Encoding.UTF8, "application/yaml");

                    var response = await _client.PostAsync(fullApiCall, content);

                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        public async Task<string> DeleteAsync(string method)
        {
            using (_handler = new HttpClientHandler())
            {
                _handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                using (_client = new HttpClient(_handler))
                {
                    _client.DefaultRequestHeaders.Accept.Clear();
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    _client.DefaultRequestHeaders.Add("Authorization", _auth);
                    
                    string fullApiCall = $"{_urlBase}{method}";

                    var response = await _client.DeleteAsync(fullApiCall);

                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
    }
}
