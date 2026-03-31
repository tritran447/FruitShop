using BusinessLogicLayer.Dtos;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Customers 
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int customerId);
        Task<CustomerDto> CreateAsync(CustomerDto dto);
        Task<bool> UpdateAsync(CustomerDto dto);
        Task<bool> DeleteAsync(int customerId);
    }
}
