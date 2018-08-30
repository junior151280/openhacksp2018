namespace api.Models
{
    public class Instance
    {
        public Instance()
        {
            Endpoints = new Endpoint();
        }

        public string Name { get; set; }
        public Endpoint Endpoints { get; set; }

    }

    public class Endpoint
    {
        public string Minecraft { get; set; }
        public string Rcon { get; set; }
    }
}
