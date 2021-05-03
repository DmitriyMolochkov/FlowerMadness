using AutoMapper;
using DAL.Core;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerMadness.ViewModels
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, UserViewModel>()
                   .ForMember(d => d.Roles, map => map.Ignore());
            CreateMap<UserViewModel, ApplicationUser>()
                .ForMember(d => d.Roles, map => map.Ignore())
                .ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

            CreateMap<UserCreateModel, ApplicationUser>()
                .ForMember(d => d.Roles, map => map.Ignore());
            CreateMap<UserUpdateModel, ApplicationUser>()
                .ForMember(d => d.Roles, map => map.Ignore());

            CreateMap<ApplicationUser, UserEditViewModel>()
                .ForMember(d => d.Roles, map => map.Ignore());
            CreateMap<UserEditViewModel, ApplicationUser>()
                .ForMember(d => d.Roles, map => map.Ignore())
                .ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

            CreateMap<ApplicationUser, UserPatchViewModel>()
                .ReverseMap();

            CreateMap<ApplicationRole, RoleViewModel>()
                //.ForMember(d => d.Permissions, map => map.MapFrom(s => s.Claims))
                .ForMember(d => d.UsersCount, map => map.MapFrom(s => s.Users != null ? s.Users.Count : 0))
                .ReverseMap();
            CreateMap<RoleViewModel, ApplicationRole>()
                .ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

            CreateMap<IdentityRoleClaim<string>, ClaimViewModel>()
                .ForMember(d => d.Type, map => map.MapFrom(s => s.ClaimType))
                .ForMember(d => d.Value, map => map.MapFrom(s => s.ClaimValue))
                .ReverseMap();

            CreateMap<ApplicationPermission, PermissionViewModel>()
                .ReverseMap();

            CreateMap<IdentityRoleClaim<string>, PermissionViewModel>()
                .ConvertUsing(s => (PermissionViewModel)ApplicationPermissions.GetPermissionByValue(s.ClaimValue));

            CreateMap<Customer, CustomerViewModelForOrder>()
                .ForMember(x => x.Gender, map => map.MapFrom(y => y.Gender.ToString()))
                .ReverseMap();

            CreateMap<CustomerDtoModel, Customer>()
                .ForMember(x => x.DateModified, map => map.MapFrom(y => DateTime.UtcNow))
                ;

            CreateMap<Customer, CustomerViewModel>()
                .IncludeBase<Customer, CustomerViewModelForOrder>()
                .ReverseMap();

            CreateMap<Product, ProductViewModel>()
                .ForMember(x => x.ProductCategoryName, map => map.MapFrom(y => y.ProductCategory.Name))
                .ReverseMap();

            CreateMap<ProductDtoModel, Product>();

            CreateMap<Product, ProductForCustomerViewModel>()
                .ForMember(x => x.ProductCategoryName, map => map.MapFrom(y => y.ProductCategory.Name))
                ;
            
            CreateMap<Order, OrderViewModel>()
                .ForMember(x => x.Status, map => map.MapFrom(y => (OrderStatus) y.Status));
            //.ReverseMap();
            
            CreateMap<ProductCategoryDtoModel, ProductCategory>();

            CreateMap<ProductCategory, ProductCategoryViewModel>();

            CreateMap<ApplicationUser, Customer>()
                .ForMember(x => x.Id, map => map.Ignore())
                .ForMember(x => x.ApplicationUserId, map => map.MapFrom(y => y.Id))
                .ForMember(x => x.Name, map => map.MapFrom(y => y.FullName))
                .ForMember(x => x.CreatedDate, map => map.MapFrom(y => DateTime.UtcNow))
                .ForMember(x => x.UpdatedDate, map => map.MapFrom(y => DateTime.UtcNow))
                .ForMember(x => x.DateCreated, map => map.MapFrom(y => DateTime.UtcNow))
                .ForMember(x => x.DateModified, map => map.MapFrom(y => DateTime.UtcNow))
                ;

            CreateMap<Customer, Order>()
                .ForMember(x => x.Id, map => map.Ignore())
                //.ForMember(x => x.CustomerId, map => map.MapFrom(y => y.Id))
                .ForMember(x => x.CreatedDate, map => map.MapFrom(y => DateTime.UtcNow))
                .ForMember(x => x.UpdatedDate, map => map.MapFrom(y => DateTime.UtcNow))
                .ForMember(x => x.DateCreated, map => map.MapFrom(y => DateTime.UtcNow))
                .ForMember(x => x.DateModified, map => map.MapFrom(y => DateTime.UtcNow))
                ;

            //CreateMap<Order, OrderDetail>()
            //    .ForMember(x => x.Id, map => map.Ignore())
            //    .ForMember(x => x.Discount, map => map.Ignore())
                //.ForMember(x => x.OrderId, map => map.MapFrom(y => y.Id))
                
                ;
            
            CreateMap<OrderDetail, OrderDetailViewModel>()
                ;

            CreateMap<OrderDetailDtoModel, OrderDetail > ()
                ;
        }
    }
}
