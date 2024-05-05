namespace WebApi.Models;

public class Role
{
    public long id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public bool is_active { get; set; }
}
public class RoleAddRequest
{
    public string role_name { get; set; }
    public string role_description { get; set; }
}
public class RoleEditRequest
{
    //public long id { get; set; }
    public string role_name { get; set; }
    public string role_description { get; set; }
    public bool is_active { get; set; }
}
public class RoleReadResponse
{
    public long role_id { get; set; }
    public string role_name { get; set; }
    public string role_description { get; set; }
    public bool is_active { get; set; }
}
