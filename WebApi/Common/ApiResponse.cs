using System.Text.Json.Serialization;

namespace WebApi.Common;


public class ApiResponse<T>
{
    public T Data { get; set; }
    public string Message { get; set; }
    [JsonIgnore]
    public int status_code { get; set; }

    public ApiResponse(T data = default, string message = null, int statusCode = 0)
    {
        Data = data;
        Message = message;
        status_code = statusCode;
    }
}



public class ApiPaginationResponse<T>
{
    public List<T> Data { get; set; }
    public string Message { get; set; }
    public int page_number { get; set; }

    public int page_size { get; set; }
    public int total_pages { get; set; }
    [JsonIgnore]
    public int status_code { get; set; }

    public ApiPaginationResponse( List<T> data = default, string message = null, int pageNumber = 0, int pageSize = 0, int totalPage = 0, int statusCode = 0)
    {
        Data = data;
        Message = message;
        page_number = pageNumber;
        page_size = pageSize;
        total_pages = totalPage;
        status_code = statusCode;
    }
}


public class ApiTokenResponse<T>
{
    public T Data { get; set; }
    public string Message { get; set; }
    [JsonIgnore]
    public int status_code { get; set; }

    public string Token { get; set; }

    public ApiTokenResponse( T data = default, string errorMessage = null, int statusCode = 0, string token = null)
    {
        Data = data;
        Message = errorMessage;
        status_code = statusCode;
        Token = token;
    }
}
