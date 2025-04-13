namespace TodoApp.Exceptions
{
    public class ValidationException : ApiException
    {
        public ValidationException(string message = "Validation failed") : base(400, message) { }
    }
}
