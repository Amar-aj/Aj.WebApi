using Dapper;
using Newtonsoft.Json;
using System.Data;
using WebApi.Common;
using WebApi.Context;
using WebApi.Models;

namespace WebApi.Services;

public interface IAccountService
{
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<string> CreateAsync(UserAddEditRequest request, CancellationToken cancellationToken);
    Task<string> UpdateAsync(long user_id, UserAddEditRequest request, CancellationToken cancellationToken);
    Task<string> DeleteAsync(long user_id, CancellationToken cancellationToken);
    Task<ApiPaginationResponse<UserReadResponse>> GetAsync(int page_number, int page_size, CancellationToken cancellationToken);
}

public class AccountService(IDapperService _dapperService) : IAccountService
{
    public async Task<string> CreateAsync(UserAddEditRequest request, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();

        parameters.Add("p_action", nameof(ActionEnum.CREATE), DbType.String);
        parameters.Add("p_user_id", dbType: DbType.Int64, direction: ParameterDirection.InputOutput);
        parameters.Add("p_username", request.username, DbType.String);
        parameters.Add("p_hashed_password", PasswordHasher.HashPassword(request.password), DbType.String);
        parameters.Add("p_email", request.email, DbType.String);
        parameters.Add("p_is_active", true, DbType.Boolean);
        parameters.Add("p_created_by", 1, DbType.Int64);
        parameters.Add("p_updated_by", null, DbType.Int64);
        parameters.Add("p_deleted_by", null, DbType.Int64);
        parameters.Add("p_message", dbType: DbType.String, direction: ParameterDirection.Output);


        try
        {
            var result = await _dapperService.InsertAsync<string>("sp_user_create_update", parameters);
            var message = parameters.Get<string>("p_message");
            return message;
        }
        catch (Exception ex)
        {
            throw;
        }

    }
    public async Task<string> UpdateAsync(long user_id, UserAddEditRequest request, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();

        parameters.Add("p_action", nameof(ActionEnum.UPDATE), DbType.String);
        parameters.Add("p_user_id", user_id, dbType: DbType.Int64, direction: ParameterDirection.InputOutput);
        parameters.Add("p_username", request.username, DbType.String);
        parameters.Add("p_hashed_password", request.password, DbType.String);
        parameters.Add("p_email", request.email, DbType.String);
        parameters.Add("p_is_active", true, DbType.Boolean);
        parameters.Add("p_created_by", null, DbType.Int64);
        parameters.Add("p_updated_by", 1, DbType.Int64);
        parameters.Add("p_deleted_by", null, DbType.Int64);
        parameters.Add("p_message", dbType: DbType.String, direction: ParameterDirection.Output);


        try
        {
            var result = await _dapperService.InsertAsync<string>("sp_user_create_update", parameters);
            var message = parameters.Get<string>("p_message");
            return message;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    public async Task<ApiPaginationResponse<UserReadResponse>> GetAsync(int page_number, int page_size, CancellationToken cancellationToken)
    {

        var jsonResponseString = await _dapperService.FunctionCallAsync($"SELECT * FROM fun_users_pagination({page_number}, {page_size})");
        return JsonConvert.DeserializeObject<ApiPaginationResponse<UserReadResponse>>(jsonResponseString);

    }

    public async Task<string> DeleteAsync(long user_id, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();

        parameters.Add("p_action", nameof(ActionEnum.DELETE), DbType.String);
        parameters.Add("p_user_id", user_id, dbType: DbType.Int64, direction: ParameterDirection.InputOutput);
        parameters.Add("p_username", null, DbType.String);
        parameters.Add("p_hashed_password", null, DbType.String);
        parameters.Add("p_email", null, DbType.String);
        parameters.Add("p_is_active", true, DbType.Boolean);
        parameters.Add("p_created_by", null, DbType.Int64);
        parameters.Add("p_updated_by", null, DbType.Int64);
        parameters.Add("p_deleted_by", 1, DbType.Int64);
        parameters.Add("p_message", dbType: DbType.String, direction: ParameterDirection.Output);


        try
        {
            var result = await _dapperService.InsertAsync<string>("sp_user_create_update", parameters);
            var message = parameters.Get<string>("p_message");
            return message;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var hashedPassword = await _dapperService.FunctionCallAsync($"SELECT * FROM fun_user_password('{request.username}')");
        if (hashedPassword is null)
        {
            return new ApiResponse<LoginResponse>(null, "Invalid User Name");
        }
        else
        {
            if (PasswordHasher.VerifyPassword(request.password, hashedPassword))
            {
                return new ApiResponse<LoginResponse>(null, "Ok");
            }
            else
            {
                return new ApiResponse<LoginResponse>(null, "Invalid Password");
            }
        }
    }
}
