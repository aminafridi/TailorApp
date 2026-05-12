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

    public async Task<PagedResult<Size>> GetPagedAsync(string? search, int page = 1, int pageSize = 50, string? searchName = null, string? searchRegNo = null, string? searchMobile = null)
    {
        using var conn = CreateConnection();
        int offset = (page - 1) * pageSize;
        int queryPageSize = pageSize <= 0 ? int.MaxValue : pageSize;

        var conditions = new List<string>();
        var parameters = new DynamicParameters();
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", queryPageSize);

        if (!string.IsNullOrEmpty(search))
        {
            conditions.Add(@"(@Search IS NULL 
               OR c.CustomerName LIKE '%' + @Search + '%'
               OR c.MobileNo1 LIKE '%' + @Search + '%'
               OR c.MobileNo2 LIKE '%' + @Search + '%'
               OR CAST(s.RegisterNo AS VARCHAR) LIKE '%' + @Search + '%')");
            parameters.Add("Search", search);
        }

        if (!string.IsNullOrEmpty(searchName))
        {
            conditions.Add("c.CustomerName LIKE '%' + @SearchName + '%'");
            parameters.Add("SearchName", searchName);
        }

        if (!string.IsNullOrEmpty(searchRegNo))
        {
            conditions.Add("CAST(s.RegisterNo AS VARCHAR) LIKE '%' + @SearchRegNo + '%'");
            parameters.Add("SearchRegNo", searchRegNo);
        }

        if (!string.IsNullOrEmpty(searchMobile))
        {
            conditions.Add("(c.MobileNo1 LIKE '%' + @SearchMobile + '%' OR c.MobileNo2 LIKE '%' + @SearchMobile + '%')");
            parameters.Add("SearchMobile", searchMobile);
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
