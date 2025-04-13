namespace TodoApp.Exceptions
{
    public class UnauthorizedException : ApiException
    {
        public UnauthorizedException(string message = "Unauthorized access") : base(401, message) { }
    }
}
