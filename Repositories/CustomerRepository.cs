using Dapper;
using Microsoft.Data.SqlClient;
using TailorApp.Models;
using System.Data;

namespace TailorApp.Repositories;

public class CustomerRepository(string connectionString) : ICustomerRepository
{
    private SqlConnection CreateConnection() => new(connectionString);

    public async Task<PagedResult<Customer>> GetPagedAsync(string? search, int page = 1, int pageSize = 50)
    {
        using var conn = CreateConnection();
        int offset = (page - 1) * pageSize;
        int queryPageSize = pageSize <= 0 ? int.MaxValue : pageSize; // handle "all"

        using var multi = await conn.QueryMultipleAsync("sp_Customers_GetPaged", 
            new { Search = search, Offset = offset, PageSize = queryPageSize }, 
            commandType: CommandType.StoredProcedure);
            
        var totalCount = await multi.ReadSingleAsync<int>();
        var items = await multi.ReadAsync<Customer>();

        return new PagedResult<Customer>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize <= 0 ? totalCount : pageSize
        };
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        using var conn = CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Customer>("sp_Customers_GetById", 
            new { Id = id }, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task<DashboardViewModel> GetDashboardStatsAsync()
    {
        using var conn = CreateConnection();
        using var multi = await conn.QueryMultipleAsync("sp_Customers_GetDashboardStats", 
            commandType: CommandType.StoredProcedure);
            
        var stats = await multi.ReadSingleAsync<DashboardViewModel>();
        stats.RecentCustomers = (await multi.ReadAsync<Customer>()).ToList();
        return stats;
    }

    public async Task<int> CreateAsync(Customer customer)
    {
        using var conn = CreateConnection();
        return await conn.ExecuteScalarAsync<int>("sp_Customers_Create", 
            new { 
                customer.CustomerName, 
                customer.MobileNo1, 
                customer.MobileNo2, 
                customer.Status 
            }, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateAsync(Customer customer)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync("sp_Customers_Update", 
            new { 
                customer.CustomerID,
                customer.CustomerName, 
                customer.MobileNo1, 
                customer.MobileNo2, 
                customer.Status 
            }, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync("sp_Customers_Delete", 
            new { Id = id }, 
            commandType: CommandType.StoredProcedure);
    }
}
