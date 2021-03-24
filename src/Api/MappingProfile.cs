using AutoMapper;
using Yaroshinski.Blog.Api.Models;
using Yaroshinski.Blog.Application.DTO;

namespace Yaroshinski.Blog.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AuthorDto, AuthorResponse>().ReverseMap();

            CreateMap<AuthorDto, AuthenticateResponse>().ReverseMap();

            CreateMap<RegisterRequest, AuthorDto>().ReverseMap();

            CreateMap<CreateAuthorRequest, AuthorDto>().ReverseMap();

            CreateMap<UpdateAuthorRequest, AuthorDto>().ReverseMap();
        }
    }
}