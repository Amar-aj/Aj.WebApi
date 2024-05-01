namespace WebApi.Context;

public class DbResponse<T>
{
    public T? Data { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public DbResponse(int statusCode, string message, T? data)
    {
        StatusCode = statusCode;
        Message = message;
        Data = data;
    }
}