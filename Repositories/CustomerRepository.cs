using Dapper;
using Microsoft.Data.SqlClient;
using TailorApp.Models;

namespace TailorApp.Repositories;

public class CustomerRepository(string connectionString) : ICustomerRepository
{
    private SqlConnection CreateConnection() => new(connectionString);

    public async Task<IEnumerable<Customer>> GetAllAsync(string? search = null)
    {
        using var conn = CreateConnection();
        const string sql = """
            SELECT 
                c.CustomerID, c.CustomerName, c.MobileNo1, c.MobileNo2, c.Status,
                COUNT(s.SizeID) AS TotalMeasurements
            FROM Customer c
            LEFT JOIN Size s ON s.Customer_ID = c.CustomerID
            WHERE (@Search IS NULL 
                   OR c.CustomerName LIKE '%' + @Search + '%'
                   OR c.MobileNo1 LIKE '%' + @Search + '%'
                   OR c.MobileNo2 LIKE '%' + @Search + '%')
            GROUP BY c.CustomerID, c.CustomerName, c.MobileNo1, c.MobileNo2, c.Status
            ORDER BY c.CustomerName
            """;
        return await conn.QueryAsync<Customer>(sql, new { Search = search });
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        using var conn = CreateConnection();
        const string sql = """
            SELECT 
                c.CustomerID, c.CustomerName, c.MobileNo1, c.MobileNo2, c.Status,
                COUNT(s.SizeID) AS TotalMeasurements
            FROM Customer c
            LEFT JOIN Size s ON s.Customer_ID = c.CustomerID
            WHERE c.CustomerID = @Id
            GROUP BY c.CustomerID, c.CustomerName, c.MobileNo1, c.MobileNo2, c.Status
            """;
        return await conn.QuerySingleOrDefaultAsync<Customer>(sql, new { Id = id });
    }

    public async Task<DashboardViewModel> GetDashboardStatsAsync()
    {
        using var conn = CreateConnection();
        const string sql = """
            SELECT 
                COUNT(*) AS TotalCustomers,
                SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END) AS ActiveCustomers,
                (SELECT COUNT(*) FROM Size) AS TotalMeasurements
            FROM Customer;

            SELECT TOP 6 
                c.CustomerID, c.CustomerName, c.MobileNo1, c.MobileNo2, c.Status,
                COUNT(s.SizeID) AS TotalMeasurements
            FROM Customer c
            LEFT JOIN Size s ON s.Customer_ID = c.CustomerID
            GROUP BY c.CustomerID, c.CustomerName, c.MobileNo1, c.MobileNo2, c.Status
            ORDER BY c.CustomerID DESC;
            """;

        using var multi = await conn.QueryMultipleAsync(sql);
        var stats = await multi.ReadSingleAsync<DashboardViewModel>();
        stats.RecentCustomers = (await multi.ReadAsync<Customer>()).ToList();
        return stats;
    }

    public async Task<int> CreateAsync(Customer customer)
    {
        using var conn = CreateConnection();
        const string sql = """
            INSERT INTO Customer (CustomerName, MobileNo1, MobileNo2, Status)
            VALUES (@CustomerName, @MobileNo1, @MobileNo2, @Status);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;
        return await conn.ExecuteScalarAsync<int>(sql, customer);
    }

    public async Task UpdateAsync(Customer customer)
    {
        using var conn = CreateConnection();
        const string sql = """
            UPDATE Customer 
            SET CustomerName = @CustomerName,
                MobileNo1 = @MobileNo1,
                MobileNo2 = @MobileNo2,
                Status = @Status
            WHERE CustomerID = @CustomerID
            """;
        await conn.ExecuteAsync(sql, customer);
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = CreateConnection();
        const string sql = "DELETE FROM Customer WHERE CustomerID = @Id";
        await conn.ExecuteAsync(sql, new { Id = id });
    }
}
