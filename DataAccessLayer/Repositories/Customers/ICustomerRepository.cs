using DataAccessLayer.Models;

namespace DataAccessLayer.Repositories.Customers
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int customerId);
        Task<Customer?> GetByEmailAsync(string email);
        Task AddAsync(Customer customer);
        void Update(Customer customer);
        void Delete(Customer customer);
        Task<int> SaveChangesAsync();
    }
}
