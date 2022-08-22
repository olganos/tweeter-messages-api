using Api.Dto;
using AutoMapper;
using Core.Commands;

namespace AutoMapperProfiles;

public class TweetProfile : Profile
{
    public TweetProfile()
    {
        AllowNullCollections = true;
        //CreateMap<Tweet, TweetDto>()
        //    .ReverseMap();

        //CreateMap<Reply, ReplyDto>()
        //    .ReverseMap();

        CreateMap<TweetDto, CreateTweetCommand>()
            .ReverseMap();

        CreateMap<TweetDto, AddReplyCommand>()
           .ReverseMap();

        CreateMap<TweetDto, UpdateTweetCommand>()
           .ReverseMap();

        CreateMap<TweetDto, DeleteTweetCommand>()
           .ReverseMap();
    }
}