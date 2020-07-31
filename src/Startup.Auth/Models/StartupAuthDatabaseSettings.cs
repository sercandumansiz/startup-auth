namespace Startup.Auth.Models
{
    public class StartupAuthDatabaseSettings : IStartupAuthDatabaseSettings
    {
        public string AuthsCollectionName { get; set; }
        public string UsersCollectionName { get; set; }
        public string InvalidTokensCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IStartupAuthDatabaseSettings
    {
        string AuthsCollectionName { get; set; }
        string UsersCollectionName { get; set; }
        string InvalidTokensCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
