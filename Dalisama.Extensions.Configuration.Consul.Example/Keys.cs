namespace Dalisama.Extensions.Configuration.Consul.Example
{
    public partial class Startup
    {
        public class Keys
        {
            public string Key1 { get; set; }
            public string Key2 { get; set; }
            public string Key3 { get; set; }
            public int Key4 { get; set; }
        }

        public class RawKeys : Keys { };

    }
}
