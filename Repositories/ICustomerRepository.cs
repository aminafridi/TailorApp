using TailorApp.Models;

namespace TailorApp.Repositories;

public interface ICustomerRepository
{
    Task<PagedResult<Customer>> GetPagedAsync(string? search, int page = 1, int pageSize = 50);
    Task<Customer?> GetByIdAsync(int id);
    Task<DashboardViewModel> GetDashboardStatsAsync();
    Task<int> CreateAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(int id);
}
