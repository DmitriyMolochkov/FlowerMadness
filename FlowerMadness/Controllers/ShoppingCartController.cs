using AutoMapper;
using DAL;
using FlowerMadness.Helpers;
using FlowerMadness.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Core.Interfaces;
using DAL.Models;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;

namespace FlowerMadness.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Roles = "user, manager, administrator")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IEmailSender _emailSender;
        private readonly IAccountManager _accountManager;

        public ShoppingCartController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            ILogger<ShoppingCartController> logger, 
            IEmailSender emailSender,
            IAccountManager accountManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _emailSender = emailSender;
            _accountManager = accountManager;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(OrderViewModel))]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCurrentOrder()
        {
            var order = await GetOrderAsync(null);
            return Ok(_mapper.Map<OrderViewModel>(order));
        }

        [HttpGet("OrderHistory")]
        [ProducesResponseType(200, Type = typeof(OrderViewModel))]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetOrderHistory([FromQuery] OrderStatus? status)
        {
            var id = Utilities.GetUserId(this.User);
            var orders = _unitOfWork.Orders.GetOrderHistory(id, status);
            return Ok(_mapper.Map<List<OrderViewModel>>(orders));
        }

        [HttpPost("ChangeProductCount")]
        [ProducesResponseType(201, Type = typeof(OrderViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangeProductCount([FromBody]OrderDetailDtoModel model)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(model.ProductId);

            if (product == null)
                return NotFound(model.ProductId);

            var order = await GetOrderAsync(null);
            

            var orderDetail = _unitOfWork.OrderDetails.GetCurrentOrderDetailForOrder(order.Id, model.ProductId);
            if (orderDetail == null)
            {
                orderDetail = _mapper.Map(model, new OrderDetail());
                order.OrderDetails.Add(orderDetail);
            }

            orderDetail.UnitPrice = product.SellingPrice;
            orderDetail.Quantity = model.Quantity;
            order.DateModified = DateTime.UtcNow;
            
            if (orderDetail.Quantity <= 0)
            {
                order.OrderDetails.Remove(orderDetail);
            }
            else
            {
                _unitOfWork.Orders.Update(order);
            }
            
            _unitOfWork.SaveChanges();
            
            return CreatedAtAction("ChangeProductCount", _mapper.Map<OrderViewModel>(order));
        }

        [HttpPut("ConfirmedOrder")]
        [ProducesResponseType(200, Type = typeof(OrderViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ConfirmedOrder()
        {
            var id = Utilities.GetUserId(this.User);

            var customer = _unitOfWork.Customers.GetCurrentCustomerForUser(id);

            if (customer == null)
                return NotFound();

            var order = _unitOfWork.Orders.GetCurrentOrderForCustomer(customer.Id, null);

            if (order == null || order.OrderDetails.Count == 0)
                return NotFound();


            if (string.IsNullOrEmpty(customer.PhoneNumber) ||
                string.IsNullOrEmpty(customer.Address) ||
                string.IsNullOrEmpty(customer.City) ||
                string.IsNullOrEmpty(customer.Email) ||
                string.IsNullOrEmpty(customer.Name))
            {
                return Conflict("Address, PhoneNumber, City, Email, Name must be filled");
            }
            
            order.Status = (byte) OrderStatus.Confirmed;
            order.DateModified = DateTime.UtcNow;
            _unitOfWork.Orders.Update(order);
            _unitOfWork.SaveChanges();

            return Ok(_mapper.Map<OrderViewModel>(order));
        }

        [HttpGet("MyInfo")]
        [ProducesResponseType(200, Type = typeof(CustomerViewModelForOrder))]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCustomerInfo()
        {
            var customer = await GetCustomerAsync();
            return Ok(_mapper.Map<CustomerViewModelForOrder>(customer));
        }

        [HttpPut("MyInfo")]
        [ProducesResponseType(200, Type = typeof(CustomerViewModelForOrder))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutCustomer([FromBody]CustomerDtoModel model)
        {
            var customer = await GetCustomerAsync();
            if (customer.Orders.Any(x => x.Status != (byte) OrderStatus.InProcess))
            {
                var orderInProgress = customer.Orders.FirstOrDefault(x => x.Status == (byte) OrderStatus.InProcess);
                customer = _mapper.Map<Customer>(model);
                customer.DateCreated = DateTime.UtcNow;
                customer.ApplicationUserId = Utilities.GetUserId(this.User);
                if (orderInProgress != null)
                    customer.Orders = new List<Order> {orderInProgress};
            }
            else
            {
                _mapper.Map(model, customer);
            }

            _unitOfWork.Customers.Update(customer);
            _unitOfWork.SaveChanges();

            return Ok(_mapper.Map<CustomerViewModelForOrder>(customer));
        }

        private async Task<Customer> GetCustomerAsync()
        {
            var id = Utilities.GetUserId(this.User);

            var customer = _unitOfWork.Customers.GetCurrentCustomerForUser(id);
            if (customer == null)
            {
                var user = await _accountManager.GetUserByIdAsync(id);
                if (user == null) return null;

                customer = _mapper.Map(user, new Customer());
                user.Customers.Add(customer);
            }

            _unitOfWork.Customers.Update(customer);
            _unitOfWork.SaveChanges();

            return customer;
        }

        private async Task<Order> GetOrderAsync([FromQuery] OrderStatus? status)
        {
            var customer = await GetCustomerAsync();

            var order = _unitOfWork.Orders.GetCurrentOrderForCustomer(customer.Id, status);
            if (order == null)
            {
                order = _mapper.Map(customer, new Order());
                customer.Orders.Add(order);
            }

            _unitOfWork.Customers.Update(customer);
            _unitOfWork.SaveChanges();
            
            return order;
        }
    }
}
