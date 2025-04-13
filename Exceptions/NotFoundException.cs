namespace TodoApp.Exceptions
{
    public class NotFoundException : ApiException
    {
        public NotFoundException(string message = "Resource not found") : base(404, message) { }
    }
}
