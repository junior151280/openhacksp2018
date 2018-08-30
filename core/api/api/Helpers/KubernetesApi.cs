using api.Models;
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
        private string _namespace;

        public KubernetesApi(string urlBase, string auth, string nspace)
        {
            _urlBase = urlBase;
            _auth = auth;
            _namespace = nspace;
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
        public async Task<string> PostAsync(string name)
        {
            using (_handler = new HttpClientHandler())
            {
                _handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                using (_client = new HttpClient(_handler))
                {
                    _client.DefaultRequestHeaders.Accept.Clear();
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/yaml"));
                    _client.DefaultRequestHeaders.Add("Authorization", _auth);

                    var deploymentResponse = await DeploymentCreate(name);
                    var serviceResponse = await ServiceCreate(name);

                    return $"{deploymentResponse} \n\n {serviceResponse}";
                }
            }
        }
        public async Task<string> DeleteAsync(string name)
        {
            using (_handler = new HttpClientHandler())
            {
                _handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                using (_client = new HttpClient(_handler))
                {
                    _client.DefaultRequestHeaders.Accept.Clear();
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    _client.DefaultRequestHeaders.Add("Authorization", _auth);

                    string deployUrl = $"apis/apps/v1/namespaces/{_namespace}/deployments/{name}";
                    string fullApiCall = $"{_urlBase}{deployUrl}";
                    var deployResponse = await DeleteMethod(fullApiCall);

                    deployUrl = $"api/v1/namespaces/{_namespace}/services/{name}";
                    fullApiCall = $"{_urlBase}{deployUrl}";
                    var serviceResponse = await DeleteMethod(fullApiCall);

                    return $"{deployResponse} \n\n {serviceResponse}";
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
        private async Task<string> DeploymentCreate(string name)
        {
            var deploymentCreateMethod = $"apis/apps/v1/namespaces/{_namespace}/deployments";
            var deploymentFullCall = $"{_urlBase}{deploymentCreateMethod}";
            var yamlBody = $"apiVersion: apps/v1\nkind: Deployment\nmetadata:\n  name: {name}\n  labels:\n    app: {name}\nspec:\n  replicas: 1\n  selector:\n    matchLabels:\n      app: {name}\n  template:\n    metadata:\n      labels:\n        app: {name}\n    spec:\n      containers:\n      - name: {name}\n        image: openhack/minecraft-server:1.0\n        env:\n        - name: EULA\n          value: \"TRUE\"\n        ports:\n        - containerPort: 25565\n        - containerPort: 25575\n        volumeMounts:\n        - mountPath: /data\n          name: minecraft-volume\n      volumes:\n      - name: minecraft-volume\n        hostPath:\n          # directory location on host\n          path: /minecraft/data\n          # this field is optional\n          type: DirectoryOrCreate";
            var content = new StringContent(yamlBody, Encoding.UTF8, "application/yaml");
            var deploymentResponse = await _client.PostAsync(deploymentFullCall, content);
            return await deploymentResponse.Content.ReadAsStringAsync();
        }
        private async Task<string> ServiceCreate(string name)
        {
            var serviceCreateMethod = $"api/v1/namespaces/{_namespace}/services";
            var serviceFullCall = $"{_urlBase}{serviceCreateMethod}";
            var yamlBody = $"kind: Service\napiVersion: v1\nmetadata:\n  name: {name}\nspec:\n  selector:\n    app: {name}\n  type: LoadBalancer\n  ports:\n  - protocol: TCP\n    name: \"server\"\n    port: 25565\n    targetPort: 25565\n  - protocol: TCP\n    name: \"rcon\"\n    port: 25575\n    targetPort: 25575";
            var content = new StringContent(yamlBody, Encoding.UTF8, "application/yaml");
            var serviceResponse = await _client.PostAsync(serviceFullCall, content);
            return await serviceResponse.Content.ReadAsStringAsync();
        }
        private async Task<string> DeleteMethod(string apiUrl)
        {
            var response = await _client.DeleteAsync(apiUrl);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
