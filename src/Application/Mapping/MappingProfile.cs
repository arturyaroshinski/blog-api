using AutoMapper;
using Yaroshinski.Blog.Application.CQRS.Commands.Create;
using Yaroshinski.Blog.Application.DTO;
using Yaroshinski.Blog.Application.Models;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Author, AuthorDto>().ReverseMap();
            CreateMap<RefreshToken, RefreshTokenDto>().ReverseMap();
            CreateMap<Post, PostDto>().ReverseMap();
            CreateMap<Comment, CommentDto>().ReverseMap();
            CreateMap<Tag, TagDto>().ReverseMap();
            
            CreateMap<Author, AuthenticateResponse>().ReverseMap();
            CreateMap<Author, CreateAuthorCommand>().ReverseMap();
        }
    }
}
