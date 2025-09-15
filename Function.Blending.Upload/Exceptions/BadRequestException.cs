namespace Function.Blending.Upload.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}