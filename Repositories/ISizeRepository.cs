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
    Task<PagedResult<Size>> GetPagedAsync(SizeSearch filter, int page = 1, int pageSize = 50);

    // Name of the customer whose record already uses this register number
    // (ignoring record exceptSizeId), or null when the number is free.
    Task<string?> GetRegisterNoOwnerAsync(int registerNo, int? exceptSizeId = null);
}
