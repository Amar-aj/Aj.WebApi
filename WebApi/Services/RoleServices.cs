using Dapper;
using Newtonsoft.Json;
using System.Data;
using WebApi.Common;
using WebApi.Context;
using WebApi.Models;

namespace WebApi.Services;


public interface IRoleServices
{
    Task<ApiResponse<long>> CreateAsync(RoleAddRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<long>> UpdateAsync(long role_id, RoleEditRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<long>> DeleteAsync(long role_id, CancellationToken cancellationToken);
    Task<ApiResponse<RoleReadResponse>> GetAsync(long role_id, CancellationToken cancellationToken);
    Task<ApiPaginationResponse<RoleReadResponse>> GetAsync(int page_number, int page_size, CancellationToken cancellationToken);
}
public class RoleServices(IDapperService _dapperService) : IRoleServices
{
    public async Task<ApiResponse<long>> CreateAsync(RoleAddRequest request, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();

        parameters.Add("p_action", nameof(ActionEnum.CREATE), DbType.String);
        parameters.Add("p_role_id", dbType: DbType.Int64, direction: ParameterDirection.InputOutput);
        parameters.Add("p_role_name", request.role_name, DbType.String);
        parameters.Add("p_role_description", request.role_description, DbType.String);
        parameters.Add("p_is_active", true, DbType.Boolean);
        parameters.Add("p_created_by", 1, DbType.Int64);
        parameters.Add("p_updated_by", null, DbType.Int64);
        parameters.Add("p_deleted_by", null, DbType.Int64);
        parameters.Add("p_message", dbType: DbType.String, direction: ParameterDirection.Output);
        parameters.Add("p_status_code", dbType: DbType.String, direction: ParameterDirection.Output);

        var result = await _dapperService.InsertAsync<string>("sp_role_create_update_delete", parameters);
        var message = parameters.Get<string>("p_message");
        var user_id = parameters.Get<long>("p_user_id");
        var status_code = parameters.Get<int>("p_status_code");

        return new ApiResponse<long>(user_id, message, status_code);
    }
    public async Task<ApiResponse<long>> UpdateAsync(long role_id, RoleEditRequest request, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();

        parameters.Add("p_action", nameof(ActionEnum.UPDATE), DbType.String);
        parameters.Add("p_role_id", role_id, dbType: DbType.Int64, direction: ParameterDirection.InputOutput);
        parameters.Add("p_role_name", request.role_name, DbType.String);
        parameters.Add("p_role_description", request.role_description, DbType.String);
        parameters.Add("p_is_active", true, DbType.Boolean);
        parameters.Add("p_created_by", null, DbType.Int64);
        parameters.Add("p_updated_by", 1, DbType.Int64);
        parameters.Add("p_deleted_by", null, DbType.Int64);
        parameters.Add("p_message", dbType: DbType.String, direction: ParameterDirection.Output);
        parameters.Add("p_status_code", dbType: DbType.String, direction: ParameterDirection.Output);

        var result = await _dapperService.InsertAsync<string>("sp_role_create_update_delete", parameters);
        var message = parameters.Get<string>("p_message");
        var user_id = parameters.Get<long>("p_user_id");
        var status_code = parameters.Get<int>("p_status_code");

        return new ApiResponse<long>(user_id, message, status_code);

    }
    public async Task<ApiResponse<long>> DeleteAsync(long role_id, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();

        parameters.Add("p_action", nameof(ActionEnum.DELETE), DbType.String);
        parameters.Add("p_role_id", role_id, dbType: DbType.Int64, direction: ParameterDirection.InputOutput);
        parameters.Add("p_role_name", null, DbType.String);
        parameters.Add("p_role_description", null, DbType.String);
        parameters.Add("p_is_active", true, DbType.Boolean);
        parameters.Add("p_created_by", null, DbType.Int64);
        parameters.Add("p_updated_by", null, DbType.Int64);
        parameters.Add("p_deleted_by", 1, DbType.Int64);
        parameters.Add("p_message", dbType: DbType.String, direction: ParameterDirection.Output);
        parameters.Add("p_status_code", dbType: DbType.String, direction: ParameterDirection.Output);

        var result = await _dapperService.InsertAsync<string>("sp_role_create_update_delete", parameters);
        var message = parameters.Get<string>("p_message");
        var user_id = parameters.Get<long>("p_user_id");
        var status_code = parameters.Get<int>("p_status_code");

        return new ApiResponse<long>(user_id, message, status_code);
    }


    public async Task<ApiResponse<RoleReadResponse>> GetAsync(long role_id, CancellationToken cancellationToken)
    {

        var jsonResponseString = await _dapperService.FunctionCallAsync($"SELECT * FROM fun_identity_roles_pagination({1}, {1},{role_id})");

        var data = JsonConvert.DeserializeObject<ApiPaginationResponse<RoleReadResponse>>(jsonResponseString);
        if (data.Data is null)
        {
            return new ApiResponse<RoleReadResponse>(null, data.Message, 200);

        }
        else
        {
            var singleData = new RoleReadResponse
            {
                role_id = data.Data[0].role_id,
                role_name = data.Data[0].role_name,
                role_description = data.Data[0].role_description,
                is_active = data.Data[0].is_active
            };
            return new ApiResponse<RoleReadResponse>(singleData, data.Message, 200);
        }
    }
    public async Task<ApiPaginationResponse<RoleReadResponse>> GetAsync(int page_number, int page_size, CancellationToken cancellationToken)
    {
        var jsonResponseString = await _dapperService.FunctionCallAsync($"SELECT * FROM fun_identity_roles_pagination({page_number}, {page_size})");
        return JsonConvert.DeserializeObject<ApiPaginationResponse<RoleReadResponse>>(jsonResponseString);
    }


}
