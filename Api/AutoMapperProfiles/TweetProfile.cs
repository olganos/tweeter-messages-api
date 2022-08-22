using Api.Dto;
using AutoMapper;
using Core.Commands;
using Core.Entities;
using DataLayer;

namespace AutoMapperProfiles;

public class TweetProfile : Profile
{
    public TweetProfile()
    {
        AllowNullCollections = true;
        CreateMap<Tweet, TweetDto>()
            .ReverseMap();

        CreateMap<Reply, ReplyDto>()
            .ReverseMap();

        CreateMap<TweetDto, CreateTweetCommand>()
            .ReverseMap();
    }
}