using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using TailorApp.Models;
using System.Data;

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
            
            return await connection.QuerySingleOrDefaultAsync<User>("sp_Users_Authenticate", 
                new { LoginName = loginName, Password = password }, 
                commandType: CommandType.StoredProcedure);
        }
    }
}
