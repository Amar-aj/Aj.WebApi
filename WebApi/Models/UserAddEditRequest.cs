namespace WebApi.Models;

public class UserAddEditRequest
{
    public string username { get; set; }
    public string password { get; set; }
    public string email { get; set; }
}
public class UserReadResponse
{
    public long user_id { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public bool is_active { get; set; }
}
