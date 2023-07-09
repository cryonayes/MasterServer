namespace MasterServer
{
    static class Constants
    {
        public const int TicksPerSec = 10;
        public const float MsPerTick = 1000f / TicksPerSec;
    }

    static class Globals
    {
        public static string MongoUri = "mongodb+srv://casestudy:case\"100@panteoncasestudy.ofcnjru.mongodb.net/?retryWrites=true&w=majority";
        public static string DatabaseName = "panteon";
        public static string CollectionName = "users";
        
    }
}
