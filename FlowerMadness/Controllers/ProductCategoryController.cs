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
using AutoMapper;
using DAL;

namespace FlowerMadness.Controllers
{
    [Route("api/[controller]")]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IEmailSender _emailSender;
        private readonly IAuthorizationService _authorizationService;

        public ProductCategoryController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<ProductCategoryController> logger, IEmailSender emailSender,
            IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _emailSender = emailSender;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<ProductCategoryViewModel>))]
        public async Task<IActionResult> Get()
        {
            var categories = await _unitOfWork.ProductCategories.GetAllAsync();
            return Ok(_mapper.Map<List<ProductCategoryViewModel>>(categories));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(ProductCategoryViewModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _unitOfWork.ProductCategories.GetByIdAsync(id);

            if (category != null)
                return Ok(_mapper.Map<ProductCategoryViewModel>(category));
            else
                return NotFound(id);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ProductCategoryViewModel))]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Roles = "manager, administrator")]
        public async Task<IActionResult> Post([FromBody] ProductCategoryDtoModel model)
        {
            //if (!(await _authorizationService.AuthorizeAsync(this.User, new string[] { }, Authorization.Policies.AssignAllowedRolesPolicy)).Succeeded)
            //    return new ChallengeResult();

            var category = _mapper.Map<ProductCategory>(model);
            await _unitOfWork.ProductCategories.PostAsync(category);
            category.DateCreated = DateTime.UtcNow;
            category.DateModified = DateTime.UtcNow;
            _unitOfWork.SaveChanges();
            return Ok(_mapper.Map<ProductCategoryViewModel>(category));

        }

        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(ProductCategoryViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Roles = "manager, administrator")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductCategoryDtoModel model)
        {
            //if (!(await _authorizationService.AuthorizeAsync(this.User, new string[] { }, Authorization.Policies.AssignAllowedRolesPolicy)).Succeeded)
            //    return new ChallengeResult();
            var category = await _unitOfWork.ProductCategories.GetByIdAsync(id);

            if (category == null)
                return NotFound(id);

            category = _mapper.Map(model, category);
            _unitOfWork.ProductCategories.Put(category);
            category.DateModified = DateTime.UtcNow;
            _unitOfWork.SaveChanges();
            return Ok(_mapper.Map<ProductCategoryViewModel>(category));

        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Roles = "manager, administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            //if (!(await _authorizationService.AuthorizeAsync(this.User, new string[] { }, Authorization.Policies.AssignAllowedRolesPolicy)).Succeeded)
            //    return new ChallengeResult();

            var category = await _unitOfWork.ProductCategories.GetByIdAsync(id);

            if (category == null)
                return Ok();

            await _unitOfWork.ProductCategories.DeleteAsync(id);
            _unitOfWork.SaveChanges();
            return Ok();
        }
    }
}
