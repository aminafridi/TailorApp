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
                size.SizeID, size.RegisterNo,
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

    public async Task<string?> GetRegisterNoOwnerAsync(int registerNo, int? exceptSizeId = null)
    {
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<string>(@"
            SELECT TOP 1 c.CustomerName
            FROM Size s
            INNER JOIN Customer c ON s.Customer_ID = c.CustomerID
            WHERE s.RegisterNo = @RegisterNo AND (@ExceptSizeId IS NULL OR s.SizeID <> @ExceptSizeId)",
            new { RegisterNo = registerNo, ExceptSizeId = exceptSizeId });
    }

    public async Task<PagedResult<Size>> GetPagedAsync(SizeSearch filter, int page = 1, int pageSize = 50)
    {
        using var conn = CreateConnection();
        int offset = pageSize <= 0 ? 0 : (page - 1) * pageSize; // "all" has a single page
        int queryPageSize = pageSize <= 0 ? int.MaxValue : pageSize;

        var conditions = new List<string>();
        var parameters = new DynamicParameters();
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", queryPageSize);

        if (!string.IsNullOrEmpty(filter.Name))
        {
            conditions.Add("c.CustomerName LIKE '%' + @Name + '%'");
            parameters.Add("Name", filter.Name);
        }

        if (filter.RegisterNo is not null)
        {
            conditions.Add("s.RegisterNo = @RegisterNo");
            parameters.Add("RegisterNo", filter.RegisterNo);
        }

        if (!string.IsNullOrEmpty(filter.Mobile))
        {
            conditions.Add("(c.MobileNo1 LIKE '%' + @Mobile + '%' OR c.MobileNo2 LIKE '%' + @Mobile + '%')");
            parameters.Add("Mobile", filter.Mobile);
        }

        string whereClause = conditions.Count > 0
            ? "WHERE " + string.Join(" AND ", conditions)
            : "";

        string countSql = $@"
            SELECT COUNT(*)
            FROM Size s
            INNER JOIN Customer c ON s.Customer_ID = c.CustomerID
            {whereClause}";

        string itemsSql = $@"
            SELECT s.*, c.CustomerName, c.MobileNo1, c.MobileNo2
            FROM Size s
            INNER JOIN Customer c ON s.Customer_ID = c.CustomerID
            {whereClause}
            ORDER BY s.SizeID DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        var totalCount = await conn.ExecuteScalarAsync<int>(countSql, parameters);
        var items = await conn.QueryAsync<Size>(itemsSql, parameters);

        return new PagedResult<Size>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize <= 0 ? totalCount : pageSize
        };
    }
}
