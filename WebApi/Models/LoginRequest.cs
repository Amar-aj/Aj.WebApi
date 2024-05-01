namespace WebApi.Models;

public sealed record LoginRequest(string username, string password);
public sealed record LoginResponse(long user_id, string username, string token);