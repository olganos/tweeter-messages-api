using Api.Dto.Responses;
using AutoMapper;
using Core.Commands;

namespace AutoMapperProfiles;

public class TweetProfile : Profile
{
    public TweetProfile()
    {
        CreateMap<TweetCreateResponse, CreateTweetCommand>()
            .ReverseMap();

        CreateMap<AddReplyResponse, AddReplyCommand>()
            .ReverseMap();
    }
}