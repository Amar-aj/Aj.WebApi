using Dapper;
using Newtonsoft.Json;
using System.Data;
using WebApi.Common;
using WebApi.Context;
using WebApi.Models;

namespace WebApi.Services;

public interface IAccountService
{
    Task<ApiTokenResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<long>> CreateAsync(UserAddEditRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<long>> UpdateAsync(long user_id, UserAddEditRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<long>> DeleteAsync(long user_id, CancellationToken cancellationToken);
    Task<ApiResponse<UserReadResponse>> GetAsync(long user_id, CancellationToken cancellationToken);
    Task<ApiPaginationResponse<UserReadResponse>> GetAsync(int page_number, int page_size, CancellationToken cancellationToken);
}

public class AccountService(IDapperService _dapperService) : IAccountService
{
    public async Task<ApiResponse<long>> CreateAsync(UserAddEditRequest request, CancellationToken cancellationToken)
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
        parameters.Add("p_status_code", dbType: DbType.String, direction: ParameterDirection.Output);

        var result = await _dapperService.InsertAsync<long>("sp_user_create_update_delete", parameters);
        var message = parameters.Get<string>("p_message");
        var user_id = parameters.Get<long>("p_user_id");
        var status_code = parameters.Get<int>("p_status_code");

        return new ApiResponse<long>(user_id, message, status_code);


    }
    public async Task<ApiResponse<long>> UpdateAsync(long user_id, UserAddEditRequest request, CancellationToken cancellationToken)
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
        parameters.Add("p_status_code", dbType: DbType.String, direction: ParameterDirection.Output);


        var result = await _dapperService.InsertAsync<long>("sp_user_create_update_delete", parameters);
        var message = parameters.Get<string>("p_message");
        var user_ids = parameters.Get<long>("p_user_id");
        var status_code = parameters.Get<int>("p_status_code");

        return new ApiResponse<long>(user_ids, message, status_code);

    }


    public async Task<ApiResponse<long>> DeleteAsync(long user_id, CancellationToken cancellationToken)
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
        parameters.Add("p_status_code", dbType: DbType.String, direction: ParameterDirection.Output);


        var result = await _dapperService.InsertAsync<long>("sp_user_create_update_delete", parameters);
        var message = parameters.Get<string>("p_message");
        var user_ids = parameters.Get<long>("p_user_id");
        var status_code = parameters.Get<int>("p_status_code");

        return new ApiResponse<long>(user_ids, message, status_code);

    }
    public async Task<ApiResponse<UserReadResponse>> GetAsync(long user_id, CancellationToken cancellationToken)
    {
        var jsonResponseString = await _dapperService.FunctionCallAsync($"SELECT * FROM fun_identity_users_pagination({1}, {1},{null},{user_id})");
        var data = JsonConvert.DeserializeObject<ApiPaginationResponse<UserReadResponse>>(jsonResponseString);
        if (data.Data is null)
        {
            return new ApiResponse<UserReadResponse>(null, data.Message, 200);

        }
        else
        {
            var singleData = new UserReadResponse
            {
                user_id = user_id,
                email = data.Data[0].email,
                username = data.Data[0].username,
                is_active = data.Data[0].is_active
            };
            return new ApiResponse<UserReadResponse>(singleData, data.Message, 200);
        }
    }
    public async Task<ApiPaginationResponse<UserReadResponse>> GetAsync(int page_number, int page_size, CancellationToken cancellationToken)
    {

        var jsonResponseString = await _dapperService.FunctionCallAsync($"SELECT * FROM fun_identity_users_pagination({page_number}, {page_size})");
        return JsonConvert.DeserializeObject<ApiPaginationResponse<UserReadResponse>>(jsonResponseString);

    }
    public async Task<ApiTokenResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var hashedPassword = await _dapperService.FunctionCallAsync($"SELECT * FROM fun_identity_user_password('{request.email.Trim()}')");
            if (hashedPassword is null)
            {
                return new ApiTokenResponse<LoginResponse>(null, "Invalid User Name");
            }
            else
            {
                if (PasswordHasher.VerifyPassword(request.password.Trim(), hashedPassword))
                {
                    var jsonResponseString = await _dapperService.FunctionCallAsync($"SELECT * FROM fun_identity_users_pagination({1}, {1},'{request.email}')");
                    var data = JsonConvert.DeserializeObject<ApiPaginationResponse<UserReadResponse>>(jsonResponseString);
                    if (data.Data is null)
                    {
                        return new ApiTokenResponse<LoginResponse>(null, data.Message, 200);
                    }
                    else
                    {
                        var token = JwtToken.GenerateToken(data.Data[0].username, "admin");
                        var singleData = new LoginResponse(data.Data[0].user_id, data.Data[0].email, data.Data[0].username);
                        return new ApiTokenResponse<LoginResponse>(singleData, "You are successfully logged in", 200, token);
                    }
                }
                else
                {
                    return new ApiTokenResponse<LoginResponse>(null, "Invalid Password");
                }
            }
        }
        catch (Exception)
        {

            throw;
        }

    }


}
