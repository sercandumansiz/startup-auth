namespace Startup.Auth.Models
{
    public class ApplicationSettings : IApplicationSettings
    {
        public string Secret { get; set; }
    }

    public interface IApplicationSettings
    {
        public string Secret { get; set; }
    }
}
