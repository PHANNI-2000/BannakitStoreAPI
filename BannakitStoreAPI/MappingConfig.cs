using AutoMapper;
using BannakitStoreApi.Models;
using BannakitStoreApi.Models.Dto;

namespace BannakitStoreApi
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryDTO, Category>();

            CreateMap<Brand, BrandDTO>();
            CreateMap<BrandDTO, Brand>();

            CreateMap<Product, ProductDTO>();
            CreateMap<ProductDTO, Product>();

            CreateMap<PaymentType, PaymentTypeDTO>();
            CreateMap<PaymentTypeDTO, PaymentType>();

            CreateMap<Stock, StockDTO>();
            CreateMap<StockDTO, Stock>();

            CreateMap<Order, OrderDTO>();
            CreateMap<OrderDTO, Order>();

            CreateMap<Role, RoleDTO>();
            CreateMap<RoleDTO, Role>();

            CreateMap<Department, DepartmentDTO>();
            CreateMap<DepartmentDTO, Department>();

            CreateMap<Employee, ManagementDTO>();
            CreateMap<ManagementDTO, Employee>();

            CreateMap<Image, ImageDTO>();
            CreateMap<ImageDTO, Image>();

            CreateMap<Category, CategoryCreateDTO>().ReverseMap();
            CreateMap<Category, CategoryUpdateDTO>().ReverseMap();

            CreateMap<Brand, BrandEntryDTO>().ReverseMap();

            CreateMap<Product, ProductCreateDTO>().ReverseMap();
            CreateMap<Product, ProductUpdateDTO>().ReverseMap();

            CreateMap<PaymentType, PaymentTypeEntryDTO>().ReverseMap();

            CreateMap<Stock, StockEntryDTO>().ReverseMap();

            CreateMap<Role, RoleEntryDTO>().ReverseMap();

            CreateMap<Employee, ManagementDTO>().ReverseMap();

            CreateMap<Image, ImageDTO>().ReverseMap();

            CreateMap<User, UserDTO>().ReverseMap();
        }
    }
}
