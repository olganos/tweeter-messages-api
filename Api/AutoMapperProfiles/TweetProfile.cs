using Api.Dto;
using AutoMapper;
using DataLayer;

namespace AutoMapperProfiles;
public class TweetProfile : Profile
{
    public TweetProfile()
    {
        AllowNullCollections = true;
        CreateMap<Tweet, TweetDto>()
            .ReverseMap();
    }
}