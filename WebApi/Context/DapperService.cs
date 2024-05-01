using Dapper;
using System.Data;
using System.Data.Common;
using WebApi.Common;

namespace WebApi.Context;


public interface IDapperService
{
    //get all
    IEnumerable<T> GetAll<T>(string storeProcedureName, DynamicParameters parameters);
    Task<IEnumerable<T>> GetAllAsync<T>(string storeProcedureName, DynamicParameters parameters);
    Task<string> FunctionCallAsync(string function);

    //get by id
    T GetById<T>(string storeProcedureName, DynamicParameters parameters);
    Task<T> GetByIdAsync<T>(string storeProcedureName, DynamicParameters parameters);
    //T LoginAsync<T>(string storeProcedureName, DynamicParameters parameters);

    //insert
    bool Insert(string storeProcedureName, DynamicParameters parameters);
    Task<T> InsertAsync<T>(string storeProcedureName, DynamicParameters parameters);
    Task<T> InsertAsync<T>(string storeProcedureName, DynamicParameters parameters, IFormFile? formFile);

    //update
    Task<T> UpdateAsync<T>(string storeProcedureName, DynamicParameters parameters);
    Task<DbResponse<T>> UpdateAsync<T>(string storeProcedureName, DynamicParameters parameters, bool idDbResponse);

    //delete
    Task<T> DeleteAsync<T>(string storeProcedureName, DynamicParameters parameters);

    //dropdown
    Task<List<T>> GetDropdownAsync<T>(string storedProcedure, DynamicParameters parameters);


    Task<ApiPaginationResponse<T>> GetPaginationResponseAsync<T>(string storedProcedure, DynamicParameters parameters, int pageNumber, int pageSize);
}


public class DapperService(DapperDbContext _dbContext) : IDapperService
{

    public IEnumerable<T> GetAll<T>(string storeProcedureName, DynamicParameters parameters)
    {
        using (IDbConnection dbConnection = _dbContext.CreateConnection())
        {
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }
            return dbConnection.Query<T>(storeProcedureName, parameters, commandType: CommandType.StoredProcedure);
        }
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>(string storeProcedureName, DynamicParameters parameters)
    {
        using (IDbConnection dbConnection = _dbContext.CreateConnection())
        {
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }
            return await dbConnection.QueryAsync<T>(storeProcedureName, parameters, commandType: CommandType.StoredProcedure);
        }
    }

    public T GetById<T>(string storeProcedureName, DynamicParameters parameters)
    {
        using (IDbConnection dbConnection = _dbContext.CreateConnection())
        {
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }
            return dbConnection.QueryFirstOrDefault<T>(storeProcedureName, parameters, commandType: CommandType.StoredProcedure);
        }
    }

    public async Task<T> GetByIdAsync<T>(string storeProcedureName, DynamicParameters parameters)
    {
        using (IDbConnection dbConnection = _dbContext.CreateConnection())
        {
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }
            return await dbConnection.QueryFirstOrDefaultAsync<T>(storeProcedureName, parameters, commandType: CommandType.StoredProcedure);
        }
    }



    public bool Insert(string storeProcedureName, DynamicParameters parameters)
    {
        using (IDbConnection dbConnection = _dbContext.CreateConnection())
        {
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }
            using var transaction = dbConnection.BeginTransaction();

            int affectedRows = dbConnection.Execute(storeProcedureName, parameters, commandType: CommandType.StoredProcedure);
            transaction.Commit();
            return affectedRows > 0;

        }
    }

    public async Task<T> InsertAsync<T>(string storeProcedureName, DynamicParameters parameters)
    {
        using (IDbConnection dbConnection = _dbContext.CreateConnection())
        {
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }
            //parameters.Add("p_RECORD_CREATED_BY", 1);
            return await dbConnection.QueryFirstOrDefaultAsync<T>(storeProcedureName, parameters, commandType: CommandType.StoredProcedure);

        }
    }

    public async Task<T> InsertAsync<T>(string storeProcedureName, DynamicParameters parameters, IFormFile? formFile)
    {
        using (IDbConnection dbConnection = _dbContext.CreateConnection())
        {
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }
            var (bytes, fileExtension, fileType) = await FormFileExtensions.GetFileDetailsAsync(formFile);

            parameters.Add("p_DOC_COPY", bytes);
            parameters.Add("p_FILE_EXTENSION", fileExtension);
            parameters.Add("p_FILE_TYPE", fileType);
            parameters.Add("p_RECORD_CREATED_BY", 1);
            return await dbConnection.QueryFirstOrDefaultAsync<T>(storeProcedureName, parameters, commandType: CommandType.StoredProcedure);

        }
    }

    public bool Update(string storeProcedureName, DynamicParameters parameters)
    {
        throw new NotImplementedException();
    }

    public Task<T> UpdateAsync<T>(string storeProcedureName, DynamicParameters parameters)
    {
        throw new NotImplementedException();
    }

    public Task<DbResponse<T>> UpdateAsync<T>(string storeProcedureName, DynamicParameters parameters, bool idDbResponse)
    {
        throw new NotImplementedException();
    }




    public async Task<T> DeleteAsync<T>(string storeProcedureName, DynamicParameters parameters)
    {
        using (IDbConnection dbConnection = _dbContext.CreateConnection())
        {
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }
            //parameters.Add("p_LanguageId", ApplicationConstant.AppLanguage);
            ////parameters.Add("p_DeletedBy", _currentUser.GetUserId());
            //parameters.Add("p_DeletedBy", 1);
            //parameters.Add("p_DeletedOn", ApplicationConstant.AppDateTime);
            //parameters.Add("p_TenantId", ApplicationConstant.AppTenant);

            return await dbConnection.QueryFirstOrDefaultAsync<T>(storeProcedureName, parameters, commandType: CommandType.StoredProcedure);

        }
    }


    public async Task<List<T>> GetDropdownAsync<T>(string storedProcedure, DynamicParameters parameters)
    {
        using (var dbConnection = _dbContext.CreateConnection())
        {
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }
            return (await dbConnection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure)).AsList();
        }
    }

    public async Task<ApiPaginationResponse<T>> GetPaginationResponseAsync<T>(string storedProcedure, DynamicParameters parameters, int pageNumber, int pageSize)
    {
        using (IDbConnection dbConnection = _dbContext.CreateConnection())
        {
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }

            int total_pages = 0;

            parameters.Add("p_PageNumber", pageNumber);
            parameters.Add("p_PageSize", pageSize);
            parameters.Add("total_pages", total_pages, DbType.Int32, ParameterDirection.Output);
            var response = await dbConnection.QueryAsync<T>(storedProcedure,
               parameters, commandType: CommandType.StoredProcedure);
            if (response == null || !response.Any())
            {
                return new ApiPaginationResponse<T>( new List<T>(), "No records found", pageNumber, pageSize, total_pages, 200);
            }
            else
            {
                total_pages = parameters.Get<int>("total_pages");
                return new ApiPaginationResponse<T>( response.AsList(), "Records fetched successfully", pageNumber, pageSize, total_pages, 200);
            }
        }
    }

    public async Task GetAllAsync(string storeProcedureName, DynamicParameters parameters)
    {
        using (IDbConnection dbConnection = _dbContext.CreateConnection())
        {
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }

            int total_pages = 0;

            //parameters.Add("p_PageNumber", pageNumber);
            //parameters.Add("p_PageSize", pageSize);
            //parameters.Add("total_pages", total_pages, DbType.Int32, ParameterDirection.Output);
            parameters.Add("p_result", direction: ParameterDirection.Output);

            var response = await dbConnection.QueryAsync(storeProcedureName,
               parameters, commandType: CommandType.StoredProcedure);
            if (response == null || !response.Any())
            {
                //return new ApiPaginationResponse<T>(true, new List<T>(), "No records found", pageNumber, pageSize, total_pages, 200);
            }
            else
            {
                //total_pages = parameters.Get<int>("total_pages");

                var data = parameters.Get<int>("p_result");

                var v = data;
                //return new ApiPaginationResponse<T>(true, response.AsList(), "Records fetched successfully", pageNumber, pageSize, total_pages, 200);
            }
        }
    }

    public async Task<string> FunctionCallAsync(string function)
    {
        using (IDbConnection dbConnection = _dbContext.CreateConnection())
        {
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }
            var response = await dbConnection.QueryFirstOrDefaultAsync<string>(function, commandType: CommandType.Text);
            return response == null ? null : response;
        }
    }
}