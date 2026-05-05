using TailorApp.Models;

namespace TailorApp.Repositories;

public interface ISizeRepository
{
    Task<IEnumerable<Size>> GetByCustomerIdAsync(int customerId);
    Task<Size?> GetByIdAsync(int sizeId);
    Task<int> CreateAsync(Size size);
    Task UpdateAsync(Size size);
    Task DeleteAsync(int sizeId);
    Task<int> GetNextRegisterNoAsync(int customerId);
}
