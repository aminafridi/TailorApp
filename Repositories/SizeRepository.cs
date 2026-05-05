using Dapper;
using Microsoft.Data.SqlClient;
using TailorApp.Models;
using System.Data;

namespace TailorApp.Repositories;

public class SizeRepository(string connectionString) : ISizeRepository
{
    private SqlConnection CreateConnection() => new(connectionString);

    public async Task<IEnumerable<Size>> GetByCustomerIdAsync(int customerId)
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<Size>("sp_Sizes_GetByCustomerId", 
            new { CustomerId = customerId }, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Size?> GetByIdAsync(int sizeId)
    {
        using var conn = CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Size>("sp_Sizes_GetById", 
            new { SizeId = sizeId }, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> GetNextRegisterNoAsync(int customerId)
    {
        using var conn = CreateConnection();
        return await conn.ExecuteScalarAsync<int>("sp_Sizes_GetNextRegisterNo", 
            new { CustomerId = customerId }, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> CreateAsync(Size size)
    {
        using var conn = CreateConnection();
        return await conn.ExecuteScalarAsync<int>("sp_Sizes_Create", 
            new {
                size.Customer_ID, size.RegisterNo, size.Lambai, size.Bazo, size.BazoType, size.BazoDetail,
                size.Tera, size.Calar, size.CalarType, size.CalarDetail, size.Chati, size.Kamar,
                size.Ghera, size.GheraType, size.ShalwarLambai, size.Pancha,
                size.IsDoubleSidePocket, size.IsFrontPocket, size.IsShalwarPocket, size.IsCheckPatiKaj,
                size.Pati, size.Design, size.OtherDetails
            }, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateAsync(Size size)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync("sp_Sizes_Update", 
            new {
                size.SizeID,
                size.Lambai, size.Bazo, size.BazoType, size.BazoDetail,
                size.Tera, size.Calar, size.CalarType, size.CalarDetail,
                size.Chati, size.Kamar, size.Ghera, size.GheraType,
                size.ShalwarLambai, size.Pancha,
                size.IsDoubleSidePocket, size.IsFrontPocket,
                size.IsShalwarPocket, size.IsCheckPatiKaj,
                size.Pati, size.Design, size.OtherDetails
            }, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteAsync(int sizeId)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync("sp_Sizes_Delete", 
            new { SizeId = sizeId }, 
            commandType: CommandType.StoredProcedure);
    }
}
