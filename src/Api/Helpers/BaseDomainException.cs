namespace Api.Helpers;

public class BaseDomainException : BadHttpRequestException
{
    public int Code { get; }

    public BaseDomainException(string message, int code) : base(message)
    {
        Code = code;
    }
}