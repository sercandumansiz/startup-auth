namespace Startup.Auth.Models
{
    public class BaseModel<T>
    {
        public T Data { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
