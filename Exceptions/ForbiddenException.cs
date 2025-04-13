namespace TodoApp.Exceptions
{
    public class ForbiddenException : ApiException
    {
        public ForbiddenException(string message = "Access denied") : base(403,message) { }
    }
}
