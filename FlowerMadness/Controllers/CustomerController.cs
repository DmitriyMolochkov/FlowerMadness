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
using DAL.Models;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;

namespace FlowerMadness.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Roles = "manager, administrator")]
    public class CustomerController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IEmailSender _emailSender;


        public CustomerController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<CustomerController> logger, IEmailSender emailSender)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _emailSender = emailSender;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<CustomerViewModel>))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult GetAllCustomers()
        {
            var allCustomers = _unitOfWork.Customers.GetAllCustomersData();
            return Ok(_mapper.Map<List<CustomerViewModel>>(allCustomers));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(CustomerViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
                return NotFound();
            return Ok(_mapper.Map<CustomerViewModel>(customer));
        }

        [HttpGet("Orders")]
        [ProducesResponseType(200, Type = typeof(List<OrderViewModel>))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult GetAllOrders([FromQuery] OrderStatus? status)
        {
            var allOrders = _unitOfWork.Orders.GetAllOrders(status);
            return Ok(_mapper.Map<List<OrderViewModel>>(allOrders));
        }

        [HttpGet("Orders/{id}")]
        [ProducesResponseType(200, Type = typeof(OrderViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult GetOrderById(int id)
        {
            var order = _unitOfWork.Orders.GetOrderById(id);
            if (order == null)
                return NotFound();
            
            return Ok(_mapper.Map<OrderViewModel>(order));
        }

        [HttpGet("{id}/OrderHistory")]
        [ProducesResponseType(200, Type = typeof(List<OrderViewModel>))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult GetOrderHistoryByCustomerId(int id, [FromQuery] OrderStatus? status)
        {
            var orders = _unitOfWork.Orders.GetOrderHistory(id, status);
            return Ok(_mapper.Map<List<OrderViewModel>>(orders));
        }

        [HttpPut("Orders/{id}")]
        [ProducesResponseType(200, Type = typeof(OrderViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult ChangeOrderStatus(int id, [FromQuery] OrderStatus status)
        {
            var order = _unitOfWork.Orders.GetOrderById(id);
            if (order == null)
                return NotFound();

            order.Status = (byte) status;
            _unitOfWork.Orders.Update(order);
            _unitOfWork.SaveChanges();
            
            return Ok(_mapper.Map<OrderViewModel>(order));
        }
    }
}
