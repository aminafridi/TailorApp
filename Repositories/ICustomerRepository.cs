using TailorApp.Models;

namespace TailorApp.Repositories;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync(string? search = null);
    Task<Customer?> GetByIdAsync(int id);
    Task<DashboardViewModel> GetDashboardStatsAsync();
    Task<int> CreateAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(int id);
}
