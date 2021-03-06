﻿using AutoMapper;
using DAL;
using DAL.Core;
using DAL.Core.Interfaces;
using DAL.Models;
using FlowerMadness.Authorization;
using FlowerMadness.Helpers;
using FlowerMadness.ViewModels;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerMadness.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IEmailSender _emailSender;
        private readonly IAuthorizationService _authorizationService;

        public ProductController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<ProductController> logger, IEmailSender emailSender,
            IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _emailSender = emailSender;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<ProductViewModel>))]
        public async Task<IActionResult> Get(int? productCategoryId, decimal? maxPrice, decimal? minPrice, string search)
        {
            var products = await _unitOfWork.Products.GetAllWithFiltersAsync(productCategoryId, maxPrice,minPrice, search);
            return Ok(_mapper.Map<List<ProductViewModel>>(products));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(ProductViewModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product != null)
                return Ok(_mapper.Map<ProductViewModel>(product));
            else
                return NotFound(id);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ProductViewModel))]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Roles = "manager, administrator")]
        public async Task<IActionResult> Post([FromBody]ProductDtoModel model)
        {
            //if (!(await _authorizationService.AuthorizeAsync(this.User, new string[] { }, Authorization.Policies.AssignAllowedRolesPolicy)).Succeeded)
            //    return new ChallengeResult();

            var product = _mapper.Map<Product>(model);
            await _unitOfWork.Products.PostAsync(product);
            product.DateCreated = DateTime.UtcNow;
            product.DateModified = DateTime.UtcNow;
            _unitOfWork.SaveChanges();
            return Ok(_mapper.Map<ProductViewModel>(product));
            
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(ProductViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Roles = "manager, administrator")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductDtoModel model)
        {
            //if (!(await _authorizationService.AuthorizeAsync(this.User, new string[] { }, Authorization.Policies.AssignAllowedRolesPolicy)).Succeeded)
            //    return new ChallengeResult();
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
                return NotFound(id);
                
            product = _mapper.Map(model, product);
            _unitOfWork.Products.Put(product);
            product.DateModified = DateTime.UtcNow;
            _unitOfWork.SaveChanges();
            return Ok(_mapper.Map<ProductViewModel>(product));
           
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Roles = "manager, administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            //if (!(await _authorizationService.AuthorizeAsync(this.User, new string[] { }, Authorization.Policies.AssignAllowedRolesPolicy)).Succeeded)
            //    return new ChallengeResult();

            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
                return Ok();
            
            await _unitOfWork.Products.DeleteAsync(id);
            _unitOfWork.SaveChanges();
            return Ok();
        }
    }
}
