using AutoMapper;
using Yaroshinski.Blog.Api.Models;
using Yaroshinski.Blog.Application.DTO;

namespace Yaroshinski.Blog.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AuthorDto, AccountResponse>().ReverseMap();

            CreateMap<AuthorDto, AuthenticateResponse>().ReverseMap();

            CreateMap<RegisterRequest, AuthorDto>().ReverseMap();

            CreateMap<CreateRequest, AuthorDto>().ReverseMap();

            CreateMap<UpdateRequest, AuthorDto>().ReverseMap();
        }
    }
}