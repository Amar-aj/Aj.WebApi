namespace WebApi.Models;

public sealed record LoginRequest(string email, string password);
public sealed record LoginResponse(long user_id, string username, string token);