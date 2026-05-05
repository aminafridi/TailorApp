using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using TailorApp.Models;

namespace TailorApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TailorShopDB") ?? "";
        }

        public async Task<User?> GetUserByCredentialsAsync(string loginName, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                SELECT UserID, Name, LoginName, Password, Status
                FROM Users
                WHERE LoginName = @LoginName AND Password = @Password AND Status = 1";

            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { LoginName = loginName, Password = password });
        }
    }
}
