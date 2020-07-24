namespace Startup.Auth.Models
{
    public class StartupAuthDatabaseSettings : IStartupAuthDatabaseSettings
    {
        public string UsersCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IStartupAuthDatabaseSettings
    {
        string UsersCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
