using AutoMapper;
using ShoppingOnline.Areas.Admin.Models;
using ShoppingOnline.Models;
using ShoppingOnline.ViewModels;

namespace ShoppingOnline.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Category, AddCategoryViewModel>();
            CreateMap<AddCategoryViewModel, Category >();
            CreateMap<Category, EditCategoryViewModel>();
            CreateMap<EditCategoryViewModel, Category>();

            CreateMap<News, AddNewViewModel>();
            CreateMap<AddNewViewModel, News>();
            CreateMap<News, EditNewViewModel>().ForMember(dest => dest.Image, opt => opt.Ignore());
            CreateMap<EditNewViewModel, News>();

            CreateMap<Posts, AddPostViewModel>();
            CreateMap<AddPostViewModel, Posts>();
            CreateMap<Posts, EditPostViewModel>().ForMember(dest => dest.Image, opt => opt.Ignore());
            CreateMap<EditPostViewModel, Posts>();

			CreateMap<Product, AddProductViewModel>();
			CreateMap<AddProductViewModel, Product>();
			CreateMap<Product, EditProductViewModel>();
			CreateMap<EditProductViewModel, Product>();

            CreateMap<ContactViewModel, Contact>();
       
        }
       
    }
}
