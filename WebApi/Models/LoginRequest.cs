namespace WebApi.Models;

public sealed record LoginRequest(string email, string password);
public  record LoginResponse(long user_id, string email, string username);