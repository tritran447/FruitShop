using AutoMapper;
using BusinessLogicLayer.Dtos;
using DataAccessLayer.Repositories.Customers;
using DataAccessLayer.Models;

namespace BusinessLogicLayer.Services.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities;
        }

        public async Task<Customer?> GetByIdAsync(int customerId)
        {
            var entity = await _repository.GetByIdAsync(customerId);
            return entity;
        }

        public async Task<CustomerDto> CreateAsync(CustomerDto dto)
        {
            var entity = _mapper.Map<Customer>(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<CustomerDto>(entity);
        }

        public async Task<bool> UpdateAsync(CustomerDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.CustomerId);
            if (entity == null)
                return false;

            _mapper.Map(dto, entity);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int customerId)
        {
            var entity = await _repository.GetByIdAsync(customerId);
            if (entity == null)
                return false;

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
