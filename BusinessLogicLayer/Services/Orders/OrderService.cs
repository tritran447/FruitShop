using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogicLayer.Dtos;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.OrderDetails;
using DataAccessLayer.Repositories.Orders;

namespace BusinessLogicLayer.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IOrderDetailRepository _detailRepo;

        public OrderService(
            IOrderRepository orderRepo,
            IOrderDetailRepository detailRepo)
        {
            _orderRepo = orderRepo;
            _detailRepo = detailRepo;
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = await _orderRepo.GetAllAsync();
            return orders.Select(MapToDto);
        }

        public async Task<OrderDto?> GetByIdAsync(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            return order == null ? null : MapToDto(order);
        }

        public async Task<OrderDto> CreateAsync(OrderDto dto)
        {
            // Tạo entity Order
            var order = new Order
            {
                CustomerId = dto.CustomerId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = dto.OrderDetails.Sum(d => d.Price * d.Quantity),
                Status = dto.Status,
                ShipAddress = dto.ShipAddress
            };

            await _orderRepo.AddAsync(order);
            await _orderRepo.SaveChangesAsync();

            // Tạo chi tiết
            foreach (var d in dto.OrderDetails)
            {
                var detail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    Price = d.Price
                };
                await _detailRepo.AddAsync(detail);
            }
            await _detailRepo.SaveChangesAsync();

            // Reload lại Order cùng chi tiết để trả về DTO
            order = await _orderRepo.GetByIdAsync(order.OrderId)
                  ?? throw new Exception("Order not found after create");

            return MapToDto(order);
        }

        public async Task UpdateAsync(OrderDto dto)
        {
            var order = await _orderRepo.GetByIdAsync(dto.OrderId)
                        ?? throw new KeyNotFoundException("Order not found");

            // Cập nhật các trường
            order.ShipAddress = dto.ShipAddress;
            order.Status = dto.Status;
            order.TotalAmount = dto.OrderDetails.Sum(d => d.Price * d.Quantity);

            _orderRepo.Update(order);
            await _orderRepo.SaveChangesAsync();

            // (Ở đây bạn có thể sync chi tiết nếu cần)
        }

        public async Task DeleteAsync(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId)
                        ?? throw new KeyNotFoundException("Order not found");

            // Xóa chi tiết trước
            var details = await _detailRepo.GetByOrderIdAsync(orderId);
            foreach (var d in details)
                _detailRepo.Remove(d);
            await _detailRepo.SaveChangesAsync();

            // Xóa order
            _orderRepo.Remove(order);
            await _orderRepo.SaveChangesAsync();
        }

        private static OrderDto MapToDto(Order o)
        {
            return new OrderDto
            {
                OrderId = o.OrderId,
                CustomerId = o.CustomerId ?? 0,
                OrderDate = o.OrderDate ?? DateTime.MinValue,
                TotalAmount = o.TotalAmount ?? 0m,
                Status = o.Status ?? "",
                ShipAddress = o.ShipAddress ?? "",
                OrderDetails = o.OrderDetails
                    .Select(d => new OrderDetailDto
                    {
                        OrderDetailId = d.OrderDetailId,
                        OrderId = d.OrderId ?? 0,
                        ProductId = d.ProductId ?? 0,
                        Quantity = d.Quantity ?? 0,
                        Price = d.Price ?? 0m
                    })
                    .ToList()
            };
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto create)
        {
            var order = new Order
            {
                CustomerId = create.CustomerId,
                ShipAddress = create.ShipAddress,
                Status = "Completed",
                OrderDate = DateTime.UtcNow,
                TotalAmount = create.Details.Sum(d => d.Price * d.Quantity)
            };
            await _orderRepo.AddAsync(order);
            await _orderRepo.SaveChangesAsync();

            foreach (var d in create.Details)
            {
                await _detailRepo.AddAsync(new OrderDetail
                {
                    OrderId = order.OrderId,    // đây là lúc gán OrderId
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    Price = d.Price
                });
            }
            await _detailRepo.SaveChangesAsync();

            return MapToDto(order);
        }

        public async Task<IEnumerable<OrderHistoryDto>> GetHistoryByCustomerAsync(int customerId)
        {
            // 1) load all orders with details
            var allOrders = await _orderRepo.GetAllAsync();

            // 2) filter, sort, and map
            var userOrders = allOrders
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderHistoryDto
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate ?? DateTime.MinValue,
                    TotalAmount = o.TotalAmount ?? 0m,
                    Status = o.Status ?? "",
                    ShipAddress = o.ShipAddress ?? "",
                    Items = o.OrderDetails.Select(d => new OrderHistoryItemDto
                    {
                        ProductId = d.ProductId ?? 0,
                        ProductName = d.Product?.ProductName ?? "",    
                        Quantity = d.Quantity ?? 0,
                        Price = d.Price ?? 0m
                    }).ToList()
                });

            return userOrders;
        }
    }
}
