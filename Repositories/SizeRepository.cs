using Dapper;
using Microsoft.Data.SqlClient;
using TailorApp.Models;

namespace TailorApp.Repositories;

public class SizeRepository(string connectionString) : ISizeRepository
{
    private SqlConnection CreateConnection() => new(connectionString);

    public async Task<IEnumerable<Size>> GetByCustomerIdAsync(int customerId)
    {
        using var conn = CreateConnection();
        const string sql = """
            SELECT s.*, c.CustomerName
            FROM Size s
            INNER JOIN Customer c ON c.CustomerID = s.Customer_ID
            WHERE s.Customer_ID = @CustomerId
            ORDER BY s.RegisterNo DESC
            """;
        return await conn.QueryAsync<Size>(sql, new { CustomerId = customerId });
    }

    public async Task<Size?> GetByIdAsync(int sizeId)
    {
        using var conn = CreateConnection();
        const string sql = """
            SELECT s.*, c.CustomerName
            FROM Size s
            INNER JOIN Customer c ON c.CustomerID = s.Customer_ID
            WHERE s.SizeID = @SizeId
            """;
        return await conn.QuerySingleOrDefaultAsync<Size>(sql, new { SizeId = sizeId });
    }

    public async Task<int> GetNextRegisterNoAsync(int customerId)
    {
        using var conn = CreateConnection();
        const string sql = """
            SELECT ISNULL(MAX(RegisterNo), 0) + 1
            FROM Size WHERE Customer_ID = @CustomerId
            """;
        return await conn.ExecuteScalarAsync<int>(sql, new { CustomerId = customerId });
    }

    public async Task<int> CreateAsync(Size size)
    {
        using var conn = CreateConnection();
        const string sql = """
            INSERT INTO Size (
                Customer_ID, RegisterNo, Lambai, Bazo, BazoType, BazoDetail,
                Tera, Calar, CalarType, CalarDetail, Chati, Kamar,
                Ghera, GheraType, ShalwarLambai, Pancha,
                IsDoubleSidePocket, IsFrontPocket, IsShalwarPocket, IsCheckPatiKaj,
                Pati, Design, OtherDetails
            ) VALUES (
                @Customer_ID, @RegisterNo, @Lambai, @Bazo, @BazoType, @BazoDetail,
                @Tera, @Calar, @CalarType, @CalarDetail, @Chati, @Kamar,
                @Ghera, @GheraType, @ShalwarLambai, @Pancha,
                @IsDoubleSidePocket, @IsFrontPocket, @IsShalwarPocket, @IsCheckPatiKaj,
                @Pati, @Design, @OtherDetails
            );
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;
        return await conn.ExecuteScalarAsync<int>(sql, size);
    }

    public async Task UpdateAsync(Size size)
    {
        using var conn = CreateConnection();
        const string sql = """
            UPDATE Size SET
                Lambai = @Lambai, Bazo = @Bazo, BazoType = @BazoType, BazoDetail = @BazoDetail,
                Tera = @Tera, Calar = @Calar, CalarType = @CalarType, CalarDetail = @CalarDetail,
                Chati = @Chati, Kamar = @Kamar, Ghera = @Ghera, GheraType = @GheraType,
                ShalwarLambai = @ShalwarLambai, Pancha = @Pancha,
                IsDoubleSidePocket = @IsDoubleSidePocket, IsFrontPocket = @IsFrontPocket,
                IsShalwarPocket = @IsShalwarPocket, IsCheckPatiKaj = @IsCheckPatiKaj,
                Pati = @Pati, Design = @Design, OtherDetails = @OtherDetails
            WHERE SizeID = @SizeID
            """;
        await conn.ExecuteAsync(sql, size);
    }

    public async Task DeleteAsync(int sizeId)
    {
        using var conn = CreateConnection();
        const string sql = "DELETE FROM Size WHERE SizeID = @SizeId";
        await conn.ExecuteAsync(sql, new { SizeId = sizeId });
    }
}
